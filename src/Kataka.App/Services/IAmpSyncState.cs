using System.Collections.Generic;

using Kataka.App.ViewModels;
using Kataka.Application.Katana;
using Kataka.Domain.Midi;

namespace Kataka.App.Services;

/// <summary>
/// The read-only slice of MainWindowViewModel that AmpSyncService needs to query.
/// The service reads these values to decide whether to queue writes; it no longer writes
/// back to VM properties (those flow via the service's observable outputs instead).
/// </summary>
public interface IAmpSyncState
{
    bool IsConnected { get; }
    bool ActiveWriteSync { get; }

    /// <summary>
    /// Set by the VM while applying a device-pushed snapshot, so the service's
    /// domain-state write-tracking skips echo writes during that window.
    /// </summary>
    bool SuppressChangeTracking { get; set; }

    bool PatchLevelMappingVerified { get; }
    int PatchLevel { get; set; }

    IReadOnlyList<AmpControlViewModel> AmpControls { get; }
    IReadOnlyList<IBasePedal> PanelEffects { get; }
    PedalFxViewModel PedalFx { get; }
    PedalboardViewModel Pedalboard { get; }

    // ── Static channel helpers shared by VM and service ───────────────────────

    static KatanaPanelChannel ParsePanelChannelDisplay(string display) => display switch
    {
        "CH A1" => KatanaPanelChannel.ChA1,
        "CH A2" => KatanaPanelChannel.ChA2,
        "CH B1" => KatanaPanelChannel.ChB1,
        "CH B2" => KatanaPanelChannel.ChB2,
        _ => KatanaPanelChannel.Panel,
    };

    static string ToPanelChannelDisplay(KatanaPanelChannel channel) => channel switch
    {
        KatanaPanelChannel.ChA1 => "CH A1",
        KatanaPanelChannel.ChA2 => "CH A2",
        KatanaPanelChannel.ChB1 => "CH B1",
        KatanaPanelChannel.ChB2 => "CH B2",
        _ => "Panel",
    };
}
