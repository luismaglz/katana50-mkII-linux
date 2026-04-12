namespace Kataka.App.ViewModels.Design;

public sealed class DesignDelay2PedalViewModel : Delay2PedalViewModel
{
    public static DesignDelay2PedalViewModel Instance => new();

    public DesignDelay2PedalViewModel()
    {
        IsEnabled = true;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
