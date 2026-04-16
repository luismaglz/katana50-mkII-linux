using Kataka.App.Services;
using Kataka.App.ViewModels.Design;
using Kataka.Domain.KatanaState;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignMainWindowViewModel : MainWindowViewModel
{
    public static DesignMainWindowViewModel Instance => new();

    public DesignMainWindowViewModel() : base(new NullKatanaSession(), new KatanaState(), new AmpSyncService(new NullKatanaSession(), new KatanaState()))
    {
        // IsConnected is false, so none of these will queue MIDI writes.
        SelectedAmpType = "CRUNCH";
        SelectedCabinetResonance = "MIDDLE";
        SelectedPanelChannel = "CH A1";
        Pedalboard.SelectedChainPattern = 2;
        Pedalboard.Refresh();
    }
}
