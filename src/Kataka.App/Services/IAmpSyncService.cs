using System;
using System.Threading.Tasks;

namespace Kataka.App.Services;

public interface IAmpSyncService
{
    // ── Observable outputs (device → app) ────────────────────────────────────

    /// <summary>Fires when the amp signals a panel-channel change.</summary>
    IObservable<string> PanelChannelPushed { get; }

    /// <summary>Status/detail metadata after a device read completes (success or failure).</summary>
    IObservable<DeviceReadMetadata> ReadCompleted { get; }

    /// <summary>Human-readable status lines for the VM's StatusMessage property.</summary>
    IObservable<string> StatusMessages { get; }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    /// <summary>Subscribes to amp push events. Call after connect.</summary>
    void Activate();

    /// <summary>Unsubscribes from amp push events. Call before disconnect.</summary>
    void Deactivate();

    /// <summary>Stops the write sync timer. Call from the window Closing handler.</summary>
    void Shutdown();

    /// <summary>Clears the pending write queue and updates the write timer. Call on IsConnected change.</summary>
    void OnConnectionChanged(bool connected);

    // ── Write queue ───────────────────────────────────────────────────────────

    void UpdateWriteSyncTimer();
    bool HasPendingWrites();
    string DescribePendingWrites();
    void ClearPendingWrites();

    // ── Read operations ───────────────────────────────────────────────────────

    /// <summary>Reads the full patch from the amp and seeds KatanaState. Also wires write-back subscriptions.</summary>
    Task<bool> TryRefreshAmpStateAsync();
}
