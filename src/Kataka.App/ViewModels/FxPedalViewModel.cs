using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public partial class FxPedalViewModel : PedalViewModel
{
    public FxPedalViewModel(KatanaPanelEffectDefinition definition) : base(definition) { }
}
