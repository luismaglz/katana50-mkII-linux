using Kataka.App.KatanaState;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class PanelViewModel : ViewModelBase
{
    private readonly AmpControlState _contourSelect;
    private readonly AmpControlState _contourSw;
    private readonly AmpControlState _globalEqSw;
    private readonly AmpControlState _soloSw;

    public PanelViewModel(IKatanaState katanaState)
    {
        AmpType = new AmpControlViewModel(katanaState.AmpType);
        AmpVariation = new AmpControlViewModel(katanaState.AmpVariation);
        Gain = new AmpControlViewModel(katanaState.Gain);
        Volume = new AmpControlViewModel(katanaState.Volume);
        Bass = new AmpControlViewModel(katanaState.Bass);
        Middle = new AmpControlViewModel(katanaState.Middle);
        Treble = new AmpControlViewModel(katanaState.Treble);
        Presence = new AmpControlViewModel(katanaState.Presence);
        CabinetResonance = new AmpControlViewModel(katanaState.CabinetResonance);

        _globalEqSw = katanaState.GlobalEq.Sw;
        _globalEqSw.ValueChanged += () => this.RaisePropertyChanged(nameof(IsGlobalEqOn));

        _contourSw = katanaState.ContourSw;
        _contourSelect = katanaState.ContourSelect;
        _contourSw.ValueChanged += () => this.RaisePropertyChanged(nameof(ContourIndex));
        _contourSelect.ValueChanged += () => this.RaisePropertyChanged(nameof(ContourIndex));

        _soloSw = katanaState.Preamp.SoloSw;
        _soloSw.ValueChanged += () => this.RaisePropertyChanged(nameof(IsSoloOn));
        SoloLevel = new AmpControlViewModel(katanaState.Preamp.SoloLevel);
    }

    public static string[] CabinetResonanceOptions { get; } = ["LOW", "MIDDLE", "HIGH"];
    public static string[] AmpTypeOptions { get; } = ["ACOUSTIC", "CLEAN", "CRUNCH", "LEAD", "BROWN"];
    public static string[] ContourOptions { get; } = ["OFF", "1", "2", "3"];

    #region AmpControls

    public AmpControlViewModel AmpType { get; internal set; }
    public AmpControlViewModel AmpVariation { get; internal set; }
    public AmpControlViewModel Gain { get; internal set; }
    public AmpControlViewModel Volume { get; internal set; }
    public AmpControlViewModel Bass { get; internal set; }
    public AmpControlViewModel Middle { get; internal set; }
    public AmpControlViewModel Treble { get; internal set; }
    public AmpControlViewModel Presence { get; internal set; }
    public AmpControlViewModel CabinetResonance { get; internal set; }

    #endregion

    #region ToneShape

    public bool IsGlobalEqOn
    {
        get => _globalEqSw.Value != 0;
        set => _globalEqSw.Value = value ? 1 : 0;
    }

    // 0 = Off, 1 = Contour preset 1, 2 = Contour preset 2, 3 = Contour preset 3
    public int ContourIndex
    {
        get => _contourSw.Value == 0 ? 0 : _contourSelect.Value + 1;
        set
        {
            if (value == 0)
            {
                _contourSw.Value = 0;
            }
            else
            {
                _contourSw.Value = 1;
                _contourSelect.Value = value - 1;
            }
        }
    }

    public bool IsSoloOn
    {
        get => _soloSw.Value != 0;
        set => _soloSw.Value = value ? 1 : 0;
    }

    public AmpControlViewModel SoloLevel { get; }

    #endregion
}
