namespace Kataka.App.Services;

/// <summary>Metadata returned alongside a full device-state read so the VM can update derived UI.</summary>
public record DeviceReadMetadata(
    int? DelayTimeMs,
    bool DelayTimeLoaded,
    bool PatchLevelLoaded,
    string StatusMessage,
    string PanelControlsStatus,
    string PedalControlsStatus,
    string AmpEditorStatus);
