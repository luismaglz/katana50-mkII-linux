namespace Kataka.App.ViewModels.Design;

public sealed class DesignModFxPedalViewModel : ModFxPedalViewModel
{
    public static DesignModFxPedalViewModel Instance => new();

    public DesignModFxPedalViewModel() : base("mod")
    {
        IsEnabled = true;
        Level = 70;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
