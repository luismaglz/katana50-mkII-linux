using System;
using System.Collections.ObjectModel;
using Kataka.App.ViewModels;
using Kataka.Application.Katana;
using Kataka.Domain.Midi;

namespace Kataka.App.Services;

/// <summary>
/// The slice of MainWindowViewModel that AmpSyncService needs to read and write.
/// Implemented by MainWindowViewModel so the service never holds a concrete VM reference.
/// </summary>
public interface IAmpSyncContext
{
    // ── Flags the service reads ───────────────────────────────────────────────

    bool IsConnected { get; }
    bool ActiveWriteSync { get; }

    /// <summary>
    /// Set by the service during ApplyDeviceState; read by the VM's partial OnXxx handlers
    /// and by the service's Track* methods to avoid re-enqueuing device-driven updates.
    /// </summary>
    bool SuppressChangeTracking { get; set; }

    bool PatchLevelMappingVerified { get; }

    // ── Panel state the service reads and writes ──────────────────────────────

    string StatusMessage { get; set; }
    string AmpEditorStatus { get; set; }
    string PanelControlsStatus { get; set; }
    string PedalControlsStatus { get; set; }
    string DelayTapStatus { get; set; }
    int? DelayTimeMs { get; set; }
    int PatchLevel { get; set; }

    string SelectedAmpType { get; set; }
    string SelectedCabinetResonance { get; set; }
    bool IsAmpVariation { get; set; }
    string SelectedPanelChannel { get; set; }

    // ── Collections ──────────────────────────────────────────────────────────

    ObservableCollection<AmpControlViewModel> AmpControls { get; }
    ObservableCollection<IBasePedal> PanelEffects { get; }
    PedalFxViewModel PedalFx { get; }
    PedalboardViewModel Pedalboard { get; }

    // ── Option tables (static on the VM; exposed as instance for the interface) ─

    string[] AmpTypeOptions { get; }
    string[] CabinetResonanceOptions { get; }

    // ── Callbacks ────────────────────────────────────────────────────────────

    void Log(string message);
    void ApplyPedalValue(string parameterKey, byte value);
    void ResetDelayTap();

    // ── Static channel helpers shared by VM and service ───────────────────────

    static KatanaPanelChannel ParsePanelChannelDisplay(string display) => display switch
    {
        "CH A1" => KatanaPanelChannel.ChA1,
        "CH A2" => KatanaPanelChannel.ChA2,
        "CH B1" => KatanaPanelChannel.ChB1,
        "CH B2" => KatanaPanelChannel.ChB2,
        _       => KatanaPanelChannel.Panel,
    };

    static string ToPanelChannelDisplay(KatanaPanelChannel channel) => channel switch
    {
        KatanaPanelChannel.ChA1 => "CH A1",
        KatanaPanelChannel.ChA2 => "CH A2",
        KatanaPanelChannel.ChB1 => "CH B1",
        KatanaPanelChannel.ChB2 => "CH B2",
        _                       => "Panel",
    };
}
