using System.Linq;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignDelayPedalViewModel : DelayPedalViewModel
{
    public static DesignDelayPedalViewModel Instance => new();

    public DesignDelayPedalViewModel()
        : base(KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == "delay"))
    {
        IsEnabled = true;
        Level = 60;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
