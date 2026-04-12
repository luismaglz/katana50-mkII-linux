using System;
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

    // For effect items in the chain, points to the live PedalViewModel with controls.
    public IBasePedal? PanelEffect { get; init; }

    // Computed type helpers for conditional rendering.
    public bool IsPedalFx    => string.Equals(Key, "pedal-fx", StringComparison.Ordinal);
    public bool IsPanelEffect => PanelEffect is not null;
    public bool IsRegularPedal => !IsEndpoint && !IsAmp;

    // Per-family flags for binding IsVisible in pedal-type-specific views.
    public bool IsBooster => Family is "booster";
    public bool IsMod     => Family is "mod";
    public bool IsFx      => Family is "fx";
    public bool IsDelay   => Family is "delay";
    public bool IsDelay2  => Family is "delay2";
    public bool IsReverb  => Family is "reverb";

    // Typed ViewModel accessors — each pedal view's DataContext is bound to its specific type.
    public BoosterPedalViewModel? BoosterPedal => PanelEffect as BoosterPedalViewModel;
    public ModFxPedalViewModel?   ModFxPedal   => PanelEffect as ModFxPedalViewModel;
    public DelayPedalViewModel?   DelayPedal   => PanelEffect as DelayPedalViewModel;
    public ReverbPedalViewModel?  ReverbPedal  => PanelEffect as ReverbPedalViewModel;

    // Visual family: "boost", "mod", "fx", "delay", "reverb", "pedal", "amp", "io"
    public string Family { get; init; } = "io";

    // Card accent color based on family (hex strings for Avalonia SolidColorBrush).
    public string FamilyAccentColor => Family switch
    {
        "booster" => "#e8960a",  // amber
        "mod"     => "#3dbf6c",  // green
        "fx"      => "#3a9fdf",  // blue
        "delay"   => "#30cfc0",  // cyan
        "delay2"  => "#30cfc0",
        "reverb"  => "#a86fcb",  // purple
        "pedal"   => "#e85c1a",  // orange
        "amp"     => "#c0392b",  // red
        _         => "#607080",  // slate for input/output
    };

    // LED glow color when active.
    public string LedColor => IsActive ? FamilyAccentColor : "#35383f";
}
