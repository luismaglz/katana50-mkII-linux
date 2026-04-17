using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using Kataka.App.KatanaState;
using Kataka.Application.Katana;
using Kataka.Domain.Midi;

using Microsoft.Extensions.Logging;

namespace Kataka.App.Services;

public sealed class AmpSyncService : IAmpSyncService
{
    private static readonly TimeSpan WriteSyncDebounce = TimeSpan.FromMilliseconds(125);

    private readonly Dictionary<string, (IReadOnlyList<byte> Address, byte Value)> _pendingWrites = [];
    private readonly IKatanaSession _session;
    private readonly IKatanaState _katanaState;
    private readonly SemaphoreSlim _syncGate = new(1, 1);
    private readonly DispatcherTimer _writeSyncTimer;
    private readonly ILogger<AmpSyncService> _logger;

    private bool _suppressWrites;
    private bool _stateWritesSubscribed;

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

        _writeSyncTimer = new DispatcherTimer { Interval = WriteSyncDebounce };
        _writeSyncTimer.Tick += async (_, _) =>
        {
            _writeSyncTimer.Stop();
            await FlushPendingWritesAsync();
        };
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

    public void Shutdown() => _writeSyncTimer.Stop();

    public void OnConnectionChanged(bool connected)
    {
        if (!connected)
        {
            if (_pendingWrites.Count > 0)
                _logger.LogInformation("Clearing {Count} pending writes after disconnect.", _pendingWrites.Count);
            _pendingWrites.Clear();
        }

        UpdateWriteSyncTimer();
    }

    // ── Timer control ─────────────────────────────────────────────────────────

    public void UpdateWriteSyncTimer()
    {
        _writeSyncTimer.Stop();
        if (_session.IsConnected && _pendingWrites.Count > 0)
            _writeSyncTimer.Start();
    }

    // ── State queries ─────────────────────────────────────────────────────────

    public bool HasPendingWrites() => _pendingWrites.Count > 0;

    public string DescribePendingWrites() =>
        _pendingWrites.Count == 0 ? "no pending changes" : $"{_pendingWrites.Count} writes";

    public void ClearPendingWrites() => _pendingWrites.Clear();

    // ── Read operations ───────────────────────────────────────────────────────

    public async Task<bool> TryRefreshAmpStateAsync()
    {
        try
        {
            _logger.LogInformation("Seeding KatanaState from full patch read.");
            _suppressWrites = true;
            try
            {
                var allStates = await _session.ReadAllPatchStatesAsync();
                _katanaState.SetStates(allStates);
            }
            finally
            {
                _suppressWrites = false;
            }

            SubscribeToStateWrites();
            _logger.LogInformation("KatanaState seeded. Write-back subscriptions active.");
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
        if (_suppressWrites || !_session.IsConnected) return;
        _pendingWrites[state.Parameter.AddressString] = (state.Parameter.Address, (byte)state.Value);
        UpdateWriteSyncTimer();
    }

    private async Task FlushPendingWritesAsync()
    {
        if (_pendingWrites.Count == 0) return;
        if (!await _syncGate.WaitAsync(0)) return;
        try
        {
            var snapshot = _pendingWrites.ToList();
            _pendingWrites.Clear();
            foreach (var (_, (address, value)) in snapshot)
            {
                _logger.LogInformation("Writing {Address} = {Value}",
                    string.Join("-", address.Select(b => b.ToString("X2"))), value);
                await _session.WriteBlockAsync(address, [value]);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Flush pending writes failed.");
        }
        finally
        {
            _syncGate.Release();
        }
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
