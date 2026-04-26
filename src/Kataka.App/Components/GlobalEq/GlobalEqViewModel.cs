using Kataka.App.KatanaState;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class GlobalEqViewModel : ViewModelBase
{
    private EqBankViewModel _activeBank;

    public GlobalEqViewModel(IKatanaState katanaState)
    {
        Sw = new AmpControlViewModel(katanaState.GlobalEq.Sw);
        Select = new AmpControlViewModel(katanaState.GlobalEq.Select);
        Bank1 = new EqBankViewModel(katanaState.GlobalEq.Bank1);
        Bank2 = new EqBankViewModel(katanaState.GlobalEq.Bank2);
        Bank3 = new EqBankViewModel(katanaState.GlobalEq.Bank3);

        _activeBank = BankVmFor(katanaState.GlobalEq.Select.Value);

        katanaState.GlobalEq.Select.ValueChanged += () =>
            ActiveBank = BankVmFor(katanaState.GlobalEq.Select.Value);

        katanaState.GlobalEq.Sw.ValueChanged += () => this.RaisePropertyChanged(nameof(IsOn));
    }

    public AmpControlViewModel Sw { get; }
    public AmpControlViewModel Select { get; }

    public bool IsOn
    {
        get => Sw.Value != 0;
        set => Sw.Value = value ? 1 : 0;
    }

    public EqBankViewModel Bank1 { get; }
    public EqBankViewModel Bank2 { get; }
    public EqBankViewModel Bank3 { get; }

    public EqBankViewModel ActiveBank
    {
        get => _activeBank;
        private set => this.RaiseAndSetIfChanged(ref _activeBank, value);
    }

    private EqBankViewModel BankVmFor(int selectValue) => selectValue switch
    {
        1 => Bank2,
        2 => Bank3,
        _ => Bank1
    };
}

public class EqBankViewModel : ViewModelBase
{
    public EqBankViewModel(GlobalEqState.EqBankState bank)
    {
        Type = new AmpControlViewModel(bank.Type!);
        Position = new AmpControlViewModel(bank.Position!);
        LowCut = new AmpControlViewModel(bank.LowCut!);
        LowGain = new AmpControlViewModel(bank.LowGain!);
        LowMidFreq = new AmpControlViewModel(bank.LowMidFreq!);
        LowMidQ = new AmpControlViewModel(bank.LowMidQ!);
        LowMidGain = new AmpControlViewModel(bank.LowMidGain!);
        HiMidFreq = new AmpControlViewModel(bank.HiMidFreq!);
        HiMidQ = new AmpControlViewModel(bank.HiMidQ!);
        HiMidGain = new AmpControlViewModel(bank.HiMidGain!);
        HighGain = new AmpControlViewModel(bank.HighGain!);
        HighCut = new AmpControlViewModel(bank.HighCut!);
        Level = new AmpControlViewModel(bank.Level!);

        Geq31Hz = new AmpControlViewModel(bank.Geq31Hz!);
        Geq62Hz = new AmpControlViewModel(bank.Geq62Hz!);
        Geq125Hz = new AmpControlViewModel(bank.Geq125Hz!);
        Geq250Hz = new AmpControlViewModel(bank.Geq250Hz!);
        Geq500Hz = new AmpControlViewModel(bank.Geq500Hz!);
        Geq1kHz = new AmpControlViewModel(bank.Geq1kHz!);
        Geq2kHz = new AmpControlViewModel(bank.Geq2kHz!);
        Geq4kHz = new AmpControlViewModel(bank.Geq4kHz!);
        Geq8kHz = new AmpControlViewModel(bank.Geq8kHz!);
        Geq16kHz = new AmpControlViewModel(bank.Geq16kHz!);
        GeqLevel = new AmpControlViewModel(bank.GeqLevel!);

        bank.Type!.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(IsParametric));
            this.RaisePropertyChanged(nameof(IsGraphic));
        };
    }

    // ── EQ type ─────────────────────────────────────────────────────────────
    public AmpControlViewModel Type { get; }

    // 0 = Parametric EQ, 1 = GE-10 (Graphic EQ)
    public bool IsParametric => Type.Value == 0;
    public bool IsGraphic => Type.Value == 1;

    // ── Parametric EQ parameters ─────────────────────────────────────────────
    // Position max=3:  0=PREAMP  1=LOOP  2=MASTER  3=OUTPUT
    public AmpControlViewModel Position { get; }

    // LowCut  max=17: 0=FLAT, 1=20Hz … 17=800Hz
    public AmpControlViewModel LowCut { get; }

    // Gain parameters, raw 0-40 where 20 = 0 dB (-20 to +20 dB)
    public AmpControlViewModel LowGain { get; }
    public AmpControlViewModel LowMidGain { get; }
    public AmpControlViewModel HiMidGain { get; }
    public AmpControlViewModel HighGain { get; }
    public AmpControlViewModel Level { get; }

    // LowMidFreq / HiMidFreq  max=27: 0=20Hz … 27=10.0kHz (28 steps)
    public AmpControlViewModel LowMidFreq { get; }
    public AmpControlViewModel HiMidFreq { get; }

    // Q  max=5: 0=0.5  1=1  2=2  3=4  4=8  5=16
    public AmpControlViewModel LowMidQ { get; }
    public AmpControlViewModel HiMidQ { get; }

    // HighCut max=14: 0=630Hz … 13=12.5kHz  14=FLAT
    public AmpControlViewModel HighCut { get; }

    // ── Graphic EQ bands ─────────────────────────────────────────────────────
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
