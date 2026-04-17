using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Avalonia.Threading;

using Kataka.App.KatanaState;
using Kataka.Application.Katana;
using Kataka.Domain.Midi;

using Microsoft.Extensions.Logging;

namespace Kataka.App.Services;

public sealed class AmpSyncService : IAmpSyncService
{
    private readonly IKatanaSession _session;
    private readonly IKatanaState _katanaState;
    private readonly ILogger<AmpSyncService> _logger;

    private bool _stateWritesSubscribed;

    private Channel<(IReadOnlyList<byte> Address, byte Value)>? _writeChannel;
    private CancellationTokenSource? _writeCts;

    // ── Observable subjects ───────────────────────────────────────────────────

    private readonly Subject<string> _panelChannelSubject = new();
    private readonly Subject<DeviceReadMetadata> _readCompletedSubject = new();
    private readonly Subject<string> _statusSubject = new();

    public IObservable<string> PanelChannelPushed => _panelChannelSubject;
    public IObservable<DeviceReadMetadata> ReadCompleted => _readCompletedSubject;
    public IObservable<string> StatusMessages => _statusSubject;

    // ── Construction ──────────────────────────────────────────────────────────

    public AmpSyncService(IKatanaSession session, IKatanaState katanaState, ILogger<AmpSyncService> logger)
    {
        _session = session;
        _katanaState = katanaState;
        _logger = logger;
    }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

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

    // ── Write loop ────────────────────────────────────────────────────────────

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
        catch (OperationCanceledException) { }
    }

    // ── Read operations ───────────────────────────────────────────────────────

    public async Task<bool> TryRefreshAmpStateAsync()
    {
        try
        {
            _logger.LogInformation("Seeding KatanaState from full patch read.");
            var allStates = await _session.ReadAllPatchStatesAsync();
            _katanaState.SetStates(allStates);

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

    // ── State write-back ──────────────────────────────────────────────────────

    private void SubscribeToStateWrites()
    {
        if (_stateWritesSubscribed) return;
        _stateWritesSubscribed = true;

        foreach (var (_, state) in _katanaState.GetAllRegisteredStates())
        {
            var captured = state;
            captured.ValueChanged += () => OnDomainStateChanged(captured);
        }
    }

    private void OnDomainStateChanged(AmpControlState state)
    {
        if (!_session.IsConnected) return;
        _writeChannel?.Writer.TryWrite((state.Parameter.Address, (byte)state.Value));
    }

    // ── Push notification infrastructure ─────────────────────────────────────

    private void OnAmpPushNotification(object? sender, SysExMessage message)
    {
        var bytes = message.Bytes;
        if (bytes[7] != 0x12) return;
        if (bytes.Count != 15 && bytes.Count != 16) return;

        var addressKey = Utilities.AddressToKey([bytes[8], bytes[9], bytes[10], bytes[11]]);
        var value = bytes.Count == 16 ? bytes[13] : bytes[12];

        _katanaState.SetState(addressKey, value);
    }

    private void OnAmpPanelChannelChanged(object? sender, KatanaPanelChannel channel)
    {
        var displayName = Utilities.ToPanelChannelDisplay(channel);
        Dispatcher.UIThread.Post(() =>
        {
            _logger.LogInformation("Amp channel changed (push): {Channel}", displayName);
            _panelChannelSubject.OnNext(displayName);
        });
    }
}
