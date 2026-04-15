using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia.Threading;
using Kataka.Application.Katana;

namespace Kataka.App.Services;

public class AmpSyncService : IAmpSyncService
{
    // ── Timing constants ──────────────────────────────────────────────────────
    private static readonly TimeSpan WriteSyncDebounce = TimeSpan.FromMilliseconds(125);
    private static readonly TimeSpan ReadSyncInterval  = TimeSpan.FromSeconds(2);
    
    // ── Sync infrastructure ───────────────────────────────────────────────────
    private readonly SemaphoreSlim _syncGate = new(1, 1);
    private readonly DispatcherTimer _writeSyncTimer;
    private readonly DispatcherTimer _readSyncTimer;
    
    // ── Push notification routing ─────────────────────────────────────────────
    private Dictionary<string, Action<byte>> _pushHandlerLookup = [];
    
    // ── Guard flags ───────────────────────────────────────────────────────────
    private bool _suppressChangeTracking;
    private bool _suspendActiveReadSync;
    private int  _activeReadSyncPhase;
    private int  _flushRetryCount;
    private bool _isShuttingDown;
    private readonly IKatanaSession _session;

    public AmpReadWriteService(IKatanaSession katanaSession)
    {
        _session = katanaSession;

        _writeSyncTimer = new DispatcherTimer { Interval = WriteSyncDebounce };
        _writeSyncTimer.Tick += async (_, _) =>
        {
            _writeSyncTimer.Stop();
            await FlushPendingWritesAsync();
            if (HasPendingWrites()) UpdateWriteSyncTimer();
        };

        _readSyncTimer = new DispatcherTimer { Interval = ReadSyncInterval };
        _readSyncTimer.Tick += async (_, _) =>
        {
            _readSyncTimer.Stop();
            await RunActiveReadSyncCycleAsync();
            if (ShouldRunActiveReadSync()) UpdateReadSyncTimer();
        };
    }
    
    public async Task OpenAsync(string inputPortId, string outputPortId)
    {
        await _session.ConnectAsync(inputPortId, outputPortId);
        BuildPushHandlerLookup();
        _session.PushNotificationReceived += OnAmpPushNotification;
        _session.PanelChannelChanged      += OnAmpPanelChannelChanged;
    }

    public async Task CloseAsync()
    {
        _session.PushNotificationReceived -= OnAmpPushNotification;
        _session.PanelChannelChanged      -= OnAmpPanelChannelChanged;
        _pushHandlerLookup.Clear();
        await _session.DisconnectAsync();
    }
    
    
    public void Connect(
        ObservableCollection<AmpControlViewModel> ampControls,
        ObservableCollection<IBasePedal> panelEffects,
        PedalFxViewModel pedalFx,
        PedalboardViewModel pedalboard,
        Action<string> setSelectedAmpType,
        Action<string> setSelectedCabinetResonance,
        Action<bool> setIsAmpVariation,
        Action<string> setSelectedPanelChannel,
        Action<int?> setDelayTimeMs,
        Action<int> setPatchLevel)
    {
        _ampControls  = ampControls;
        _panelEffects = panelEffects;
        _pedalFx      = pedalFx;
        _pedalboard   = pedalboard;

        _setSelectedAmpType          = setSelectedAmpType;
        _setSelectedCabinetResonance = setSelectedCabinetResonance;
        _setIsAmpVariation           = setIsAmpVariation;
        _setSelectedPanelChannel     = setSelectedPanelChannel;
        _setDelayTimeMs              = setDelayTimeMs;
        _setPatchLevel               = setPatchLevel;

        _ampControlsByKey.Clear();
        _panelEffectsByKey.Clear();

        foreach (var control in ampControls)
            _ampControlsByKey[control.Parameter.Key] = control;

        foreach (var effect in panelEffects)
            _panelEffectsByKey[effect.Definition.SwitchParameter.Key] = effect;

        UpdateWriteSyncTimer();
        UpdateReadSyncTimer();
    }
    
    public void Shutdown()
    {
        _isShuttingDown = true;
        _writeSyncTimer.Stop();
        _readSyncTimer.Stop();
    }

}