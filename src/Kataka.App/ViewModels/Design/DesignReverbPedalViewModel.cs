namespace Kataka.App.ViewModels.Design;

public sealed class DesignReverbPedalViewModel : ReverbPedalViewModel
{
    public static DesignReverbPedalViewModel Instance => new();

    public DesignReverbPedalViewModel()
    {
        IsEnabled = true;
        Level = 55;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
