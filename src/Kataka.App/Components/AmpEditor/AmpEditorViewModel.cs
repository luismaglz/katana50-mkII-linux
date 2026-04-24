using Kataka.App.KatanaState;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public class AmpEditorViewModel : ViewModelBase
{
    private readonly IKatanaState _katanaState;

    public AmpEditorViewModel(
        IKatanaState katanaState)
    {
        _katanaState = katanaState;

        katanaState.AmpVariation.ValueChanged += () =>
            IsAmpVariation = katanaState.AmpVariation.Value != 0;

        Panel = new PanelViewModel(katanaState);
        ChannelSelection = new ChannelSelectionViewModel(katanaState);

        this.WhenAnyValue(x => x.IsAmpVariation)
            .Subscribe(v => _katanaState.AmpVariation.Value = v ? 1 : 0)
            .DisposeWith(Disposables);
    }

    public PedalboardViewModel Pedalboard { get; }
    public PanelViewModel Panel { get; }
    public ChannelSelectionViewModel ChannelSelection { get; }


    [Reactive] public bool IsAmpVariation { get; set; }
}
