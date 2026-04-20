using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Channels;

using Kataka.App.KatanaState;
using Kataka.Application.Katana;
using Kataka.Domain.Midi;

using Microsoft.Extensions.Logging;

namespace Kataka.App.Services;

public sealed class AmpSyncService : IAmpSyncService
{
    private readonly IKatanaState _katanaState;
    private readonly ILogger<AmpSyncService> _logger;

    /// <summary> Observable subjects ─────────────────────────────────────────────────── </summary>
    private readonly Subject<string> _panelChannelSubject = new();

    private readonly Subject<DeviceReadMetadata> _readCompletedSubject = new();
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private readonly IKatanaSession _session;
    private readonly Subject<string> _statusSubject = new();

    private bool _stateWritesSubscribed;

    private Channel<(IReadOnlyList<byte> Address, byte Value)>? _writeChannel;
    private CancellationTokenSource? _writeCts;

    /// <summary> Construction ────────────────────────────────────────────────────────── </summary>
    public AmpSyncService(IKatanaSession session, IKatanaState katanaState, ILogger<AmpSyncService> logger)
    {
        _session = session;
        _katanaState = katanaState;
        _logger = logger;
    }

    public IObservable<string> PanelChannelPushed => _panelChannelSubject;
    public IObservable<DeviceReadMetadata> ReadCompleted => _readCompletedSubject;
    public IObservable<string> StatusMessages => _statusSubject;

    /// <summary> Lifecycle ───────────────────────────────────────────────────────────── </summary>
    public void Activate()
    {
        _session.PushNotificationReceived += OnAmpPushNotification;
        _session.PanelChannelChanged += OnAmpPanelChannelChanged;
    }

    public void Deactivate()
    {
        _session.PushNotificationReceived -= OnAmpPushNotification;
        _session.PanelChannelChanged -= OnAmpPanelChannelChanged;
    }

    public void Shutdown() => StopWriteLoop();

    public void OnConnectionChanged(bool connected)
    {
        if (!connected)
            StopWriteLoop();
    }

    /// <summary> Read operations ─────────────────────────────────────────────────────── </summary>
    public async Task<bool> TryRefreshAmpStateAsync()
    {
        if (!await _refreshLock.WaitAsync(0))
        {
            _logger.LogDebug("Refresh already in progress, skipping concurrent request.");
            return false;
        }

        try
        {
            _logger.LogInformation("Seeding KatanaState from full patch read.");
            var allStates = await _session.ReadAllPatchStatesAsync();
            _katanaState.SetStates(allStates);

            var channel = await _session.ReadCurrentPanelChannelAsync();
            if (channel.HasValue)
                _katanaState.SelectedChannel = channel.Value;

            // Also read System-level Global EQ parameters (not included in ReadAllPatchStatesAsync)
            try
            {
                var catalogType = typeof(KatanaMkIIParameterCatalog);
                var defs = catalogType.GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(p => p.Name.StartsWith("GlobalEq"))
                    .Select(p => p.GetValue(null))
                    .OfType<KatanaParameterDefinition>()
                    .ToList();

                if (defs.Count > 0)
                {
                    _logger.LogInformation("Reading {Count} Global EQ parameters from amp.", defs.Count);
                    var sysValues = await _session.ReadParametersAsync(defs);
                    // ReadParametersAsync keys by parameter.Key (kebab-case), but _stateFields uses
                    // parameter.AddressString (hex). Remap before calling SetStates.
                    var remapped = defs
                        .Where(d => sysValues.ContainsKey(d.Key))
                        .ToDictionary(d => d.AddressString, d => sysValues[d.Key]);
                    _katanaState.SetStates(remapped);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read Global EQ parameters during seed.");
            }

            SubscribeToStateWrites();
            StartWriteLoop();
            _logger.LogInformation("KatanaState seeded. Write loop started.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Full patch state read failed.");
            return false;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    /// <summary>
    ///     Select the Amp Channel
    /// </summary>
    /// <param name="channel"></param>
    public async Task SelectChannelAsync(KatanaPanelChannel channel)
    {
        await _session.SelectPanelChannelAsync(channel);
        // The amp will push back the full patch block and a channel-change notification;
        // OnAmpPushNotification and OnChannelChangePush update state without a re-read.
    }

    /// <summary> Write loop ──────────────────────────────────────────────────────────── </summary>
    private void StartWriteLoop()
    {
        StopWriteLoop();

        _writeChannel = Channel.CreateUnbounded<(IReadOnlyList<byte>, byte)>(
            new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
        _writeCts = new CancellationTokenSource();

        _ = RunWriteLoopAsync(_writeChannel, _writeCts.Token);
    }

    private void StopWriteLoop()
    {
        _writeCts?.Cancel();
        _writeCts?.Dispose();
        _writeCts = null;
        _writeChannel = null; // stale items abandoned and GC'd
    }

    private async Task RunWriteLoopAsync(
        Channel<(IReadOnlyList<byte> Address, byte Value)> channel,
        CancellationToken ct)
    {
        var reader = channel.Reader;
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(20));
        try
        {
            while (await timer.WaitForNextTickAsync(ct))
            {
                if (!reader.TryRead(out var msg)) continue;
                try
                {
                    await _session.WriteBlockAsync(msg.Address, [msg.Value]);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogWarning(ex, "Write failed for {Address}.",
                        string.Join("-", msg.Address.Select(b => b.ToString("X2"))));
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    /// <summary> State write-back ────────────────────────────────────────────────────── </summary>
    private void SubscribeToStateWrites()
    {
        if (_stateWritesSubscribed) return;
        _stateWritesSubscribed = true;

        foreach (var (_, state) in _katanaState.GetAllRegisteredStates())
        {
            var captured = state;
            captured.WriteRequested += () => OnDomainStateChanged(captured);
        }
    }

    private void OnDomainStateChanged(AmpControlState state)
    {
        if (!_session.IsConnected) return;
        _logger.LogDebug("Domain state change: {Address} -> {Value}", state.Parameter.AddressString, state.Value);
        _writeChannel?.Writer.TryWrite((state.Parameter.Address, (byte)state.Value));
    }

    /// <summary> Push notification infrastructure ───────────────────────────────────── </summary>
    private void OnAmpPushNotification(object? sender, SysExMessage message)
    {
        var bytes = message.Bytes;
        _logger.LogDebug("Received push notification: {Bytes}", string.Join(" ", bytes.Select(b => b.ToString("X2"))));

        // Must be a DT1 (DataSet1) message with at least 1 data byte.
        // Layout: F0 41 00 00 00 00 33 12 [addr×4] [data×N] [checksum] F7  (total = 14 + N)
        if (bytes[7] != 0x12) return;
        if (bytes.Count < 15) return;

        var address = new byte[] { bytes[8], bytes[9], bytes[10], bytes[11] };
        var dataLength = bytes.Count - 14; // N bytes between address and checksum

        if (dataLength == 1)
        {
            var addressKey = Utilities.AddressToKey(address);
            var value = bytes[12];
            _logger.LogDebug("Received {Value} on {Address}", value, addressKey);

            if (addressKey == Utilities.AddressToKey(KatanaMkIIParameterCatalog.CurrentChannelAddress))
            {
                OnChannelChangePush(value);
                return;
            }

            _katanaState.SetState(addressKey, value);
        }
        else
        {
            // Multi-byte block push (e.g. full patch dump on channel change).
            // Expand each byte to its individual address and update state.
            _logger.LogDebug("Received block push: {Length} bytes at {Address}", dataLength,
                Utilities.AddressToKey(address));
            for (var i = 0; i < dataLength; i++)
            {
                var byteAddr = Utilities.AddressOffset(address, i);
                var key = Utilities.AddressToKey(byteAddr);

                _katanaState.SetState(key, bytes[12 + i]);
            }
        }
    }

    private void OnChannelChangePush(byte channelCode)
    {
        KatanaPanelChannel? channel = channelCode switch
        {
            0 => KatanaPanelChannel.Panel,
            1 => KatanaPanelChannel.ChA1,
            2 => KatanaPanelChannel.ChA2,
            5 => KatanaPanelChannel.ChB1,
            6 => KatanaPanelChannel.ChB2,
            _ => null
        };

        if (channel is null)
        {
            _logger.LogWarning("Unknown channel code in SysEx push: {Code:X2}", channelCode);
            return;
        }

        _logger.LogInformation("Amp channel changed (SysEx): {Channel}", channel.ToString());
        _katanaState.SelectedChannel = channel.Value;
    }

    private void OnAmpPanelChannelChanged(object? sender, KatanaPanelChannel channel)
    {
        _logger.LogInformation("Amp channel changed (Program Change): {Channel}", channel.ToString());
        _katanaState.SelectedChannel = channel;
    }
}
