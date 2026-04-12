namespace Kataka.App.ViewModels.Design;

public sealed class DesignModPedalViewModel : ModPedalViewModel
{
    public static DesignModPedalViewModel Instance => new();

    public DesignModPedalViewModel()
    {
        IsEnabled = true;
        Level = 70;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
