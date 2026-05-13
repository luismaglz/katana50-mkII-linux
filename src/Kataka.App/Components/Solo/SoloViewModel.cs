using Kataka.App.KatanaState;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class SoloViewModel : ViewModelBase
{
    private readonly AmpControlState _sw;

    public SoloViewModel(IKatanaState katanaState)
    {
        _sw = katanaState.Preamp.SoloSw;
        Level = new AmpControlViewModel(katanaState.Preamp.SoloLevel);

        var eq = katanaState.SoloEq;
        EqSw = new AmpControlViewModel(eq.EqSw);
        EqPosition = new AmpControlViewModel(eq.EqPosition);
        EqLowCut = new AmpControlViewModel(eq.EqLowCut);
        EqLowGain = new AmpControlViewModel(eq.EqLowGain);
        EqMidFreq = new AmpControlViewModel(eq.EqMidFreq);
        EqMidQ = new AmpControlViewModel(eq.EqMidQ);
        EqMidGain = new AmpControlViewModel(eq.EqMidGain);
        EqHighGain = new AmpControlViewModel(eq.EqHighGain);
        EqHighCut = new AmpControlViewModel(eq.EqHighCut);
        EqLevel = new AmpControlViewModel(eq.EqLevel);

        _sw.ValueChanged += () => this.RaisePropertyChanged(nameof(IsEnabled));
        eq.EqSw.ValueChanged += () => this.RaisePropertyChanged(nameof(IsEqOn));
    }

    public bool IsEnabled
    {
        get => _sw.Value != 0;
        set => _sw.Value = value ? 1 : 0;
    }

    public AmpControlViewModel Level { get; }

    public bool IsEqOn
    {
        get => EqSw.Value != 0;
        set => EqSw.Value = value ? 1 : 0;
    }

    public AmpControlViewModel EqSw { get; }
    public AmpControlViewModel EqPosition { get; }
    public AmpControlViewModel EqLowCut { get; }
    public AmpControlViewModel EqLowGain { get; }
    public AmpControlViewModel EqMidFreq { get; }
    public AmpControlViewModel EqMidQ { get; }
    public AmpControlViewModel EqMidGain { get; }
    public AmpControlViewModel EqHighGain { get; }
    public AmpControlViewModel EqHighCut { get; }
    public AmpControlViewModel EqLevel { get; }
}
