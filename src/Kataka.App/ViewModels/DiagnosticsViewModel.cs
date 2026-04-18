using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using Kataka.App.Logging;
using Kataka.App.Services;

using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class DiagnosticsViewModel : ViewModelBase
{
    private readonly IAmpSyncService _syncService;
    private readonly Func<bool> _isConnected;

    public DiagnosticsViewModel(IAmpSyncService syncService, ObservableLoggerProvider loggerProvider, Func<bool> isConnected)
    {
        _syncService = syncService;
        _isConnected = isConnected;

        loggerProvider.LogMessages.Subscribe(AppendRaw).DisposeWith(Disposables);

        syncService.ReadCompleted.Subscribe(meta =>
        {
            if (meta.AmpEditorStatus.Length > 0) AmpEditorStatus = meta.AmpEditorStatus;
        }).DisposeWith(Disposables);
    }

    [Reactive] public string DiagnosticLog { get; set; } = "Diagnostic log ready.";
    [Reactive] public string AmpEditorStatus { get; set; } = "Amp editor values have not been read yet.";

    internal void AppendRaw(string line)
    {
        DiagnosticLog = DiagnosticLog == "Diagnostic log ready."
            ? line
            : $"{line}{Environment.NewLine}{DiagnosticLog}";
    }

    [RelayCommand]
    private void ClearLog() => DiagnosticLog = "Diagnostic log ready.";

    [RelayCommand]
    private async Task ReadAmpControlsAsync() =>
        await (_isConnected() ? _syncService.TryRefreshAmpStateAsync() : Task.FromResult(false));

    [RelayCommand]
    private async Task ReadPanelControlsAsync() =>
        await (_isConnected() ? _syncService.TryRefreshAmpStateAsync() : Task.FromResult(false));

    [RelayCommand]
    private async Task ReadPedalControlsAsync() =>
        await (_isConnected() ? _syncService.TryRefreshAmpStateAsync() : Task.FromResult(false));
}
