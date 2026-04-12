namespace Kataka.App.ViewModels.Design;

public sealed class DesignFxPedalViewModel : FxPedalViewModel
{
    public static DesignFxPedalViewModel Instance => new();

    public DesignFxPedalViewModel()
    {
        IsEnabled = true;
        Level = 75;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
