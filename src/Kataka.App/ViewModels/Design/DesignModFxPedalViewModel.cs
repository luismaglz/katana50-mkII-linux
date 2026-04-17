using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignModFxPedalViewModel : ModFxPedalViewModel
{
    public static DesignModFxPedalViewModel Instance => new();

    public DesignModFxPedalViewModel()
        : base("mod", new Kataka.App.KatanaState.KatanaState(NullLogger<Kataka.App.KatanaState.KatanaState>.Instance))
    {
        IsEnabled = true;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
