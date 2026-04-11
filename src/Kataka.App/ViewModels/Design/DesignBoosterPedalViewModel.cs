using System.Linq;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignBoosterPedalViewModel : BoosterPedalViewModel
{
    public static DesignBoosterPedalViewModel Instance => new();

    public DesignBoosterPedalViewModel()
        : base(KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == "booster"))
    {
        IsEnabled = true;
        Level = 64;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
