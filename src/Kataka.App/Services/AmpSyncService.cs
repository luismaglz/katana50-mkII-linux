using System.Reactive.Subjects;
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
        try
        {
            _logger.LogInformation("Seeding KatanaState from full patch read.");
            var allStates = await _session.ReadAllPatchStatesAsync();
            _katanaState.SetStates(allStates);

            var channel = await _session.ReadCurrentPanelChannelAsync();
            if (channel.HasValue)
                _katanaState.SelectedChannel = channel.Value;

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
    }

    /// <summary>
    ///     Select the Amp Channel
    /// </summary>
    /// <param name="channel"></param>
    public async Task SelectChannelAsync(KatanaPanelChannel channel)
    {
        await _session.SelectPanelChannelAsync(channel);
        _katanaState.SelectedChannel = channel;
        await TryRefreshAmpStateAsync();
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
        _writeChannel?.Writer.TryWrite((state.Parameter.Address, (byte)state.Value));
    }

    /// <summary> Push notification infrastructure ───────────────────────────────────── </summary>
    private void OnAmpPushNotification(object? sender, SysExMessage message)
    {
        var bytes = message.Bytes;
        _logger.LogDebug("Received push notification: {Bytes}", string.Join(" ", bytes.Select(b => b.ToString("X2"))));

        if (bytes[7] != 0x12) return;
        if (bytes.Count != 15 && bytes.Count != 16) return;

        var addressKey = Utilities.AddressToKey([bytes[8], bytes[9], bytes[10], bytes[11]]);
        var value = bytes.Count == 16 ? bytes[13] : bytes[12];
        _logger.LogDebug("Received {Value} on {Address}", value, addressKey);

        if (addressKey == Utilities.AddressToKey(KatanaMkIIParameterCatalog.CurrentChannelAddress))
        {
            OnChannelChangePush(value);
            return;
        }

        _katanaState.SetState(addressKey, value);
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
        _ = RefreshOnChannelChangeAsync(channel.Value);
    }

    private void OnAmpPanelChannelChanged(object? sender, KatanaPanelChannel channel)
    {
        _logger.LogInformation("Amp channel changed (push): {Channel}", channel.ToString());
        _ = RefreshOnChannelChangeAsync(channel);
    }

    private async Task RefreshOnChannelChangeAsync(KatanaPanelChannel channel)
    {
        _katanaState.SelectedChannel = channel;
        await TryRefreshAmpStateAsync();
    }
}
