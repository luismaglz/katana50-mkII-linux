namespace Kataka.App.ViewModels.Design;

public sealed class DesignDelayPedalViewModel : DelayPedalViewModel
{
    public static DesignDelayPedalViewModel Instance => new();

    public DesignDelayPedalViewModel() : base("delay")
    {
        IsEnabled = true;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
