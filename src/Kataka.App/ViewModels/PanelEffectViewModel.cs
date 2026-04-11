using CommunityToolkit.Mvvm.ComponentModel;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public partial class PanelEffectViewModel : ViewModelBase
{
    public PanelEffectViewModel(KatanaPanelEffectDefinition definition)
    {
        Definition = definition;
    }

    public KatanaPanelEffectDefinition Definition { get; }

    public string DisplayName => Definition.DisplayName;

    [ObservableProperty]
    public partial bool IsEnabled { get; set; }

    [ObservableProperty]
    public partial string Variation { get; set; } = "Unknown";
}
