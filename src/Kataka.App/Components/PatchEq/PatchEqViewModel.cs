using Kataka.App.KatanaState;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class PatchEqViewModel : ViewModelBase
{
    public PatchEqViewModel(IKatanaState katanaState)
    {
        var eq = katanaState.PatchEq2;

        Sw = new AmpControlViewModel(eq.Sw);
        Type = new AmpControlViewModel(eq.Type);
        Position = new AmpControlViewModel(eq.Position!);
        LowCut = new AmpControlViewModel(eq.LowCut);
        LowGain = new AmpControlViewModel(eq.LowGain);
        LowMidFreq = new AmpControlViewModel(eq.LowMidFreq);
        LowMidQ = new AmpControlViewModel(eq.LowMidQ);
        LowMidGain = new AmpControlViewModel(eq.LowMidGain);
        HiMidFreq = new AmpControlViewModel(eq.HiMidFreq);
        HiMidQ = new AmpControlViewModel(eq.HiMidQ);
        HiMidGain = new AmpControlViewModel(eq.HiMidGain);
        HighGain = new AmpControlViewModel(eq.HighGain);
        HighCut = new AmpControlViewModel(eq.HighCut);
        Level = new AmpControlViewModel(eq.Level);
        Geq31Hz = new AmpControlViewModel(eq.Geq31Hz);
        Geq62Hz = new AmpControlViewModel(eq.Geq62Hz);
        Geq125Hz = new AmpControlViewModel(eq.Geq125Hz);
        Geq250Hz = new AmpControlViewModel(eq.Geq250Hz);
        Geq500Hz = new AmpControlViewModel(eq.Geq500Hz);
        Geq1kHz = new AmpControlViewModel(eq.Geq1kHz);
        Geq2kHz = new AmpControlViewModel(eq.Geq2kHz);
        Geq4kHz = new AmpControlViewModel(eq.Geq4kHz);
        Geq8kHz = new AmpControlViewModel(eq.Geq8kHz);
        Geq16kHz = new AmpControlViewModel(eq.Geq16kHz);
        GeqLevel = new AmpControlViewModel(eq.GeqLevel);

        eq.Sw.ValueChanged += () => this.RaisePropertyChanged(nameof(IsOn));
        eq.Type.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(IsParametric));
            this.RaisePropertyChanged(nameof(IsGraphic));
        };
    }

    public AmpControlViewModel Sw { get; }
    public AmpControlViewModel Type { get; }
    public AmpControlViewModel Position { get; }

    public bool IsOn
    {
        get => Sw.Value != 0;
        set => Sw.Value = value ? 1 : 0;
    }

    // 0 = Parametric EQ, 1 = Graphic EQ
    public bool IsParametric => Type.Value == 0;
    public bool IsGraphic => Type.Value == 1;

    // Parametric
    public AmpControlViewModel LowCut { get; }
    public AmpControlViewModel LowGain { get; }
    public AmpControlViewModel LowMidFreq { get; }
    public AmpControlViewModel LowMidQ { get; }
    public AmpControlViewModel LowMidGain { get; }
    public AmpControlViewModel HiMidFreq { get; }
    public AmpControlViewModel HiMidQ { get; }
    public AmpControlViewModel HiMidGain { get; }
    public AmpControlViewModel HighGain { get; }
    public AmpControlViewModel HighCut { get; }
    public AmpControlViewModel Level { get; }

    // Graphic
    public AmpControlViewModel Geq31Hz { get; }
    public AmpControlViewModel Geq62Hz { get; }
    public AmpControlViewModel Geq125Hz { get; }
    public AmpControlViewModel Geq250Hz { get; }
    public AmpControlViewModel Geq500Hz { get; }
    public AmpControlViewModel Geq1kHz { get; }
    public AmpControlViewModel Geq2kHz { get; }
    public AmpControlViewModel Geq4kHz { get; }
    public AmpControlViewModel Geq8kHz { get; }
    public AmpControlViewModel Geq16kHz { get; }
    public AmpControlViewModel GeqLevel { get; }
}
