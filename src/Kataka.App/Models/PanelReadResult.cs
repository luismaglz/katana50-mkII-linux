namespace Kataka.App.Services;

public record PanelReadResult(
    bool Success,
    string StatusMessage,
    string PanelControlsStatus,
    bool ResetLastDelayTap,
    bool PatchLevelLoaded,
    int? NewPatchLevel,
    bool DelayTimeLoaded,
    int? NewDelayTimeMs,
    string NewDelayTapStatus);