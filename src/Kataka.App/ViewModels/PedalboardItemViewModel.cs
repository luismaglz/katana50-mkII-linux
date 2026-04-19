using CommunityToolkit.Mvvm.ComponentModel;

namespace Kataka.App.ViewModels;

public sealed partial class PedalboardItemViewModel : ObservableObject
{
    public string Key { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    // Mutable so the AMP node can reflect channel changes without rebuilding the collection.
    [ObservableProperty] public partial string Detail { get; set; } = string.Empty;

    public bool IsEndpoint { get; init; }

    public bool IsInput => IsEndpoint && DisplayName == "INPUT";
    public bool IsOutput => IsEndpoint && DisplayName == "OUTPUT";

    public bool IsAmp { get; init; }

    public bool IsConnectedFromPrevious { get; init; }

    // For effect items in the chain, points to the live PedalViewModel with controls.
    public PedalViewModel? PanelEffect { get; init; }

    // Per-family flags for binding IsVisible in pedal-type-specific views.
    public bool IsBooster => Family is "booster";
    public bool IsMod => Family is "mod";
    public bool IsFx => Family is "fx";
    public bool IsDelay => Family is "delay";
    public bool IsDelay2 => Family is "delay2";
    public bool IsReverb => Family is "reverb";

    // Typed ViewModel accessors — only accessed when the corresponding IsXxx guard is true.
    public BoosterPedalViewModel BoosterPedal => (PanelEffect as BoosterPedalViewModel)!;
    public ModFxPedalViewModel ModFxPedal => (PanelEffect as ModFxPedalViewModel)!;
    public DelayPedalViewModel DelayPedal => (PanelEffect as DelayPedalViewModel)!;
    public ReverbPedalViewModel ReverbPedal => (PanelEffect as ReverbPedalViewModel)!;

    // Visual family: "boost", "mod", "fx", "delay", "delay2", "reverb", "amp", "io"
    public string Family { get; init; } = "io";

    // Card accent color based on family.
    public string FamilyAccentColor => Family switch
    {
        "booster" => "#e8960a", // amber
        "mod" => "#3dbf6c", // green
        "fx" => "#3a9fdf", // blue
        "delay" => "#30cfc0", // cyan
        "delay2" => "#30cfc0",
        "reverb" => "#a86fcb", // purple
        "amp" => "#c0392b", // red
        _ => "#607080" // slate for input/output
    };
}
