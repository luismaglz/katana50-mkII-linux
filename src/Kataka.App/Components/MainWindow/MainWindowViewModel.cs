using Avalonia.Platform.Storage;

using Kataka.App.KatanaState;
using Kataka.App.Logging;
using Kataka.App.Services;
using Kataka.Application.Katana;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IAmpSyncService syncService;

    public MainWindowViewModel(IKatanaSession katanaSession, IKatanaState katanaState, IAmpSyncService ampSyncService,
        ILoggerFactory loggerFactory, ObservableLoggerProvider loggerProvider)
    {
        syncService = ampSyncService;

        MidiConnection = new MidiConnectionViewModel(katanaSession, ampSyncService, AppendStatus,
            loggerFactory.CreateLogger<MidiConnectionViewModel>());
        Diagnostics = new DiagnosticsViewModel(ampSyncService, loggerProvider, () => MidiConnection.IsConnected);
        Patch = new PatchViewModel(katanaSession, ampSyncService, () => MidiConnection.IsConnected, AppendStatus,
            loggerFactory.CreateLogger<PatchViewModel>());
        AmpEditor = new AmpEditorViewModel(katanaState);
        GlobalEq = new GlobalEqViewModel(katanaState);
        PedalboardMiniMap = new PedalboardMiniMapViewModel(katanaState);

        syncService.StatusMessages.Subscribe(msg => StatusMessage = msg).DisposeWith(Disposables);

        MidiConnection.WhenAnyValue(x => x.IsConnected)
            .Subscribe(v => Patch.CanWritePatch = v)
            .DisposeWith(Disposables);
    }

    public MidiConnectionViewModel MidiConnection { get; }
    public DiagnosticsViewModel Diagnostics { get; }
    public PatchViewModel Patch { get; }
    public AmpEditorViewModel AmpEditor { get; }

    public PedalboardMiniMapViewModel PedalboardMiniMap { get; }
    public GlobalEqViewModel GlobalEq { get; }
    public PaletteEditorViewModel PaletteEditor { get; } = new();

    [Reactive] public string StatusMessage { get; set; } = "Ready to scan for MIDI devices.";

    public IStorageProvider? StorageProvider
    {
        set => Patch.StorageProvider = value;
    }

    public void Shutdown() => syncService.Shutdown();

    /// <summary> Private helpers ─────────────────────────────────────────────────────── </summary>
    private void AppendStatus(string msg) => StatusMessage = msg;
}
