using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignBoosterPedalViewModel : BoosterPedalViewModel
{
    public static DesignBoosterPedalViewModel Instance => new();

    public DesignBoosterPedalViewModel() : base(new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance))
    {
        IsEnabled = true;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
