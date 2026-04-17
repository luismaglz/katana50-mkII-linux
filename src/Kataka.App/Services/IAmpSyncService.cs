using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kataka.App.Services;

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

    // /// <summary>
    // ///     Wires up domain-state write-tracking subscriptions.
    // ///     Call once after the VM's collections are fully populated.
    // /// </summary>
    // void Initialize(IAmpSyncState state);

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
    // void TrackPanelChannelChange(string displayName);
    void UpdateWriteSyncTimer();
    bool HasPendingWrites();
    string DescribePendingWrites();
    void ClearPendingWrites();

    Task<bool> TryRefreshAmpStateAsync();
    // Task<bool> TryWritePatchLevelAsync();
}
