using System;
using System.Collections.Generic;

using Avalonia.Platform.Storage;

using Kataka.App.Services;
using Kataka.Application.Katana;
using Kataka.Domain.KatanaState;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IAmpSyncState
{
    private readonly IAmpSyncService syncService;

    public MainWindowViewModel(IKatanaSession katanaSession, IKatanaState katanaState, IAmpSyncService ampSyncService)
    {
        syncService = ampSyncService;

        MidiConnection = new MidiConnectionViewModel(katanaSession, ampSyncService, AppendStatus, AppendLog);
        Diagnostics    = new DiagnosticsViewModel(ampSyncService, () => MidiConnection.IsConnected);
        Patch          = new PatchViewModel(katanaSession, ampSyncService, () => MidiConnection.IsConnected, AppendStatus, AppendLog);
        AmpEditor      = new AmpEditorViewModel(katanaSession, katanaState, ampSyncService, AppendStatus, AppendLog);

        syncService.Initialize(this);

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

    private void AppendLog(string msg)
    {
        var line = $"[{DateTimeOffset.Now:HH:mm:ss}] {msg}";
        Diagnostics.AppendRaw(line);
        Console.WriteLine(line);
    }
}
