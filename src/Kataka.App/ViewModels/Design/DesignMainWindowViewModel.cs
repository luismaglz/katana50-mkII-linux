using Kataka.App.Logging;
using Kataka.App.Services;

using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignMainWindowViewModel : MainWindowViewModel
{
    public DesignMainWindowViewModel() : base(
        new NullKatanaSession(),
        new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance),
        new AmpSyncService(new NullKatanaSession(),
            new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance),
            NullLogger<AmpSyncService>.Instance),
        NullLoggerFactory.Instance,
        new ObservableLoggerProvider())
    {
        // IsConnected is false, so none of these will queue MIDI writes.
        AmpEditor.Pedalboard.SelectedChainPattern = 2;
    }

    public static DesignMainWindowViewModel Instance => new();
}

public sealed class DesignAmpEditorViewModel : AmpEditorViewModel
{
    public DesignAmpEditorViewModel() : base(
        new NullKatanaSession(),
        new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance),
        new AmpSyncService(new NullKatanaSession(),
            new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance),
            NullLogger<AmpSyncService>.Instance),
        _ => { },
        NullLogger<AmpEditorViewModel>.Instance) =>
        Pedalboard.SelectedChainPattern = 2;

    public static DesignAmpEditorViewModel Instance => new();
}
