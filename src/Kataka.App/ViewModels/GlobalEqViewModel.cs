using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Kataka.App.KatanaState;

namespace Kataka.App.ViewModels;

public class GlobalEqViewModel : ViewModelBase
{
    private readonly IKatanaState _katanaState;

    public GlobalEqViewModel(IKatanaState katanaState)
    {
        _katanaState = katanaState;

        // Update VM when selected bank changes
        _katanaState.GlobalEq.Select.ValueChanged += UpdateFromActiveBank;

        // Subscribe to bank members to pick up changes from amp reads
        foreach (var bank in new[] { _katanaState.GlobalEq.Bank1, _katanaState.GlobalEq.Bank2, _katanaState.GlobalEq.Bank3 })
        {
            bank.Geq31Hz.ValueChanged += UpdateFromActiveBank;
            bank.Geq62Hz.ValueChanged += UpdateFromActiveBank;
            bank.Geq125Hz.ValueChanged += UpdateFromActiveBank;
            bank.Geq250Hz.ValueChanged += UpdateFromActiveBank;
            bank.Geq500Hz.ValueChanged += UpdateFromActiveBank;
            bank.Geq1kHz.ValueChanged += UpdateFromActiveBank;
            bank.Geq2kHz.ValueChanged += UpdateFromActiveBank;
            bank.Geq4kHz.ValueChanged += UpdateFromActiveBank;
            bank.Geq8kHz.ValueChanged += UpdateFromActiveBank;
            bank.Geq16kHz.ValueChanged += UpdateFromActiveBank;
            bank.GeqLevel.ValueChanged += UpdateFromActiveBank;
            bank.Type.ValueChanged += UpdateFromActiveBank;
            bank.Sw.ValueChanged += UpdateFromActiveBank;
        }

        // Write-back from VM to state
        this.WhenAnyValue(x => x.GraphicEq31Hz)
            .Subscribe(v => ActiveBank().Geq31Hz.Value = v)
            .DisposeWith(Disposables);
        this.WhenAnyValue(x => x.GraphicEq62Hz)
            .Subscribe(v => ActiveBank().Geq62Hz.Value = v)
            .DisposeWith(Disposables);
        this.WhenAnyValue(x => x.GraphicEq125Hz)
            .Subscribe(v => ActiveBank().Geq125Hz.Value = v)
            .DisposeWith(Disposables);
        this.WhenAnyValue(x => x.GraphicEq250Hz)
            .Subscribe(v => ActiveBank().Geq250Hz.Value = v)
            .DisposeWith(Disposables);
        this.WhenAnyValue(x => x.GraphicEq500Hz)
            .Subscribe(v => ActiveBank().Geq500Hz.Value = v)
            .DisposeWith(Disposables);
        this.WhenAnyValue(x => x.GraphicEq1kHz)
            .Subscribe(v => ActiveBank().Geq1kHz.Value = v)
            .DisposeWith(Disposables);
        this.WhenAnyValue(x => x.GraphicEq2kHz)
            .Subscribe(v => ActiveBank().Geq2kHz.Value = v)
            .DisposeWith(Disposables);
        this.WhenAnyValue(x => x.GraphicEq4kHz)
            .Subscribe(v => ActiveBank().Geq4kHz.Value = v)
            .DisposeWith(Disposables);
        this.WhenAnyValue(x => x.GraphicEq8kHz)
            .Subscribe(v => ActiveBank().Geq8kHz.Value = v)
            .DisposeWith(Disposables);
        this.WhenAnyValue(x => x.GraphicEq16kHz)
            .Subscribe(v => ActiveBank().Geq16kHz.Value = v)
            .DisposeWith(Disposables);
        this.WhenAnyValue(x => x.GraphicEqLevel)
            .Subscribe(v => ActiveBank().GeqLevel.Value = v)
            .DisposeWith(Disposables);

        this.WhenAnyValue(x => x.Type)
            .Subscribe(v => ActiveBank().Type.Value = v)
            .DisposeWith(Disposables);

        this.WhenAnyValue(x => x.IsOn)
            .Subscribe(v => ActiveBank().Sw.Value = v ? 1 : 0)
            .DisposeWith(Disposables);

        // Initialize from current state
        UpdateFromActiveBank();
    }

    private GlobalEqState.EqBankState ActiveBank()
    {
        return _katanaState.GlobalEq.Select.Value switch
        {
            1 => _katanaState.GlobalEq.Bank2,
            2 => _katanaState.GlobalEq.Bank3,
            _ => _katanaState.GlobalEq.Bank1
        };
    }

    private void UpdateFromActiveBank()
    {
        var bank = ActiveBank();
        GraphicEq31Hz = bank.Geq31Hz.Value;
        GraphicEq62Hz = bank.Geq62Hz.Value;
        GraphicEq125Hz = bank.Geq125Hz.Value;
        GraphicEq250Hz = bank.Geq250Hz.Value;
        GraphicEq500Hz = bank.Geq500Hz.Value;
        GraphicEq1kHz = bank.Geq1kHz.Value;
        GraphicEq2kHz = bank.Geq2kHz.Value;
        GraphicEq4kHz = bank.Geq4kHz.Value;
        GraphicEq8kHz = bank.Geq8kHz.Value;
        GraphicEq16kHz = bank.Geq16kHz.Value;
        GraphicEqLevel = bank.GeqLevel.Value;
        Type = bank.Type.Value;
        IsOn = bank.Sw.Value != 0;
    }

    [Reactive] public int GraphicEq31Hz { get; set; }
    [Reactive] public int GraphicEq62Hz { get; set; }
    [Reactive] public int GraphicEq125Hz { get; set; }
    [Reactive] public int GraphicEq250Hz { get; set; }
    [Reactive] public int GraphicEq500Hz { get; set; }
    [Reactive] public int GraphicEq1kHz { get; set; }
    [Reactive] public int GraphicEq2kHz { get; set; }
    [Reactive] public int GraphicEq4kHz { get; set; }
    [Reactive] public int GraphicEq8kHz { get; set; }
    [Reactive] public int GraphicEq16kHz { get; set; }
    [Reactive] public int GraphicEqLevel { get; set; }

    [Reactive] public int Type { get; set; }
    [Reactive] public bool IsOn { get; set; }
}