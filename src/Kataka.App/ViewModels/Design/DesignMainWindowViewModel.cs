using Kataka.App.Logging;
using Kataka.App.Services;
using Kataka.App.ViewModels.Design;

using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignMainWindowViewModel : MainWindowViewModel
{
    public static DesignMainWindowViewModel Instance => new();

    public DesignMainWindowViewModel() : base(
        new NullKatanaSession(),
        new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance),
        new AmpSyncService(new NullKatanaSession(), new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance), NullLogger<AmpSyncService>.Instance),
        NullLoggerFactory.Instance,
        new ObservableLoggerProvider())
    {
        // IsConnected is false, so none of these will queue MIDI writes.
        AmpEditor.SelectedAmpType = "CRUNCH";
        AmpEditor.SelectedCabinetResonance = "MIDDLE";
        AmpEditor.SelectedPanelChannel = "CH A1";
        AmpEditor.Pedalboard.SelectedChainPattern = 2;
        AmpEditor.Pedalboard.Refresh();
    }
}

public sealed class DesignAmpEditorViewModel : AmpEditorViewModel
{
    public static DesignAmpEditorViewModel Instance => new();

    public DesignAmpEditorViewModel() : base(
        new NullKatanaSession(),
        new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance),
        new AmpSyncService(new NullKatanaSession(), new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance), NullLogger<AmpSyncService>.Instance),
        _ => { },
        NullLogger<AmpEditorViewModel>.Instance)
    {
        SelectedAmpType = "CRUNCH";
        SelectedCabinetResonance = "MIDDLE";
        SelectedPanelChannel = "CH A1";
        Pedalboard.SelectedChainPattern = 2;
        Pedalboard.Refresh();
    }
}
