using System;
using System.Collections.Generic;

using Avalonia.Platform.Storage;

using Kataka.App.KatanaState;
using Kataka.App.Logging;
using Kataka.App.Services;
using Kataka.Application.Katana;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IAmpSyncState
{
    private readonly IAmpSyncService syncService;

    public MainWindowViewModel(IKatanaSession katanaSession, IKatanaState katanaState, IAmpSyncService ampSyncService,
        ILoggerFactory loggerFactory, ObservableLoggerProvider loggerProvider)
    {
        syncService = ampSyncService;

        MidiConnection = new MidiConnectionViewModel(katanaSession, ampSyncService, AppendStatus, loggerFactory.CreateLogger<MidiConnectionViewModel>());
        Diagnostics = new DiagnosticsViewModel(ampSyncService, loggerProvider, () => MidiConnection.IsConnected);
        Patch = new PatchViewModel(katanaSession, ampSyncService, () => MidiConnection.IsConnected, AppendStatus, loggerFactory.CreateLogger<PatchViewModel>());
        AmpEditor = new AmpEditorViewModel(katanaSession, katanaState, ampSyncService, AppendStatus, loggerFactory.CreateLogger<AmpEditorViewModel>());

        // syncService.Initialize(this);

        syncService.StatusMessages.Subscribe(msg => StatusMessage = msg);

        MidiConnection.WhenAnyValue(x => x.IsConnected)
            .Subscribe(v => Patch.CanWritePatch = v);

        AmpEditor.Initialize();
    }

    public MidiConnectionViewModel MidiConnection { get; }
    public DiagnosticsViewModel Diagnostics { get; }
    public PatchViewModel Patch { get; }
    public AmpEditorViewModel AmpEditor { get; }

    [Reactive] public string StatusMessage { get; set; } = "Ready to scan for MIDI devices.";

    public IStorageProvider? StorageProvider
    {
        set => Patch.StorageProvider = value;
    }

    public void Shutdown() => syncService.Shutdown();

    // ── IAmpSyncState explicit interface implementation ───────────────────────

    bool IAmpSyncState.IsConnected => MidiConnection.IsConnected;
    bool IAmpSyncState.ActiveWriteSync => AmpEditor.ActiveWriteSync;

    bool IAmpSyncState.SuppressChangeTracking
    {
        get => AmpEditor.SuppressChangeTracking;
        set => AmpEditor.SuppressChangeTracking = value;
    }

    bool IAmpSyncState.PatchLevelMappingVerified => false;

    int IAmpSyncState.PatchLevel
    {
        get => Patch.PatchLevel;
        set => Patch.PatchLevel = value;
    }

    IReadOnlyList<AmpControlViewModel> IAmpSyncState.AmpControls => AmpEditor.AmpControls;
    IReadOnlyList<IBasePedal> IAmpSyncState.PanelEffects => AmpEditor.PanelEffects;
    PedalFxViewModel IAmpSyncState.PedalFx => AmpEditor.PedalFx;
    PedalboardViewModel IAmpSyncState.Pedalboard => AmpEditor.Pedalboard;

    // ── Private helpers ───────────────────────────────────────────────────────

    private void AppendStatus(string msg) => StatusMessage = msg;
}
