using System.Linq;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignModPedalViewModel : ModPedalViewModel
{
    public static DesignModPedalViewModel Instance => new();

    public DesignModPedalViewModel()
        : base(KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == "mod"))
    {
        IsEnabled = true;
        Level = 70;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
