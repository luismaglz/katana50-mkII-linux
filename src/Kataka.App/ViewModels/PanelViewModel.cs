using Kataka.App.KatanaState;

namespace Kataka.App.ViewModels;

public class PanelViewModel : ViewModelBase
{
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
    }

    public static string[] CabinetResonanceOptions { get; } = ["LOW", "MIDDLE", "HIGH"];

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
}
