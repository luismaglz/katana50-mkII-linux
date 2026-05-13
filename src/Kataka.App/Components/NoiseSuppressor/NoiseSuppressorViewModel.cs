using Kataka.App.KatanaState;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class NoiseSuppressorViewModel : ViewModelBase
{
    private readonly NoiseSuppressorState _state;

    public NoiseSuppressorViewModel(IKatanaState katanaState)
    {
        _state = katanaState.NoiseSuppressor;
        Threshold = new AmpControlViewModel(_state.Threshold);
        Release = new AmpControlViewModel(_state.Release);

        _state.Sw.ValueChanged += () => this.RaisePropertyChanged(nameof(IsEnabled));
    }

    public AmpControlViewModel Threshold { get; }
    public AmpControlViewModel Release { get; }

    public bool IsEnabled
    {
        get => _state.Sw.Value != 0;
        set => _state.Sw.Value = value ? 1 : 0;
    }
}
