using Kataka.App.Logging;
using Kataka.App.Services;

using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignDiagnosticsViewModel : DiagnosticsViewModel
{
    public static DesignDiagnosticsViewModel Instance => new();

    public DesignDiagnosticsViewModel()
        : base(
            new AmpSyncService(
                new NullKatanaSession(),
                new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance),
                NullLogger<AmpSyncService>.Instance),
            new ObservableLoggerProvider(),
            () => false)
    {
        DiagnosticLog = "Design-time diagnostic log.";
        AmpEditorStatus = "Design-time status.";
    }
}
