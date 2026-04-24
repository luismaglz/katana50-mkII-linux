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
    }

    public static DesignMainWindowViewModel Instance => new();
}

public sealed class DesignAmpEditorViewModel : AmpEditorViewModel
{
    public DesignAmpEditorViewModel() : base(
        new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance))
    {
    }

    public static DesignAmpEditorViewModel Instance => new();
}
