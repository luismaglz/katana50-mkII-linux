using Kataka.Domain.Midi;

namespace Kataka.App.Services;

public interface IAmpSyncService
{
    /// <summary> Observable outputs (device → app) ──────────────────────────────────── </summary>
    /// <summary>Fires when the amp signals a panel-channel change.</summary>
    IObservable<string> PanelChannelPushed { get; }

    /// <summary>Status/detail metadata after a device read completes (success or failure).</summary>
    IObservable<DeviceReadMetadata> ReadCompleted { get; }

    /// <summary>Human-readable status lines for the VM's StatusMessage property.</summary>
    IObservable<string> StatusMessages { get; }

    /// <summary> Lifecycle ───────────────────────────────────────────────────────────── </summary>
    /// <summary>Subscribes to amp push events. Call after connect.</summary>
    void Activate();

    /// <summary>Unsubscribes from amp push events. Call before disconnect.</summary>
    void Deactivate();

    /// <summary>Stops the write loop and cleans up. Call from the window Closing handler.</summary>
    void Shutdown();

    /// <summary>Starts or stops the write loop based on connection state.</summary>
    void OnConnectionChanged(bool connected);

    /// <summary> Read operations ─────────────────────────────────────────────────────── </summary>
    /// <summary>Reads the full patch from the amp and seeds KatanaState. Also wires write-back subscriptions.</summary>
    Task<bool> TryRefreshAmpStateAsync();
}
