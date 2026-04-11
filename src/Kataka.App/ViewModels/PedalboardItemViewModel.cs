using CommunityToolkit.Mvvm.ComponentModel;

namespace Kataka.App.ViewModels;

public sealed partial class PedalboardItemViewModel : ObservableObject
{
    public string Key { get; init; } = string.Empty;

    [ObservableProperty]
    private bool isSelected;

    public string DisplayName { get; init; } = string.Empty;

    public string Detail { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public bool IsEndpoint { get; init; }

    public bool IsAmp { get; init; }

    public bool IsConnectedFromPrevious { get; init; }

    public bool CanToggle { get; init; }

    // Visual family: "boost", "mod", "fx", "delay", "reverb", "pedal", "amp", "io"
    public string Family { get; init; } = "io";

    // Card accent color based on family (hex strings for Avalonia SolidColorBrush).
    public string FamilyAccentColor => Family switch
    {
        "boost"  => "#e8960a",  // amber
        "mod"    => "#3dbf6c",  // green
        "fx"     => "#3a9fdf",  // blue
        "delay"  => "#30cfc0",  // cyan
        "delay2" => "#30cfc0",
        "reverb" => "#a86fcb",  // purple
        "pedal"  => "#e85c1a",  // orange
        "amp"    => "#c0392b",  // red
        _        => "#607080",  // slate for input/output
    };

    // LED glow color when active.
    public string LedColor => IsActive ? FamilyAccentColor : "#35383f";
}
