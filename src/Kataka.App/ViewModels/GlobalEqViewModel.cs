using Kataka.App.KatanaState;

using ReactiveUI;

namespace Kataka.App.ViewModels;

/// <summary>
///     ViewModel for the Global EQ panel. Exposes the currently-active EQ bank as
///     <see cref="ActiveBank" /> (an <see cref="EqBankViewModel" />). When the amp
///     switches banks, <see cref="ActiveBank" /> is swapped and the view re-binds
///     automatically — no circular write-back possible because all mutations go
///     through <see cref="AmpControlViewModel.Value" /> → <see cref="AmpControlState.WriteRequested" />.
/// </summary>
public class GlobalEqViewModel : ViewModelBase
{
    private EqBankViewModel _activeBank;

    public GlobalEqViewModel(IKatanaState katanaState)
    {
        Sw = new AmpControlViewModel(katanaState.GlobalEq.Sw);
        Bank1 = new EqBankViewModel(katanaState.GlobalEq.Bank1);
        Bank2 = new EqBankViewModel(katanaState.GlobalEq.Bank2);
        Bank3 = new EqBankViewModel(katanaState.GlobalEq.Bank3);

        _activeBank = BankVmFor(katanaState.GlobalEq.Select.Value);

        // When amp changes the active bank, swap ActiveBank (no write-back — SetFromAmp
        // fires only ValueChanged, never WriteRequested).
        katanaState.GlobalEq.Select.ValueChanged +=
            () => ActiveBank = BankVmFor(katanaState.GlobalEq.Select.Value);

        // Boolean wrapper for IsOn — stays in sync with Sw.Value without write-back.
        katanaState.GlobalEq.Sw.ValueChanged += () => this.RaisePropertyChanged(nameof(IsOn));
    }

    /// <summary>Master EQ on/off (PRM_SYS_EQ_SW) — shared across all banks.</summary>
    public AmpControlViewModel Sw { get; }

    /// <summary>Boolean wrapper over <see cref="Sw" />.Value for CheckBox binding.</summary>
    public bool IsOn
    {
        get => Sw.Value != 0;
        set => Sw.Value = value ? 1 : 0;
    }

    public EqBankViewModel Bank1 { get; }
    public EqBankViewModel Bank2 { get; }
    public EqBankViewModel Bank3 { get; }

    /// <summary>The VM for the currently-selected EQ bank. The view binds to this.</summary>
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

/// <summary>
///     Wraps a single <see cref="GlobalEqState.EqBankState" /> with one
///     <see cref="AmpControlViewModel" /> per parameter. The view binds directly to
///     these — amp reads update them through <c>ValueChanged</c> → <c>PropertyChanged</c>,
///     and UI edits write through the <c>Value</c> setter → <c>WriteRequested</c>.
/// </summary>
public class EqBankViewModel : ViewModelBase
{
    public EqBankViewModel(GlobalEqState.EqBankState bank)
    {
        Type = new AmpControlViewModel(bank.Type!);
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
    }

    public AmpControlViewModel Type { get; }
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