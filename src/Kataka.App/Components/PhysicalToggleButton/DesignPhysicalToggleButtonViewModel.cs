namespace Kataka.App.Components.PhysicalToggleButton;

public sealed class DesignPhysicalToggleButtonViewModel
{
    public bool IsChecked { get; set; } = true;
    public string DisplayName { get; set; } = "BOOSTER";

    public static DesignPhysicalToggleButtonViewModel Instance => new();
}
