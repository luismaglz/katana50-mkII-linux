namespace Kataka.App.ViewModels.Design;

public sealed class DesignBoosterPedalViewModel : BoosterPedalViewModel
{
    public static DesignBoosterPedalViewModel Instance => new();

    public DesignBoosterPedalViewModel()
    {
        IsEnabled = true;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
