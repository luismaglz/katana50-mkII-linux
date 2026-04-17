using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using Microsoft.Extensions.Logging;

using Kataka.App.ViewModels;
using Kataka.Application.Katana;
using Kataka.Domain.KatanaState;
using Kataka.Domain.Midi;

namespace Kataka.App.Services;

/// <summary>Metadata returned alongside a full device-state read so the VM can update derived UI.</summary>
public record DeviceReadMetadata(
    int? DelayTimeMs,
    bool DelayTimeLoaded,
    bool PatchLevelLoaded,
    string StatusMessage,
    string PanelControlsStatus,
    string PedalControlsStatus,
    string AmpEditorStatus);

public interface IAmpSyncService
{
    // ── Observable outputs (device → app) ────────────────────────────────────

    /// <summary>Fires whenever a full device-state batch is read. Keys are parameter keys; values are raw wire bytes.</summary>
    IObservable<IReadOnlyDictionary<string, byte>> DeviceStateLoaded { get; }

    /// <summary>Fires when the device pushes a single-parameter update.</summary>
    IObservable<(string Key, byte Value)> DeviceParameterPushed { get; }

    /// <summary>Fires when the amp signals a panel-channel change.</summary>
    IObservable<string> PanelChannelPushed { get; }

    /// <summary>Status/detail metadata after a device read completes (success or failure).</summary>
    IObservable<DeviceReadMetadata> ReadCompleted { get; }

    /// <summary>Human-readable status lines for the VM's StatusMessage property.</summary>
    IObservable<string> StatusMessages { get; }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    /// <summary>
    ///     Wires up domain-state write-tracking subscriptions.
    ///     Call once after the VM's collections are fully populated.
    /// </summary>
    void Initialize(IAmpSyncState state);

    /// <summary>Builds the push-notification lookup and subscribes to amp events. Call after connect.</summary>
    void Activate();

    /// <summary>Unsubscribes from amp events. Call before disconnect.</summary>
    void Deactivate();

    /// <summary>Stops the write sync timer immediately. Call from the window Closing handler.</summary>
    void Shutdown();

    /// <summary>Clears the pending write queue and updates the write timer. Call on IsConnected change.</summary>
    void OnConnectionChanged(bool connected);

    // ── Write operations (app → device) ──────────────────────────────────────

    void QueueWrite(string key, byte value, string logMessage);
    void TrackPanelChannelChange(string displayName);
    void UpdateWriteSyncTimer();
    bool HasPendingWrites();
    string DescribePendingWrites();
    void ClearPendingWrites();

    Task<bool> TryRefreshAmpStateAsync();
    Task<bool> TryWritePatchLevelAsync();
}

public sealed class AmpSyncService : IAmpSyncService
{
    private static readonly TimeSpan WriteSyncDebounce = TimeSpan.FromMilliseconds(125);

    private readonly Dictionary<string, AmpControlViewModel> _ampControlsByKey = [];
    private readonly Dictionary<string, IBasePedal> _panelEffectsByKey = [];
    private readonly Dictionary<string, byte> _pendingWrites = [];
    private readonly IKatanaSession _session;
    private readonly IKatanaState _state;
    private readonly SemaphoreSlim _syncGate = new(1, 1);
    private readonly DispatcherTimer _writeSyncTimer;

    private Dictionary<string, AmpControlState> _domainAmpControlsByKey = [];
    private Dictionary<string, AmpControlState> _domainPanelStatesByKey = [];

    private int _flushRetryCount;
    private bool _isShuttingDown;
    private string? _pendingPanelChannel;
    private Dictionary<string, Action<byte>> _pushHandlerLookup = [];
    private IAmpSyncState _context = null!;

    // ── Observable subjects ───────────────────────────────────────────────────

    private readonly Subject<IReadOnlyDictionary<string, byte>> _deviceStateSubject = new();
    private readonly Subject<(string Key, byte Value)> _deviceParameterSubject = new();
    private readonly Subject<string> _panelChannelSubject = new();
    private readonly Subject<DeviceReadMetadata> _readCompletedSubject = new();
    private readonly Subject<string> _statusSubject = new();

    public IObservable<IReadOnlyDictionary<string, byte>> DeviceStateLoaded => _deviceStateSubject;
    public IObservable<(string Key, byte Value)> DeviceParameterPushed => _deviceParameterSubject;
    public IObservable<string> PanelChannelPushed => _panelChannelSubject;
    public IObservable<DeviceReadMetadata> ReadCompleted => _readCompletedSubject;
    public IObservable<string> StatusMessages => _statusSubject;

    private readonly ILogger<AmpSyncService> _logger;

    // ── Construction ──────────────────────────────────────────────────────────

    public AmpSyncService(IKatanaSession session, IKatanaState state, ILogger<AmpSyncService> logger)
    {
        _session = session;
        _state = state;
        _logger = logger;

        _writeSyncTimer = new DispatcherTimer { Interval = WriteSyncDebounce };
        _writeSyncTimer.Tick += async (_, _) =>
        {
            _writeSyncTimer.Stop();
            await FlushPendingWritesAsync();
            if (HasPendingWrites()) UpdateWriteSyncTimer();
        };
    }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    public void Initialize(IAmpSyncState state)
    {
        _context = state;

        _domainAmpControlsByKey = new Dictionary<string, AmpControlState>(_state.GetAmpControlsByKey());
        _domainPanelStatesByKey = new Dictionary<string, AmpControlState>(_state.GetPedalDomainControlsByKey());

        SubscribeDomainWriteTracking(_domainAmpControlsByKey.Values);
        SubscribeDomainWriteTracking(_domainPanelStatesByKey.Values);

        _ampControlsByKey.Clear();
        foreach (var control in state.AmpControls)
            _ampControlsByKey[control.Parameter.Key] = control;

        _panelEffectsByKey.Clear();
        foreach (var effect in state.PanelEffects)
        {
            _panelEffectsByKey[effect.Definition.SwitchParameter.Key] = effect;

            if (effect is ModFxPedalViewModel)
            {
                var captured = effect;
                effect.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(IBasePedal.IsEnabled))
                        TrackPanelEffectChange(captured);
                    else if (args.PropertyName == nameof(IBasePedal.SelectedTypeOption))
                        TrackPanelEffectTypeChange(captured);
                };
                effect.ParameterChanged += (_, args) => TrackDetailParamChange(args.Key, args.Value);
            }
        }

        state.Pedalboard.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(PedalboardViewModel.SelectedChainPattern)) return;
            if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
            var value = _context.Pedalboard.SelectedChainPattern;
            if (value < 0 || value >= PedalboardViewModel.ChainPatternNames.Length) return;
            _pendingWrites[KatanaMkIIParameterCatalog.ChainPattern.Key] = (byte)value;
            _logger.LogInformation("Queued panel sync: Chain Pattern -> {Name}.", PedalboardViewModel.ChainPatternNames[value]);
            UpdateWriteSyncTimer();
        };

        state.PedalFx.PropertyChanged += (_, args) => TrackPedalChange(args.PropertyName);

        UpdateWriteSyncTimer();
    }

    public void Activate()
    {
        BuildPushHandlerLookup();
        _session.PushNotificationReceived += OnAmpPushNotification;
        _session.PanelChannelChanged += OnAmpPanelChannelChanged;
    }

    public void Deactivate()
    {
        _session.PushNotificationReceived -= OnAmpPushNotification;
        _session.PanelChannelChanged -= OnAmpPanelChannelChanged;
        _pushHandlerLookup.Clear();
    }

    public void Shutdown()
    {
        _isShuttingDown = true;
        _writeSyncTimer.Stop();
    }

    public void OnConnectionChanged(bool connected)
    {
        if (!connected)
        {
            if (_pendingWrites.Count > 0 || _pendingPanelChannel is not null)
                _logger.LogInformation("Clearing pending sync queue after disconnect: {Pending}.", DescribePendingWrites());
            _pendingWrites.Clear();
            _pendingPanelChannel = null;
        }

        UpdateWriteSyncTimer();
    }

    // ── Change tracking ───────────────────────────────────────────────────────

    public void QueueWrite(string key, byte value, string logMessage)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        _pendingWrites[key] = value;
        _logger.LogInformation("Queued panel sync: {Message}.", logMessage);
        UpdateWriteSyncTimer();
    }

    public void TrackPanelChannelChange(string displayName)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        _pendingPanelChannel = displayName;
        _logger.LogInformation("Queued panel channel sync: {Channel}.", displayName);
        UpdateWriteSyncTimer();
    }

    // ── Timer control ─────────────────────────────────────────────────────────

    public void UpdateWriteSyncTimer()
    {
        _writeSyncTimer.Stop();
        if (_context is not null && _context.ActiveWriteSync && _context.IsConnected && HasPendingWrites())
            _writeSyncTimer.Start();
    }

    // ── State queries ─────────────────────────────────────────────────────────

    public bool HasPendingWrites() => _pendingWrites.Count > 0 || _pendingPanelChannel is not null;

    public string DescribePendingWrites()
    {
        if (_pendingWrites.Count == 0 && _pendingPanelChannel is null) return "no pending changes";
        var parts = new List<string>();
        if (_pendingWrites.Count > 0) parts.Add($"{_pendingWrites.Count} writes");
        if (_pendingPanelChannel is not null) parts.Add("channel");
        return string.Join(", ", parts);
    }

    public void ClearPendingWrites()
    {
        _pendingWrites.Clear();
        _pendingPanelChannel = null;
    }

    // ── Public read operations ────────────────────────────────────────────────

    public async Task<bool> TryRefreshAmpStateAsync()
    {
        await TryReadAmpControlsAsync();
        await TryReadPanelControlsAsync();
        await TryReadPedalControlsAsync();
        return true;
    }

    private async Task<bool> TryReadAmpControlsAsync()
    {
        try
        {
            _logger.LogInformation("Reading Katana amp editor controls.");
            var values = await _session.ReadParametersAsync(_context.AmpControls.Select(c => c.Parameter).ToArray());

            ApplyDeviceState(() =>
            {
                foreach (var control in _context.AmpControls)
                {
                    var value = values[control.Parameter.Key];
                    if (_domainAmpControlsByKey.TryGetValue(control.Parameter.Key, out var domainControl))
                        domainControl.Value = value;
                    _logger.LogInformation("{Name} reply: {Value}", control.DisplayName, value);
                }
            });

            EmitStatus("Amp editor controls read successfully.");
            _readCompletedSubject.OnNext(new DeviceReadMetadata(
                null, false, false,
                "Amp editor controls read successfully.",
                string.Empty,
                string.Empty,
                "Amp editor values were loaded from the Katana."));
            return true;
        }
        catch (Exception ex)
        {
            EmitStatus(ex.Message);
            _logger.LogError(ex, "Amp editor read failed.");
            _readCompletedSubject.OnNext(new DeviceReadMetadata(
                null, false, false,
                ex.Message,
                string.Empty,
                string.Empty,
                "Amp editor read failed."));
            return false;
        }
    }

    private async Task<bool> TryReadPanelControlsAsync()
    {
        try
        {
            _logger.LogInformation("Reading Katana panel controls.");

            var currentChannel = await _session.ReadCurrentPanelChannelAsync();
            if (currentChannel is not null)
            {
                var displayName = IAmpSyncState.ToPanelChannelDisplay(currentChannel.Value);
                ApplyDeviceState(() =>
                {
                    _logger.LogInformation("Current panel channel: {Channel}", displayName);
                    _panelChannelSubject.OnNext(displayName);
                });
            }

            var parameterList = _context.PanelEffects
                .SelectMany(e => e.GetSyncParameters())
                .Append(KatanaMkIIParameterCatalog.AmpType)
                .Append(KatanaMkIIParameterCatalog.CabinetResonance)
                .Append(KatanaMkIIParameterCatalog.AmpVariation)
                .Append(KatanaMkIIParameterCatalog.ChainPattern)
                .ToArray();
            var values = await _session.ReadParametersAsync(parameterList);

            ApplyDeviceState(() =>
            {
                foreach (var (key, rawValue) in values)
                {
                    if (_domainAmpControlsByKey.TryGetValue(key, out var ampControl))
                        ampControl.Value = rawValue;
                    else if (_domainPanelStatesByKey.TryGetValue(key, out var panelControl))
                        panelControl.Value = rawValue;
                }

                // ModFxPedalViewModel still uses the legacy ApplyAmpValues path.
                var intValues = values.ToDictionary(kvp => kvp.Key, kvp => (int)kvp.Value, StringComparer.Ordinal);
                foreach (var effect in _context.PanelEffects.OfType<ModFxPedalViewModel>())
                    effect.ApplyAmpValues(intValues);

                // ChainPattern drives Pedalboard directly.
                if (values.TryGetValue(KatanaMkIIParameterCatalog.ChainPattern.Key, out var chainValue) &&
                    chainValue < PedalboardViewModel.ChainPatternNames.Length)
                    _context.Pedalboard.SelectedChainPattern = chainValue;

                // Publish the full snapshot so the VM can react to any remaining derived state.
                _deviceStateSubject.OnNext(values);

                foreach (var effect in _context.PanelEffects)
                    _logger.LogInformation("{Name}: {State} / {Variation} / {Type}", effect.DisplayName, effect.IsEnabled ? "On" : "Off", effect.Variation, effect.TypeCaption);
            });

            var patchLevelLoaded = await TryRefreshPatchLevelAsync();
            var delayTimeMs = await TryRefreshDelayTimeAsync();
            var delayTimeLoaded = delayTimeMs.HasValue;

            var status = "Panel controls read successfully.";
            var detail = (patchLevelLoaded, delayTimeLoaded) switch
            {
                (true, true) => "Panel channel, patch level, effect toggles, variation colors, effect types, and delay time were loaded.",
                (true, false) => "Panel channel, patch level, effect toggles, variation colors, and effect types were loaded. Delay time refresh failed.",
                (false, true) => "Panel channel, effect toggles, variation colors, effect types, and delay time were loaded. Patch level mapping is still pending.",
                _ => "Panel channel, effect toggles, variation colors, and effect types were loaded. Patch level mapping is still pending, and delay time refresh failed."
            };

            EmitStatus(status);
            _readCompletedSubject.OnNext(new DeviceReadMetadata(
                delayTimeMs, delayTimeLoaded, patchLevelLoaded,
                status, detail, string.Empty, string.Empty));
            return true;
        }
        catch (Exception ex)
        {
            EmitStatus(ex.Message);
            _logger.LogError(ex, "Panel control read failed.");
            _readCompletedSubject.OnNext(new DeviceReadMetadata(
                null, false, false,
                ex.Message, "Panel control read failed.", string.Empty, string.Empty));
            return false;
        }
    }

    private async Task<bool> TryReadPedalControlsAsync()
    {
        try
        {
            _logger.LogInformation("Reading Katana pedal controls.");
            var values = await _session.ReadParametersAsync(_context.PedalFx.GetReadParameters().ToArray());

            ApplyDeviceState(() => _context.PedalFx.ApplyValues(values));

            _logger.LogInformation("Pedal FX: {State} / {Type} / {Position} / Foot Volume {Volume}",
                _context.PedalFx.IsEnabled ? "On" : "Off", _context.PedalFx.SelectedTypeOption,
                _context.PedalFx.SelectedPositionOption, _context.PedalFx.FootVolume);

            var status = "Pedal controls read successfully.";
            var detail = _context.PedalFx.IsWahMode
                ? "Pedal FX, wah controls, and foot volume were loaded."
                : "Pedal FX type and foot volume were loaded. Non-wah subtype controls are not surfaced yet.";

            EmitStatus(status);
            _readCompletedSubject.OnNext(new DeviceReadMetadata(
                null, false, false, status, string.Empty, detail, string.Empty));
            return true;
        }
        catch (Exception ex)
        {
            EmitStatus(ex.Message);
            _logger.LogError(ex, "Pedal control read failed.");
            _readCompletedSubject.OnNext(new DeviceReadMetadata(
                null, false, false, ex.Message, string.Empty, "Pedal control read failed.", string.Empty));
            return false;
        }
    }

    public async Task<bool> TryWritePatchLevelAsync()
    {
        if (!_context.PatchLevelMappingVerified) return false;
        try
        {
            var requested = Math.Clamp(
                _context.PatchLevel,
                KatanaMkIIParameterCatalog.PatchLevel.Minimum,
                KatanaMkIIParameterCatalog.PatchLevel.Maximum);
            _context.PatchLevel = await _session.WriteParameterAsync(KatanaMkIIParameterCatalog.PatchLevel, (byte)requested);
            _logger.LogInformation("Patch Level confirmed at {Level}.", _context.PatchLevel);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Patch level write failed.");
            return false;
        }
    }

    // ── Change-tracking helpers ───────────────────────────────────────────────

    private void TrackPanelEffectChange(IBasePedal effect)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        _pendingWrites[effect.Definition.SwitchParameter.Key] = effect.IsEnabled ? (byte)1 : (byte)0;
        _logger.LogInformation("Queued panel sync: {Name} -> {State}.", effect.DisplayName, effect.IsEnabled ? "On" : "Off");
        UpdateWriteSyncTimer();
    }

    private void TrackPanelEffectTypeChange(IBasePedal effect)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected
            || effect.Definition.TypeParameter is null) return;
        if (!effect.TryGetTypeValue(effect.SelectedTypeOption, out var typeValue)) return;
        _pendingWrites[effect.Definition.TypeParameter.Key] = typeValue;
        _logger.LogInformation("Queued panel type sync: {Name} -> {Type}.", effect.DisplayName, effect.SelectedTypeOption);
        UpdateWriteSyncTimer();
    }

    private void TrackDetailParamChange(string key, int value)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        _pendingWrites[key] = (byte)Math.Clamp(value, 0, 127);
        _logger.LogInformation("Queued detail param sync: {Key} -> {Value}.", key, value);
        UpdateWriteSyncTimer();
    }

    private void TrackPedalChange(string? propertyName)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        if (!_context.PedalFx.TryGetParameterValue(propertyName, out var parameter, out var value, out var description)) return;
        _pendingWrites[parameter.Key] = value;
        _logger.LogInformation("Queued pedal sync: {Name} -> {Description}.", parameter.DisplayName, description);
        UpdateWriteSyncTimer();
    }

    private void SubscribeDomainWriteTracking(IEnumerable<AmpControlState> states)
    {
        foreach (var state in states)
        {
            var captured = state;
            captured.ValueChanged += () =>
            {
                if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
                var value = Math.Clamp(captured.Value, captured.Minimum, captured.Maximum);
                _pendingWrites[captured.Parameter.Key] = (byte)value;
                _logger.LogInformation("Queued sync: {Name} -> {Value}.", captured.Parameter.DisplayName, value);
                UpdateWriteSyncTimer();
            };
        }
    }

    // ── Write flush loop ──────────────────────────────────────────────────────

    private async Task FlushPendingWritesAsync()
    {
        if (_isShuttingDown || !_context.ActiveWriteSync || !_context.IsConnected) return;
        if (!HasPendingWrites()) return;

        if (!_syncGate.Wait(0))
        {
            _logger.LogInformation("Queued write sync is already running; the latest changes will wait for the next flush.");
            return;
        }

        var snapshot = new Dictionary<string, byte>(StringComparer.Ordinal);
        string? channelSnapshot = null;

        try
        {
            foreach (var entry in _pendingWrites) snapshot[entry.Key] = entry.Value;
            channelSnapshot = _pendingPanelChannel;
            _pendingWrites.Clear();
            _pendingPanelChannel = null;

            var ampSnapshot = snapshot
                .Where(e => _ampControlsByKey.ContainsKey(e.Key))
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.Ordinal);

            var panelSwitchSnapshot = snapshot
                .Where(e => _panelEffectsByKey.ContainsKey(e.Key))
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.Ordinal);

            var panelTypeSnapshot = snapshot
                .Where(e => _context.PanelEffects.Any(ef => ef.Definition.TypeParameter?.Key == e.Key))
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.Ordinal);

            var pedalKeys = new HashSet<string>(
                _context.PedalFx.GetReadParameters().Select(p => p.Key), StringComparer.Ordinal);
            var pedalSnapshot = snapshot
                .Where(e => pedalKeys.Contains(e.Key))
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.Ordinal);

            var detailParamSnapshot = snapshot
                .Where(e => !ampSnapshot.ContainsKey(e.Key) && !panelSwitchSnapshot.ContainsKey(e.Key)
                            && !panelTypeSnapshot.ContainsKey(e.Key) && !pedalSnapshot.ContainsKey(e.Key))
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.Ordinal);

            _logger.LogInformation("Flushing queued sync: {Amp} amp, {Panel} panel, {PanelType} panel type, {Pedal} pedal, {Detail} detail, {Channel} channel change.",
                ampSnapshot.Count, panelSwitchSnapshot.Count, panelTypeSnapshot.Count, pedalSnapshot.Count, detailParamSnapshot.Count, channelSnapshot is null ? "no" : "1");

            await FlushPendingAmpWritesAsync(ampSnapshot);
            await FlushPendingPanelWritesAsync(channelSnapshot, panelSwitchSnapshot, panelTypeSnapshot, snapshot);
            await FlushPendingPedalWritesAsync(pedalSnapshot);
            await FlushPendingDetailParamWritesAsync(detailParamSnapshot);

            if (snapshot.Count > 0 || channelSnapshot is not null)
            {
                EmitStatus("Queued changes synced to the Katana.");
                _logger.LogInformation("Queued changes synced to the Katana.");
            }

            _flushRetryCount = 0;
        }
        catch (Exception ex)
        {
            foreach (var entry in snapshot) _pendingWrites[entry.Key] = entry.Value;
            _pendingPanelChannel ??= channelSnapshot;
            _flushRetryCount++;
            var backoffMs = Math.Min(100 * (1 << _flushRetryCount), 2000);
            EmitStatus(ex.Message);
            _logger.LogError(ex, "Queued write sync failed (retry {Retry}, backoff {Backoff}ms). Re-queued {Pending}.", _flushRetryCount, backoffMs, DescribePendingWrites());
            await Task.Delay(backoffMs);
        }
        finally
        {
            _syncGate.Release();
        }
    }

    // ── Flush helpers ─────────────────────────────────────────────────────────

    private async Task FlushPendingAmpWritesAsync(IReadOnlyDictionary<string, byte> ampWrites)
    {
        if (ampWrites.Count == 0) return;

        var orderedParameters = ampWrites.Keys
            .Select(key => _ampControlsByKey[key].Parameter)
            .OrderBy(p => p.Address[0]).ThenBy(p => p.Address[1])
            .ThenBy(p => p.Address[2]).ThenBy(p => p.Address[3])
            .ToList();

        var groupStart = 0;
        while (groupStart < orderedParameters.Count)
        {
            var group = new List<KatanaParameterDefinition> { orderedParameters[groupStart] };
            var groupIndex = groupStart + 1;
            while (groupIndex < orderedParameters.Count && CanBatchWrite(group[^1], orderedParameters[groupIndex]))
            {
                group.Add(orderedParameters[groupIndex]);
                groupIndex++;
            }
            var startAddress = group[0].Address.ToArray();
            var data = group.Select(p => ampWrites[p.Key]).ToArray();
            _logger.LogInformation("Writing amp sync batch: {Keys}.", DescribeAmpKeys(group.Select(p => p.Key)));
            await _session.WriteBlockAsync(startAddress, data);
            groupStart = groupIndex;
        }
    }

    private async Task FlushPendingPanelWritesAsync(
        string? channel,
        IReadOnlyDictionary<string, byte> panelSwitchWrites,
        IReadOnlyDictionary<string, byte> panelTypeWrites,
        IReadOnlyDictionary<string, byte> allWrites)
    {
        if (!string.IsNullOrWhiteSpace(channel))
        {
            _logger.LogInformation("Writing queued panel channel: {Channel}.", channel);
            await _session.SelectPanelChannelAsync(IAmpSyncState.ParsePanelChannelDisplay(channel));
        }

        foreach (var entry in panelSwitchWrites)
        {
            var parameter = _panelEffectsByKey[entry.Key].Definition.SwitchParameter;
            _logger.LogInformation("Writing queued panel effect: {Name} -> {State}.", _panelEffectsByKey[entry.Key].DisplayName, entry.Value != 0 ? "On" : "Off");
            await _session.WriteBlockAsync(parameter.Address, [entry.Value]);
        }

        foreach (var entry in panelTypeWrites)
        {
            var effect = _context.PanelEffects.First(p => p.Definition.TypeParameter?.Key == entry.Key);
            _logger.LogInformation("Writing queued panel type: {Name} -> {Type}.", effect.DisplayName, effect.ToTypeOption(entry.Value));
            await _session.WriteBlockAsync(effect.Definition.TypeParameter!.Address, [entry.Value]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.AmpType.Key, out var ampTypeValue))
        {
            _logger.LogInformation("Writing queued amp type: index {Value}.", ampTypeValue);
            await _session.WriteBlockAsync(KatanaMkIIParameterCatalog.AmpType.Address, [ampTypeValue]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.CabinetResonance.Key, out var cabinetValue))
        {
            _logger.LogInformation("Writing queued cabinet resonance: index {Value}.", cabinetValue);
            await _session.WriteBlockAsync(KatanaMkIIParameterCatalog.CabinetResonance.Address, [cabinetValue]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.AmpVariation.Key, out var ampVariationValue))
        {
            _logger.LogInformation("Writing queued amp variation: {Variation}.", ampVariationValue == 0 ? "TYPE 1" : "TYPE 2");
            await _session.WriteBlockAsync(KatanaMkIIParameterCatalog.AmpVariation.Address, [ampVariationValue]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.ChainPattern.Key, out var chainPatternValue))
        {
            var name = chainPatternValue < PedalboardViewModel.ChainPatternNames.Length
                ? PedalboardViewModel.ChainPatternNames[chainPatternValue]
                : chainPatternValue.ToString();
            _logger.LogInformation("Writing queued chain pattern: {Name}.", name);
            await _session.WriteBlockAsync(KatanaMkIIParameterCatalog.ChainPattern.Address, [chainPatternValue]);
        }
    }

    private async Task FlushPendingPedalWritesAsync(IReadOnlyDictionary<string, byte> pedalWrites)
    {
        if (pedalWrites.Count == 0) return;

        var orderedParameters = pedalWrites.Keys
            .Select(_context.PedalFx.GetParameter)
            .OrderBy(p => p.Address[0]).ThenBy(p => p.Address[1])
            .ThenBy(p => p.Address[2]).ThenBy(p => p.Address[3])
            .ToList();

        var groupStart = 0;
        while (groupStart < orderedParameters.Count)
        {
            var group = new List<KatanaParameterDefinition> { orderedParameters[groupStart] };
            var groupIndex = groupStart + 1;
            while (groupIndex < orderedParameters.Count && CanBatchWrite(group[^1], orderedParameters[groupIndex]))
            {
                group.Add(orderedParameters[groupIndex]);
                groupIndex++;
            }
            var startAddress = group[0].Address.ToArray();
            var data = group.Select(p => pedalWrites[p.Key]).ToArray();
            _logger.LogInformation("Writing pedal sync batch: {Keys}.", DescribePedalKeys(group.Select(p => p.Key)));
            await _session.WriteBlockAsync(startAddress, data);
            groupStart = groupIndex;
        }
    }

    private async Task FlushPendingDetailParamWritesAsync(IReadOnlyDictionary<string, byte> detailWrites)
    {
        if (detailWrites.Count == 0) return;

        var allDefinitions = _context.PanelEffects
            .SelectMany(e => e.GetSyncParameters())
            .GroupBy(p => p.Key, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);

        var orderedParameters = detailWrites.Keys
            .Where(allDefinitions.ContainsKey)
            .Select(k => allDefinitions[k])
            .OrderBy(p => p.Address[0]).ThenBy(p => p.Address[1])
            .ThenBy(p => p.Address[2]).ThenBy(p => p.Address[3])
            .ToList();

        foreach (var parameter in orderedParameters)
        {
            var value = detailWrites[parameter.Key];
            _logger.LogInformation("Writing detail param: {Name} -> {Value}.", parameter.DisplayName, value);
            await _session.WriteBlockAsync(parameter.Address.ToArray(), [value]);
        }
    }

    private async Task<bool> TryRefreshPatchLevelAsync()
    {
        if (!_context.PatchLevelMappingVerified) return false;
        try
        {
            _context.PatchLevel = await _session.ReadParameterAsync(KatanaMkIIParameterCatalog.PatchLevel);
            _logger.LogInformation("Patch Level reply: {Level}", _context.PatchLevel);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Patch level read failed.");
            return false;
        }
    }

    private async Task<int?> TryRefreshDelayTimeAsync()
    {
        try
        {
            var data = await _session.ReadBlockAsync(KatanaMkIIParameterCatalog.DelayTimeAddress, 2);
            var decoded = DecodeDelayTime(data);
            if (decoded <= 0)
            {
                _logger.LogInformation("Delay time reply did not contain a usable millisecond value.");
                return null;
            }

            _logger.LogInformation("Delay time reply: {Ms} ms.", decoded);
            return decoded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delay time read failed.");
            return null;
        }
    }

    // ── Push notification infrastructure ─────────────────────────────────────

    private void BuildPushHandlerLookup()
    {
        _pushHandlerLookup = new Dictionary<string, Action<byte>>(StringComparer.Ordinal);

        // Amp controls → domain state (VMs observe via ValueChanged).
        foreach (var control in _context.AmpControls)
        {
            var captured = control;
            _pushHandlerLookup[AddressToKey(captured.Parameter.Address)] = value =>
            {
                if (_domainAmpControlsByKey.TryGetValue(captured.Parameter.Key, out var domainControl))
                    domainControl.Value = value;
            };
        }

        // Panel effects: domain-migrated VMs use domain state; ModFx still uses ApplyAmpValues.
        foreach (var effect in _context.PanelEffects)
        {
            foreach (var param in effect.GetSyncParameters())
            {
                var capturedEffect = effect;
                var capturedKey = param.Key;
                _pushHandlerLookup[AddressToKey(param.Address)] = value =>
                {
                    if (_domainPanelStatesByKey.TryGetValue(capturedKey, out var domainControl))
                        domainControl.Value = value;
                    else
                        capturedEffect.ApplyAmpValues(new Dictionary<string, int>(StringComparer.Ordinal)
                        { [capturedKey] = value });
                };
            }
        }

        // AmpType / CabinetResonance / AmpVariation push → domain state.
        _pushHandlerLookup[AddressToKey(KatanaMkIIParameterCatalog.AmpType.Address)] = value =>
        {
            if (_domainAmpControlsByKey.TryGetValue(KatanaMkIIParameterCatalog.AmpType.Key, out var c)) c.Value = value;
        };
        _pushHandlerLookup[AddressToKey(KatanaMkIIParameterCatalog.CabinetResonance.Address)] = value =>
        {
            if (_domainAmpControlsByKey.TryGetValue(KatanaMkIIParameterCatalog.CabinetResonance.Key, out var c)) c.Value = value;
        };
        _pushHandlerLookup[AddressToKey(KatanaMkIIParameterCatalog.AmpVariation.Address)] = value =>
        {
            if (_domainAmpControlsByKey.TryGetValue(KatanaMkIIParameterCatalog.AmpVariation.Key, out var c)) c.Value = value;
        };

        // Panel channel push.
        _pushHandlerLookup[AddressToKey([0x00, 0x01, 0x00, 0x00])] = value =>
        {
            var displayName = value switch
            {
                0 => "Panel",
                1 => "CH A1",
                2 => "CH A2",
                5 => "CH B1",
                6 => "CH B2",
                _ => null
            };
            if (displayName is null) return;

            _logger.LogInformation("Amp channel changed (push): {Channel}", displayName);
            _panelChannelSubject.OnNext(displayName);

            _ = Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Task.Delay(150);
                await TryReadAmpControlsAsync();
                await TryReadPanelControlsAsync();
                await TryReadPedalControlsAsync();
            });
        };
    }

    private void OnAmpPushNotification(object? sender, SysExMessage message)
    {
        var bytes = message.Bytes;
        if (bytes[7] != 0x12) return;
        if (bytes.Count != 15 && bytes.Count != 16) return;

        var addressKey = AddressToKey([bytes[8], bytes[9], bytes[10], bytes[11]]);
        var value = bytes.Count == 16 ? bytes[13] : bytes[12];

        if (!_pushHandlerLookup.TryGetValue(addressKey, out var action)) return;

        Dispatcher.UIThread.Post(() => ApplyDeviceState(() => action(value)));
    }

    private void OnAmpPanelChannelChanged(object? sender, KatanaPanelChannel channel)
    {
        var displayName = IAmpSyncState.ToPanelChannelDisplay(channel);
        Dispatcher.UIThread.Post(() =>
        {
            _logger.LogInformation("Amp channel changed (push): {Channel}", displayName);
            _panelChannelSubject.OnNext(displayName);
        });
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void ApplyDeviceState(Action apply)
    {
        _context.SuppressChangeTracking = true;
        try { apply(); }
        finally { _context.SuppressChangeTracking = false; }
    }

    private void EmitStatus(string message) => _statusSubject.OnNext(message);

    private string DescribeAmpKeys(IEnumerable<string> keys) =>
        string.Join(", ", keys.Distinct(StringComparer.Ordinal).Select(k => _ampControlsByKey[k].DisplayName));

    private string DescribePedalKeys(IEnumerable<string> keys) =>
        string.Join(", ", keys.Distinct(StringComparer.Ordinal).Select(k => _context.PedalFx.GetParameter(k).DisplayName));

    private static bool CanBatchWrite(KatanaParameterDefinition previous, KatanaParameterDefinition current) =>
        previous.Address[0] == current.Address[0] &&
        previous.Address[1] == current.Address[1] &&
        previous.Address[2] == current.Address[2] &&
        previous.Address[3] + 1 == current.Address[3];

    private static string AddressToKey(IReadOnlyList<byte> address) =>
        $"{address[0]:X2}-{address[1]:X2}-{address[2]:X2}-{address[3]:X2}";

    private static int DecodeDelayTime(IReadOnlyList<byte> data)
    {
        if (data.Count != 2) throw new ArgumentException("Delay time data must contain exactly 2 bytes.", nameof(data));
        return (data[0] & 0x0F) << 7 | data[1] & 0x7F;
    }
}
