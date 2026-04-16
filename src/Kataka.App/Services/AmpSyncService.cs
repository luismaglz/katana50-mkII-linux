using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using Kataka.App.ViewModels;
using Kataka.Application.Katana;
using Kataka.Domain.KatanaState;
using Kataka.Domain.Midi;

namespace Kataka.App.Services;

public interface IAmpSyncService
{
    /// <summary>
    ///     Wires up change-tracking subscriptions to the VM's collections and starts the write timer.
    ///     Call after AmpControls, PanelEffects, PedalFx, and Pedalboard are fully populated.
    /// </summary>
    void Initialize(IAmpSyncContext context);

    /// <summary>Builds the push-notification lookup and subscribes to amp events. Call after connect.</summary>
    void Activate();

    /// <summary>Unsubscribes from amp events. Call before disconnect.</summary>
    void Deactivate();

    /// <summary>Stops the write sync timer immediately. Call from the window Closing handler.</summary>
    void Shutdown();

    /// <summary>Clears the pending write queue and updates the write timer. Call on IsConnected change.</summary>
    void OnConnectionChanged(bool connected);

    /// <summary>
    ///     Queues a single key→value write and kicks the debounce timer.
    ///     Used by the VM's partial void OnXxx handlers for standalone panel parameters
    ///     (AmpType, CabinetResonance, AmpVariation, ChainPattern).
    /// </summary>
    void QueueWrite(string key, byte value, string logMessage);

    void TrackPanelChannelChange(string displayName);
    void UpdateWriteSyncTimer();
    bool HasPendingWrites();
    string DescribePendingWrites();
    void ClearPendingWrites();

    Task<bool> TryRefreshAmpStateAsync();

    // Task<bool> TryReadAmpControlsAsync();
    // Task<bool> TryReadPanelControlsAsync();
    // Task<bool> TryReadPedalControlsAsync();
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
    private IAmpSyncContext _context = null!;
    private Dictionary<string, AmpControlState> _domainAmpControlsByKey = [];
    private Dictionary<string, AmpControlState> _domainPanelStatesByKey = [];

    private int _flushRetryCount;
    private bool _isShuttingDown;
    private string? _pendingPanelChannel;
    private Dictionary<string, Action<byte>> _pushHandlerLookup = [];

    // ── Construction ──────────────────────────────────────────────────────────

    public AmpSyncService(IKatanaSession session, IKatanaState state)
    {
        _session = session;
        _state = state;

        _writeSyncTimer = new DispatcherTimer
        {
            Interval = WriteSyncDebounce
        };
        _writeSyncTimer.Tick += async (_, _) =>
        {
            _writeSyncTimer.Stop();
            await FlushPendingWritesAsync();
            if (HasPendingWrites()) UpdateWriteSyncTimer();
        };
    }

    /// <summary>
    ///     Wires up change-tracking subscriptions to the VM's collections and starts the write timer.
    ///     Call after AmpControls, PanelEffects, PedalFx, and Pedalboard are fully populated.
    /// </summary>
    public void Initialize(IAmpSyncContext context)
    {
        _context = context;

        _domainAmpControlsByKey = new Dictionary<string, AmpControlState>(_state.GetAmpControlsByKey());
        _domainPanelStatesByKey = new Dictionary<string, AmpControlState>(_state.GetPedalDomainControlsByKey());

        // Subscribe to ALL domain state changes: fires the pending-write queue.
        // The guard inside ensures we only queue writes for user-initiated changes.
        SubscribeDomainWriteTracking(_domainAmpControlsByKey.Values);
        SubscribeDomainWriteTracking(_domainPanelStatesByKey.Values);

        _ampControlsByKey.Clear();
        foreach (var control in context.AmpControls)
        {
            _ampControlsByKey[control.Parameter.Key] = control;
            // No VM PropertyChanged subscription — domain ValueChanged drives writes now.
        }

        _panelEffectsByKey.Clear();
        foreach (var effect in context.PanelEffects)
        {
            _panelEffectsByKey[effect.Definition.SwitchParameter.Key] = effect;

            // Domain-migrated VMs (Booster, Delay, Reverb) write through domain state.
            // Only ModFxPedalViewModel still uses the legacy VM-event path.
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

        context.Pedalboard.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(PedalboardViewModel.SelectedChainPattern)) return;
            if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
            var value = _context.Pedalboard.SelectedChainPattern;
            if (value < 0 || value >= PedalboardViewModel.ChainPatternNames.Length) return;
            _pendingWrites[KatanaMkIIParameterCatalog.ChainPattern.Key] = (byte)value;
            _context.Log($"Queued panel sync: Chain Pattern -> {PedalboardViewModel.ChainPatternNames[value]}.");
            UpdateWriteSyncTimer();
        };

        context.PedalFx.PropertyChanged += (_, args) => TrackPedalChange(args.PropertyName);

        UpdateWriteSyncTimer();
    }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    /// <summary>Builds the push-notification lookup and subscribes to amp events. Call after connect.</summary>
    public void Activate()
    {
        BuildPushHandlerLookup();
        _session.PushNotificationReceived += OnAmpPushNotification;
        _session.PanelChannelChanged += OnAmpPanelChannelChanged;
    }

    /// <summary>Unsubscribes from amp events. Call before disconnect.</summary>
    public void Deactivate()
    {
        _session.PushNotificationReceived -= OnAmpPushNotification;
        _session.PanelChannelChanged -= OnAmpPanelChannelChanged;
        _pushHandlerLookup.Clear();
    }

    /// <summary>Stops the write sync timer immediately. Call from the window Closing handler.</summary>
    public void Shutdown()
    {
        _isShuttingDown = true;
        _writeSyncTimer.Stop();
    }

    /// <summary>Clears the pending write queue and updates the write timer. Call on IsConnected change.</summary>
    public void OnConnectionChanged(bool connected)
    {
        if (!connected)
        {
            if (_pendingWrites.Count > 0 || _pendingPanelChannel is not null) _context.Log($"Clearing pending sync queue after disconnect: {DescribePendingWrites()}.");

            _pendingWrites.Clear();
            _pendingPanelChannel = null;
        }

        UpdateWriteSyncTimer();
    }

    // ── Change tracking ───────────────────────────────────────────────────────

    /// <summary>
    ///     Queues a single key→value write and kicks the debounce timer.
    ///     Used by the VM's partial void OnXxx handlers for standalone panel parameters
    ///     (AmpType, CabinetResonance, AmpVariation, ChainPattern).
    /// </summary>
    public void QueueWrite(string key, byte value, string logMessage)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        _pendingWrites[key] = value;
        _context.Log($"Queued panel sync: {logMessage}.");
        UpdateWriteSyncTimer();
    }

    public void TrackPanelChannelChange(string displayName)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        _pendingPanelChannel = displayName;
        _context.Log($"Queued panel channel sync: {displayName}.");
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

    public async Task<bool> TryRefreshAmpStateAsync()
    {
        await TryReadAmpControlsAsync();
        await TryReadPanelControlsAsync();
        await TryReadPedalControlsAsync();
        return true;
    }

// ── Public read operations ────────────────────────────────────────────────

    public async Task<bool> TryReadAmpControlsAsync()
    {
        try
        {
            _context.Log("Reading Katana amp editor controls.");
            var values = await _session.ReadParametersAsync(_context.AmpControls.Select(c => c.Parameter).ToArray());
            ApplyDeviceState(() =>
            {
                foreach (var control in _context.AmpControls)
                {
                    var value = values[control.Parameter.Key];
                    // Write to domain state only — VM observes via ValueChanged subscription.
                    if (_domainAmpControlsByKey.TryGetValue(control.Parameter.Key, out var domainControl))
                        domainControl.Value = value;
                    _context.Log($"{control.DisplayName} reply: {value}");
                }
            });

            _context.StatusMessage = "Amp editor controls read successfully.";
            _context.AmpEditorStatus = "Amp editor values were loaded from the Katana.";
            return true;
        }
        catch (Exception ex)
        {
            _context.AmpEditorStatus = "Amp editor read failed.";
            _context.StatusMessage = ex.Message;
            _context.Log("Amp editor read failed.");
            _context.Log(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> TryReadPanelControlsAsync()
    {
        try
        {
            _context.Log("Reading Katana panel controls.");

            var currentChannel = await _session.ReadCurrentPanelChannelAsync();
            if (currentChannel is not null)
                ApplyDeviceState(() =>
                {
                    _context.SelectedPanelChannel = IAmpSyncContext.ToPanelChannelDisplay(currentChannel.Value);
                    _context.Log($"Current panel channel: {_context.SelectedPanelChannel}");
                });

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
                // Write amp/panel param values to domain state.
                // Domain-migrated VMs (Booster, Delay, Reverb) observe via ValueChanged — no direct VM call needed.
                // AmpType, CabinetResonance, AmpVariation are in _domainAmpControlsByKey and also update
                // the MainWindowViewModel string properties via their ValueChanged subscriptions.
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
                {
                    effect.ApplyAmpValues(intValues);
                }

                // ChainPattern still drives Pedalboard directly (Pedalboard not yet domain-migrated).
                if (values.TryGetValue(KatanaMkIIParameterCatalog.ChainPattern.Key, out var chainValue) &&
                    chainValue < PedalboardViewModel.ChainPatternNames.Length)
                    _context.Pedalboard.SelectedChainPattern = chainValue;

                foreach (var effect in _context.PanelEffects)
                {
                    _context.Log($"{effect.DisplayName}: {(effect.IsEnabled ? "On" : "Off")} / {effect.Variation} / {effect.TypeCaption}");
                }

                _context.Log(
                    $"Amp Type: {_context.SelectedAmpType} ({(_context.IsAmpVariation ? "TYPE 2" : "TYPE 1")}) / " +
                    $"Cabinet Resonance: {_context.SelectedCabinetResonance} / " +
                    $"Chain: {PedalboardViewModel.ChainPatternNames[_context.Pedalboard.SelectedChainPattern]}");
            });

            _context.ResetDelayTap();
            var patchLevelLoaded = await TryRefreshPatchLevelAsync();
            var delayTimeLoaded = await TryRefreshDelayTimeAsync();

            _context.StatusMessage = "Panel controls read successfully.";
            _context.PanelControlsStatus = (patchLevelLoaded, delayTimeLoaded) switch
            {
                (true, true) => "Panel channel, patch level, effect toggles, variation colors, effect types, and delay time were loaded.",
                (true, false) => "Panel channel, patch level, effect toggles, variation colors, and effect types were loaded. Delay time refresh failed.",
                (false, true) => "Panel channel, effect toggles, variation colors, effect types, and delay time were loaded. Patch level mapping is still pending.",
                _ => "Panel channel, effect toggles, variation colors, and effect types were loaded. Patch level mapping is still pending, and delay time refresh failed."
            };

            return true;
        }
        catch (Exception ex)
        {
            _context.PanelControlsStatus = "Panel control read failed.";
            _context.StatusMessage = ex.Message;
            _context.Log("Panel control read failed.");
            _context.Log(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> TryReadPedalControlsAsync()
    {
        try
        {
            _context.Log("Reading Katana pedal controls.");
            var values = await _session.ReadParametersAsync(_context.PedalFx.GetReadParameters().ToArray());

            ApplyDeviceState(() =>
            {
                _context.PedalFx.ApplyValues(values);
                _context.Log(
                    $"Pedal FX: {(_context.PedalFx.IsEnabled ? "On" : "Off")} / {_context.PedalFx.SelectedTypeOption} / {_context.PedalFx.SelectedPositionOption} / Foot Volume {_context.PedalFx.FootVolume}");
                if (_context.PedalFx.IsWahMode)
                    _context.Log(
                        $"Pedal Wah: {_context.PedalFx.SelectedWahTypeOption} / Pos {_context.PedalFx.PedalPosition} / Min {_context.PedalFx.PedalMinimum} / Max {_context.PedalFx.PedalMaximum} / Level {_context.PedalFx.EffectLevel} / Direct {_context.PedalFx.DirectMix}");
            });

            _context.StatusMessage = "Pedal controls read successfully.";
            _context.PedalControlsStatus = _context.PedalFx.IsWahMode
                ? "Pedal FX, wah controls, and foot volume were loaded."
                : "Pedal FX type and foot volume were loaded. Non-wah subtype controls are not surfaced yet.";

            return true;
        }
        catch (Exception ex)
        {
            _context.PedalControlsStatus = "Pedal control read failed.";
            _context.StatusMessage = ex.Message;
            _context.Log("Pedal control read failed.");
            _context.Log(ex.ToString());
            Console.Error.WriteLine(ex);
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
            if (requested != _context.PatchLevel)
            {
                _context.PatchLevel = requested;
                _context.Log($"Clamped Patch Level to {requested}.");
            }

            _context.PatchLevel = await _session.WriteParameterAsync(KatanaMkIIParameterCatalog.PatchLevel, (byte)requested);
            _context.Log($"Patch Level confirmed at {_context.PatchLevel}.");
            return true;
        }
        catch (Exception ex)
        {
            _context.Log("Patch level write failed.");
            _context.Log(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    private void TrackAmpControlChange(AmpControlViewModel control)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        var clampedValue = Math.Clamp(control.Value, control.Minimum, control.Maximum);
        _pendingWrites[control.Parameter.Key] = (byte)clampedValue;
        _context.Log($"Queued amp sync: {control.DisplayName} -> {clampedValue}.");
        UpdateWriteSyncTimer();
    }

    private void TrackPanelEffectChange(IBasePedal effect)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        _pendingWrites[effect.Definition.SwitchParameter.Key] = effect.IsEnabled ? (byte)1 : (byte)0;
        _context.Log($"Queued panel sync: {effect.DisplayName} -> {(effect.IsEnabled ? "On" : "Off")}.");
        UpdateWriteSyncTimer();
    }

    private void TrackPanelEffectTypeChange(IBasePedal effect)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected
            || effect.Definition.TypeParameter is null) return;
        if (!effect.TryGetTypeValue(effect.SelectedTypeOption, out var typeValue)) return;
        _pendingWrites[effect.Definition.TypeParameter.Key] = typeValue;
        _context.Log($"Queued panel type sync: {effect.DisplayName} -> {effect.SelectedTypeOption}.");
        UpdateWriteSyncTimer();
    }

    private void TrackDetailParamChange(string key, int value)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        _pendingWrites[key] = (byte)Math.Clamp(value, 0, 127);
        _context.Log($"Queued detail param sync: {key} -> {value}.");
        UpdateWriteSyncTimer();
    }

    private void TrackPedalChange(string? propertyName)
    {
        if (_context.SuppressChangeTracking || !_context.ActiveWriteSync || !_context.IsConnected) return;
        if (!_context.PedalFx.TryGetParameterValue(propertyName, out var parameter, out var value, out var description)) return;
        _pendingWrites[parameter.Key] = value;
        _context.Log($"Queued pedal sync: {parameter.DisplayName} -> {description}.");
        UpdateWriteSyncTimer();
    }

    /// <summary>
    ///     Subscribes to each domain AmpControlState's ValueChanged event.
    ///     When a value changes and the app is in a write-ready state, the key→value is queued for flush.
    /// </summary>
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
                _context.Log($"Queued sync: {captured.Parameter.DisplayName} -> {value}.");
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
            _context.Log("Queued write sync is already running; the latest changes will wait for the next flush.");
            return;
        }

        var snapshot = new Dictionary<string, byte>(StringComparer.Ordinal);
        string? channelSnapshot = null;

        try
        {
            foreach (var entry in _pendingWrites)
            {
                snapshot[entry.Key] = entry.Value;
            }
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

            _context.Log(
                $"Flushing queued sync: {ampSnapshot.Count} amp, {panelSwitchSnapshot.Count} panel, " +
                $"{panelTypeSnapshot.Count} panel type, {pedalSnapshot.Count} pedal, " +
                $"{detailParamSnapshot.Count} detail, {(channelSnapshot is null ? "no" : "1")} channel change.");

            await FlushPendingAmpWritesAsync(ampSnapshot);
            await FlushPendingPanelWritesAsync(channelSnapshot, panelSwitchSnapshot, panelTypeSnapshot, snapshot);
            await FlushPendingPedalWritesAsync(pedalSnapshot);
            await FlushPendingDetailParamWritesAsync(detailParamSnapshot);

            if (snapshot.Count > 0 || channelSnapshot is not null)
            {
                _context.StatusMessage = "Queued changes synced to the Katana.";
                _context.Log("Queued changes synced to the Katana.");
            }

            _flushRetryCount = 0;
        }
        catch (Exception ex)
        {
            foreach (var entry in snapshot)
            {
                _pendingWrites[entry.Key] = entry.Value;
            }
            _pendingPanelChannel ??= channelSnapshot;
            _flushRetryCount++;
            var backoffMs = Math.Min(100 * (1 << _flushRetryCount), 2000);
            _context.StatusMessage = ex.Message;
            _context.Log($"Queued write sync failed (retry {_flushRetryCount}, backoff {backoffMs}ms). Re-queued {DescribePendingWrites()}.");
            _context.Log(ex.ToString());
            Console.Error.WriteLine(ex);
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
            var group = new List<KatanaParameterDefinition>
            {
                orderedParameters[groupStart]
            };
            var groupIndex = groupStart + 1;
            while (groupIndex < orderedParameters.Count && CanBatchWrite(group[^1], orderedParameters[groupIndex]))
            {
                group.Add(orderedParameters[groupIndex]);
                groupIndex++;
            }
            var startAddress = group[0].Address.ToArray();
            var data = group.Select(p => ampWrites[p.Key]).ToArray();
            _context.Log($"Writing amp sync batch: {DescribeAmpKeys(group.Select(p => p.Key))}.");
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
            _context.Log($"Writing queued panel channel: {channel}.");
            await _session.SelectPanelChannelAsync(IAmpSyncContext.ParsePanelChannelDisplay(channel));
        }

        foreach (var entry in panelSwitchWrites)
        {
            var parameter = _panelEffectsByKey[entry.Key].Definition.SwitchParameter;
            _context.Log($"Writing queued panel effect: {_panelEffectsByKey[entry.Key].DisplayName} -> {(entry.Value != 0 ? "On" : "Off")}.");
            await _session.WriteBlockAsync(parameter.Address, [entry.Value]);
        }

        foreach (var entry in panelTypeWrites)
        {
            var effect = _context.PanelEffects.First(p => p.Definition.TypeParameter?.Key == entry.Key);
            _context.Log($"Writing queued panel type: {effect.DisplayName} -> {effect.ToTypeOption(entry.Value)}.");
            await _session.WriteBlockAsync(effect.Definition.TypeParameter!.Address, [entry.Value]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.AmpType.Key, out var ampTypeValue))
        {
            _context.Log($"Writing queued amp type: {_context.AmpTypeOptions.ElementAtOrDefault(ampTypeValue) ?? ampTypeValue.ToString()}.");
            await _session.WriteBlockAsync(KatanaMkIIParameterCatalog.AmpType.Address, [ampTypeValue]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.CabinetResonance.Key, out var cabinetValue))
        {
            _context.Log($"Writing queued cabinet resonance: {_context.CabinetResonanceOptions.ElementAtOrDefault(cabinetValue) ?? cabinetValue.ToString()}.");
            await _session.WriteBlockAsync(KatanaMkIIParameterCatalog.CabinetResonance.Address, [cabinetValue]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.AmpVariation.Key, out var ampVariationValue))
        {
            _context.Log($"Writing queued amp variation: {(ampVariationValue == 0 ? "TYPE 1" : "TYPE 2")}.");
            await _session.WriteBlockAsync(KatanaMkIIParameterCatalog.AmpVariation.Address, [ampVariationValue]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.ChainPattern.Key, out var chainPatternValue))
        {
            var name = chainPatternValue < PedalboardViewModel.ChainPatternNames.Length
                ? PedalboardViewModel.ChainPatternNames[chainPatternValue]
                : chainPatternValue.ToString();
            _context.Log($"Writing queued chain pattern: {name}.");
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
            var group = new List<KatanaParameterDefinition>
            {
                orderedParameters[groupStart]
            };
            var groupIndex = groupStart + 1;
            while (groupIndex < orderedParameters.Count && CanBatchWrite(group[^1], orderedParameters[groupIndex]))
            {
                group.Add(orderedParameters[groupIndex]);
                groupIndex++;
            }
            var startAddress = group[0].Address.ToArray();
            var data = group.Select(p => pedalWrites[p.Key]).ToArray();
            _context.Log($"Writing pedal sync batch: {DescribePedalKeys(group.Select(p => p.Key))}.");
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
            _context.Log($"Writing detail param: {parameter.DisplayName} -> {value}.");
            await _session.WriteBlockAsync(parameter.Address.ToArray(), [value]);
        }
    }

    private async Task<bool> TryRefreshPatchLevelAsync()
    {
        if (!_context.PatchLevelMappingVerified) return false;
        try
        {
            _context.PatchLevel = await _session.ReadParameterAsync(KatanaMkIIParameterCatalog.PatchLevel);
            _context.Log($"Patch Level reply: {_context.PatchLevel}");
            return true;
        }
        catch (Exception ex)
        {
            _context.Log("Patch level read failed.");
            _context.Log(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    private async Task<bool> TryRefreshDelayTimeAsync()
    {
        try
        {
            var data = await _session.ReadBlockAsync(KatanaMkIIParameterCatalog.DelayTimeAddress, 2);
            var delayTime = DecodeDelayTime(data);
            if (delayTime <= 0)
            {
                _context.DelayTimeMs = null;
                _context.DelayTapStatus = "Delay time is not currently readable for the active effect state.";
                _context.Log("Delay time reply did not contain a usable millisecond value.");
                return false;
            }

            _context.DelayTimeMs = delayTime;
            _context.DelayTapStatus = $"Delay time loaded: {delayTime} ms.";
            _context.Log($"Delay time reply: {delayTime} ms.");
            return true;
        }
        catch (Exception ex)
        {
            _context.DelayTimeMs = null;
            _context.DelayTapStatus = "Delay time refresh failed.";
            _context.Log("Delay time read failed.");
            _context.Log(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    // ── Push notification infrastructure ─────────────────────────────────────

    private void BuildPushHandlerLookup()
    {
        _pushHandlerLookup = new Dictionary<string, Action<byte>>(StringComparer.Ordinal);

        // Amp controls: write to domain only — VMs observe via ValueChanged.
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
                        {
                            [capturedKey] = value
                        });
                };
            }
        }

        // AmpType / CabinetResonance / AmpVariation push — write to domain; MainWindowViewModel
        // subscriptions update the string properties automatically.
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

        // Panel channel — PATCH NUM: address 00-01-00-00, INTEGER2x7 (16-byte DT1)
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

            _context.Log($"Amp channel changed (push): {displayName}");
            ApplyDeviceState(() => _context.SelectedPanelChannel = displayName);
            // Re-read all state after the amp finishes its internal channel switch.
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

        // DT1: F0 41 DevID Model(4) 0x12 Addr(4) Data(N) Chksum F7
        // N=1 → 15 bytes; N=2 → 16 bytes (INTEGER2x7)
        if (bytes.Count != 15 && bytes.Count != 16) return;

        var addressKey = AddressToKey([bytes[8], bytes[9], bytes[10], bytes[11]]);
        var value = bytes.Count == 16 ? bytes[13] : bytes[12];

        if (!_pushHandlerLookup.TryGetValue(addressKey, out var action)) return;

        Dispatcher.UIThread.Post(() => ApplyDeviceState(() => action(value)));
    }

    private void OnAmpPanelChannelChanged(object? sender, KatanaPanelChannel channel)
    {
        var displayName = IAmpSyncContext.ToPanelChannelDisplay(channel);
        Dispatcher.UIThread.Post(() =>
        {
            _context.Log($"Amp channel changed (push): {displayName}");
            ApplyDeviceState(() => _context.SelectedPanelChannel = displayName);
        });
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void ApplyDeviceState(Action apply)
    {
        _context.SuppressChangeTracking = true;
        try
        {
            apply();
        }
        finally
        {
            _context.SuppressChangeTracking = false;
        }
    }

    private string DescribeAmpKeys(IEnumerable<string> keys) =>
        string.Join(", ", keys.Distinct(StringComparer.Ordinal).Select(k => _ampControlsByKey[k].DisplayName));

    private string DescribePanelKeys(IEnumerable<string> keys) =>
        string.Join(", ", keys.Distinct(StringComparer.Ordinal).Select(k => _panelEffectsByKey[k].DisplayName));

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
        if (data.Count != 2)
            throw new ArgumentException("Delay time data must contain exactly 2 bytes.", nameof(data));
        return (data[0] & 0x0F) << 7 | data[1] & 0x7F;
    }
}
