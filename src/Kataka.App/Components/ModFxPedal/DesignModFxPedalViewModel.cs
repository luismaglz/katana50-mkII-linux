using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignModFxPedalViewModel : ModFxPedalViewModel
{
    public DesignModFxPedalViewModel()
        : base("mod", new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance))
    {
        IsEnabled = true;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }

    public static DesignModFxPedalViewModel Instance => new();
}
