using System.Linq;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignReverbPedalViewModel : ReverbPedalViewModel
{
    public static DesignReverbPedalViewModel Instance => new();

    public DesignReverbPedalViewModel()
        : base(KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == "reverb"))
    {
        IsEnabled = true;
        Level = 55;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
