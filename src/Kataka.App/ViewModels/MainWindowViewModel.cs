using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;

using Avalonia.Platform.Storage;

using CommunityToolkit.Mvvm.Input;

using Kataka.App.Services;
using Kataka.Application.Katana;
using Kataka.Domain.KatanaState;
using Kataka.Domain.Midi;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IAmpSyncState

{
    private static readonly TimeSpan TapResetThreshold = TimeSpan.FromSeconds(2.5);
    private readonly IKatanaState _katanaState;
    private readonly Dictionary<string, string> inputPortIds = [];
    private readonly IKatanaSession katanaSession;
    private readonly Dictionary<string, string> outputPortIds = [];
    private readonly IAmpSyncService syncService;
    private readonly CompositeDisposable _disposables = new();

    private DateTimeOffset? lastDelayTapAt;

    public MainWindowViewModel(IKatanaSession katanaSession, IKatanaState katanaState, IAmpSyncService ampSyncService)
    {
        this.katanaSession = katanaSession;
        _katanaState = katanaState;
        syncService = ampSyncService;

        // Subscribe to domain state changes that drive the panel-level VM properties.
        katanaState.AmpType.ValueChanged += () =>
        {
            var idx = katanaState.AmpType.Value;
            if (idx < AmpTypeOptions.Length) SelectedAmpType = AmpTypeOptions[idx];
        };
        katanaState.CabinetResonance.ValueChanged += () =>
        {
            var idx = katanaState.CabinetResonance.Value;
            if (idx < CabinetResonanceOptions.Length) SelectedCabinetResonance = CabinetResonanceOptions[idx];
        };
        katanaState.AmpVariation.ValueChanged += () =>
            IsAmpVariation = katanaState.AmpVariation.Value != 0;

        foreach (var effectViewModel in new IBasePedal[]
                 {
                     new BoosterPedalViewModel(katanaState),
                     new ModFxPedalViewModel("mod"),
                     new ModFxPedalViewModel("fx"),
                     new DelayPedalViewModel("delay", katanaState),
                     new DelayPedalViewModel("delay2", katanaState),
                     new ReverbPedalViewModel(katanaState)
                 })
        {
            PanelEffects.Add(effectViewModel);
        }

        foreach (var channel in PanelChannels)
            PanelChannelOptions.Add(new PanelChannelOptionViewModel(channel));

        var panelEffectsByDefinitionKey = PanelEffects.ToDictionary(e => e.Definition.Key);
        Pedalboard = new PedalboardViewModel(panelEffectsByDefinitionKey, SelectedPanelChannel);

        syncService.Initialize(this);

        // ── Subscribe to service observable outputs ──────────────────────────

        syncService.StatusMessages
            .Subscribe(msg => StatusMessage = msg);


        syncService.LogMessages
            .Subscribe(line =>
            {
                DiagnosticLog = DiagnosticLog == "Diagnostic log ready."
                    ? line
                    : $"{DiagnosticLog}{Environment.NewLine}{line}";
            });


        syncService.PanelChannelPushed
            .Subscribe(displayName =>
            {
                SuppressChangeTracking = true;
                SelectedPanelChannel = displayName;
                SuppressChangeTracking = false;
            });


        syncService.ReadCompleted
            .Subscribe(meta =>
            {
                if (meta.AmpEditorStatus.Length > 0) AmpEditorStatus = meta.AmpEditorStatus;
                if (meta.PanelControlsStatus.Length > 0) PanelControlsStatus = meta.PanelControlsStatus;
                if (meta.PedalControlsStatus.Length > 0) PedalControlsStatus = meta.PedalControlsStatus;

                if (meta.DelayTimeLoaded && meta.DelayTimeMs.HasValue)
                {
                    DelayTimeMs = meta.DelayTimeMs;
                    DelayTapStatus = $"Delay time loaded: {meta.DelayTimeMs} ms.";
                }
                else if (meta.PanelControlsStatus.Length > 0 && !meta.DelayTimeLoaded)
                {
                    // Panel read completed but delay time wasn't available — reset tap state.
                    lastDelayTapAt = null;
                }
            });


        // ── WhenAnyValue side-effects (replaces partial void OnXxxChanged) ───

        this.WhenAnyValue(x => x.IsConnected)
            .Subscribe(v => syncService.OnConnectionChanged(v));


        this.WhenAnyValue(x => x.ActiveWriteSync)
            .Subscribe(v =>
            {
                AppendLog($"Active write sync {(v ? "enabled" : "disabled")}.");
                syncService.UpdateWriteSyncTimer();
            });


        this.WhenAnyValue(x => x.SelectedAmpType)
            .Subscribe(v =>
            {
                var idx = Array.IndexOf(AmpTypeOptions, v);
                if (idx < 0) return;
                _katanaState.AmpType.Value = idx;
            });


        this.WhenAnyValue(x => x.SelectedCabinetResonance)
            .Subscribe(v =>
            {
                var idx = Array.IndexOf(CabinetResonanceOptions, v);
                if (idx < 0) return;
                _katanaState.CabinetResonance.Value = idx;
            });


        this.WhenAnyValue(x => x.IsAmpVariation)
            .Subscribe(v => _katanaState.AmpVariation.Value = v ? 1 : 0);


        this.WhenAnyValue(x => x.SelectedPanelChannel)
            .Subscribe(v =>
            {
                UpdatePanelChannelSelection();
                Pedalboard.SelectedChannel = v;
                syncService.TrackPanelChannelChange(v);
            });


        UpdatePanelChannelSelection();
        Pedalboard.Refresh();
    }

    public ObservableCollection<string> InputPorts { get; } = [];
    public ObservableCollection<string> OutputPorts { get; } = [];

    [Reactive] public string DetectionMessage { get; set; } = "No scan has been run yet.";
    [Reactive] public bool IsScanning { get; set; }
    [Reactive] public bool IsKatanaDetected { get; set; }
    [Reactive] public string? SelectedInputPort { get; set; }
    [Reactive] public string? SelectedOutputPort { get; set; }
    [Reactive] public string DiagnosticLog { get; set; } = "Diagnostic log ready.";
    [Reactive] public string IdentityReply { get; set; } = "Identity request has not been run yet.";

    public ObservableCollection<PanelChannelOptionViewModel> PanelChannelOptions { get; } = [];
    public ObservableCollection<string> PanelChannels { get; } =
    [
        "Panel", "CH A1", "CH A2", "CH B1", "CH B2"
    ];

    public bool IsPatchLevelAvailable => false;
    public bool CanWritePatch => IsConnected;

    public static string[] AmpTypeOptions { get; } = ["ACOUSTIC", "CLEAN", "CRUNCH", "LEAD", "BROWN"];
    public static string[] CabinetResonanceOptions { get; } = ["LOW", "MIDDLE", "HIGH"];

    public IStorageProvider? StorageProvider { get; set; }

    [Reactive] public string StatusMessage { get; set; } = "Ready to scan for MIDI devices.";

    [Reactive] public bool IsConnected { get; set; }

    public ObservableCollection<AmpControlViewModel> AmpControls { get; } = [];
    public ObservableCollection<IBasePedal> PanelEffects { get; } = [];
    public PedalFxViewModel PedalFx { get; } = new();
    public PedalboardViewModel Pedalboard { get; }

    [Reactive] public string AmpEditorStatus { get; set; } = "Amp editor values have not been read yet.";
    [Reactive] public string PanelControlsStatus { get; set; } = "Panel controls have not been read yet.";
    [Reactive] public string PedalControlsStatus { get; set; } = "Pedal controls have not been read yet.";
    [Reactive] public string SelectedPanelChannel { get; set; } = "Panel";
    [Reactive] public int PatchLevel { get; set; } = 100;
    [Reactive] public string DelayTapStatus { get; set; } = "Delay time has not been read yet.";
    [Reactive] public int? DelayTimeMs { get; set; }
    [Reactive] public bool ActiveWriteSync { get; set; } = true;
    [Reactive] public string SelectedAmpType { get; set; } = "CLEAN";
    [Reactive] public string SelectedCabinetResonance { get; set; } = "MIDDLE";
    [Reactive] public bool IsAmpVariation { get; set; } = false;

    // ── IAmpSyncState explicit interface members ──────────────────────────────

    bool IAmpSyncState.SuppressChangeTracking { get; set; }
    // Expose SuppressChangeTracking internally for the PanelChannelPushed subscription.
    private bool SuppressChangeTracking
    {
        get => ((IAmpSyncState)this).SuppressChangeTracking;
        set => ((IAmpSyncState)this).SuppressChangeTracking = value;
    }

    bool IAmpSyncState.PatchLevelMappingVerified => false;

    IReadOnlyList<AmpControlViewModel> IAmpSyncState.AmpControls => AmpControls;
    IReadOnlyList<IBasePedal> IAmpSyncState.PanelEffects => PanelEffects;

    [RelayCommand]
    private void SelectPanelChannel(string? channel)
    {
        if (!string.IsNullOrWhiteSpace(channel)) SelectedPanelChannel = channel;
    }

    [RelayCommand]
    private async Task TapDelayAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before tapping delay time.";
            return;
        }

        var now = DateTimeOffset.Now;
        if (lastDelayTapAt is null || now - lastDelayTapAt.Value > TapResetThreshold)
        {
            lastDelayTapAt = now;
            DelayTapStatus = "First tap registered. Tap again to set the delay time.";
            StatusMessage = "Waiting for the second tap to calculate delay time.";
            AppendLog("Registered first delay tap.");
            return;
        }

        var tappedDelayTime = (int)Math.Clamp(
            Math.Round((now - lastDelayTapAt.Value).TotalMilliseconds), 1, 2000);
        lastDelayTapAt = now;

        try
        {
            AppendLog($"Writing tapped delay time: {tappedDelayTime} ms.");
            await katanaSession.WriteBlockAsync(
                KatanaMkIIParameterCatalog.DelayTimeAddress,
                EncodeDelayTime(tappedDelayTime));
            DelayTimeMs = tappedDelayTime;
            DelayTapStatus = $"Delay time tapped to {tappedDelayTime} ms.";
            StatusMessage = "Delay time updated from tap tempo.";
        }
        catch (Exception ex)
        {
            DelayTapStatus = "Delay tap write failed.";
            StatusMessage = ex.Message;
            AppendLog("Delay tap write failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
    }

    [RelayCommand]
    private async Task WritePatchAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before writing a patch.";
            return;
        }

        try
        {
            AppendLog("Sending WRITE PATCH command to Katana.");
            await katanaSession.WriteBlockAsync(KatanaMkIIParameterCatalog.PatchWriteAddress, [0x00, 0x00]);
            StatusMessage = "Patch written to Katana.";
            AppendLog("WRITE PATCH command sent.");
        }
        catch (Exception ex)
        {
            StatusMessage = "Patch write failed.";
            AppendLog("Patch write command failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
    }

    [RelayCommand]
    private async Task LoadPatchAsync()
    {
        if (StorageProvider is null) { StatusMessage = "File dialog not available."; return; }

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Patch File",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Boss Tone Studio Patch") { Patterns = ["*.tsl"] },
                new FilePickerFileType("All Files") { Patterns = ["*"] }
            ]
        });

        if (files.Count == 0) return;

        var file = files[0];
        try
        {
            AppendLog($"Loading patch from {file.Name}...");

            string json;
            await using (var stream = await file.OpenReadAsync())
            using (var reader = new StreamReader(stream))
                json = await reader.ReadToEndAsync();

            var patch = TslPatchSerializer.Deserialize(json);
            AppendLog($"Patch '{patch.Name}' parsed — {patch.Blocks.Count} block(s). Sending to amp...");

            await katanaSession.LoadPatchAsync(patch);
            AppendLog("Patch loaded. Refreshing display...");
            await syncService.TryRefreshAmpStateAsync();
            StatusMessage = $"Patch '{patch.Name}' loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Patch load failed.";
            AppendLog($"Patch load failed: {ex.Message}");
            Console.Error.WriteLine(ex);
        }
    }

    [RelayCommand]
    private async Task SavePatchAsync()
    {
        if (StorageProvider is null) { StatusMessage = "File dialog not available."; return; }

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Patch File",
            SuggestedFileName = "patch.tsl",
            DefaultExtension = "tsl",
            FileTypeChoices = [new FilePickerFileType("Boss Tone Studio Patch") { Patterns = ["*.tsl"] }]
        });

        if (file is null) return;

        try
        {
            AppendLog("Reading all patch blocks from amp...");
            var patchName = Path.GetFileNameWithoutExtension(file.Name);
            var patch = await katanaSession.ReadCurrentPatchAsync(patchName);
            var json = TslPatchSerializer.Serialize(patch);

            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);

            AppendLog($"Patch '{patch.Name}' saved to {file.Name}.");
            StatusMessage = $"Patch saved as '{file.Name}'.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Patch save failed.";
            AppendLog($"Patch save failed: {ex.Message}");
            Console.Error.WriteLine(ex);
        }
    }

    [RelayCommand]
    private async Task ScanAsync()
    {
        await katanaSession.DisconnectAsync();
        IsConnected = false;
        IsScanning = true;
        StatusMessage = "Scanning MIDI ports...";
        DetectionMessage = "Looking for Katana input/output ports.";
        IsKatanaDetected = false;
        AppendLog("Starting MIDI port scan.");
        SelectedInputPort = null;
        SelectedOutputPort = null;
        InputPorts.Clear();
        OutputPorts.Clear();
        inputPortIds.Clear();
        outputPortIds.Clear();

        try
        {
            var ports = await katanaSession.ListPortsAsync();
            AppendLog($"Port scan returned {ports.Count} total port(s).");

            foreach (var port in ports.Where(p => p.Direction == MidiPortDirection.Input).OrderBy(p => p.Name))
            {
                InputPorts.Add(port.Name);
                inputPortIds[port.Name] = port.Id;
                AppendLog($"Input: {port.Name}");
            }

            foreach (var port in ports.Where(p => p.Direction == MidiPortDirection.Output).OrderBy(p => p.Name))
            {
                OutputPorts.Add(port.Name);
                outputPortIds[port.Name] = port.Id;
                AppendLog($"Output: {port.Name}");
            }

            var katanaInput = ports.FirstOrDefault(p =>
                p.Direction == MidiPortDirection.Input &&
                p.Name.Contains("katana", StringComparison.OrdinalIgnoreCase));
            var katanaOutput = ports.FirstOrDefault(p =>
                p.Direction == MidiPortDirection.Output &&
                p.Name.Contains("katana", StringComparison.OrdinalIgnoreCase));

            if (katanaInput is not null && katanaOutput is not null)
            {
                IsKatanaDetected = true;
                SelectedInputPort = katanaInput.Name;
                SelectedOutputPort = katanaOutput.Name;
                DetectionMessage = $"Katana detected: {katanaInput.Name} / {katanaOutput.Name}";
                StatusMessage = "Katana-style MIDI ports found. You can connect to the selected pair.";
                AppendLog("Katana-style port pair auto-selected.");
                return;
            }

            DetectionMessage = "No Katana MIDI port pair was detected.";
            StatusMessage = $"Scan complete. Found {InputPorts.Count} input port(s) and {OutputPorts.Count} output port(s).";
            AppendLog("No Katana-style port pair found.");
        }
        catch (Exception ex)
        {
            DetectionMessage = "MIDI scan failed.";
            StatusMessage = ex.Message;
            AppendLog("MIDI scan failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
        finally
        {
            IsScanning = false;
            AppendLog("MIDI port scan finished.");
        }
    }

    [RelayCommand]
    private async Task ConnectAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedInputPort) || string.IsNullOrWhiteSpace(SelectedOutputPort))
        {
            StatusMessage = "Select both an input port and an output port before connecting.";
            return;
        }

        try
        {
            AppendLog($"Opening input '{SelectedInputPort}' and output '{SelectedOutputPort}'.");
            if (!inputPortIds.TryGetValue(SelectedInputPort, out var inputPortId))
                throw new InvalidOperationException($"Input port '{SelectedInputPort}' is not available.");
            if (!outputPortIds.TryGetValue(SelectedOutputPort, out var outputPortId))
                throw new InvalidOperationException($"Output port '{SelectedOutputPort}' is not available.");

            await katanaSession.ConnectAsync(inputPortId, outputPortId);
            IsConnected = true;
            syncService.Activate();

            var looksLikeKatana =
                SelectedInputPort.Contains("katana", StringComparison.OrdinalIgnoreCase) &&
                SelectedOutputPort.Contains("katana", StringComparison.OrdinalIgnoreCase);

            DetectionMessage = looksLikeKatana
                ? $"Connected to Katana-style ports: {SelectedInputPort} / {SelectedOutputPort}"
                : $"Connected to selected MIDI ports: {SelectedInputPort} / {SelectedOutputPort}";
            StatusMessage = "MIDI ports opened successfully.";
            AppendLog("MIDI ports opened successfully.");

            AppendLog("Auto-loading amp, panel, and pedal state.");
            await syncService.TryRefreshAmpStateAsync();
        }
        catch (Exception ex)
        {
            IsConnected = false;
            DetectionMessage = "Connection failed.";
            StatusMessage = ex.Message;
            AppendLog("Connection failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
    }

    [RelayCommand]
    private async Task DisconnectAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "No MIDI connection is currently open.";
            return;
        }

        syncService.Deactivate();
        await katanaSession.DisconnectAsync();
        IsConnected = false;
        StatusMessage = "Disconnected from the selected MIDI ports.";
        DetectionMessage = "Connection closed.";
        AppendLog("MIDI connection closed.");
    }

    [RelayCommand]
    private async Task RequestIdentityAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before requesting identity.";
            return;
        }

        try
        {
            AppendLog("Sending universal device identity request.");
            var reply = await katanaSession.RequestIdentityAsync();
            IdentityReply = reply.ToHexString();
            DetectionMessage = UniversalDeviceIdentity.IsIdentityReply(reply)
                ? "Identity reply received."
                : "A SysEx reply was received, but it did not match the standard identity pattern.";
            StatusMessage = "Identity request completed.";
            AppendLog($"Identity reply: {IdentityReply}");
        }
        catch (Exception ex)
        {
            IdentityReply = "Identity request failed.";
            DetectionMessage = "Identity request failed.";
            StatusMessage = ex.Message;
            AppendLog("Identity request failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
    }

    [RelayCommand]
    private async Task ReadAmpControlsAsync() =>
        await (katanaSession.IsConnected
            ? syncService.TryRefreshAmpStateAsync()
            : Task.FromResult(false));

    [RelayCommand]
    private async Task ReadPanelControlsAsync() =>
        await (katanaSession.IsConnected
            ? syncService.TryRefreshAmpStateAsync()
            : Task.FromResult(false));

    [RelayCommand]
    private async Task ReadPedalControlsAsync() =>
        await (katanaSession.IsConnected
            ? syncService.TryRefreshAmpStateAsync()
            : Task.FromResult(false));

    [RelayCommand]
    private async Task WritePanelControlsAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before writing panel controls.";
            return;
        }

        try
        {
            AppendLog("Writing Katana panel controls.");
            var channel = IAmpSyncState.ParsePanelChannelDisplay(SelectedPanelChannel);
            await katanaSession.SelectPanelChannelAsync(channel);
            AppendLog($"Selected panel channel: {SelectedPanelChannel}");
            var patchLevelWritten = await syncService.TryWritePatchLevelAsync();

            foreach (var effect in PanelEffects)
            {
                var confirmedValue = await katanaSession.WriteParameterAsync(
                    effect.Definition.SwitchParameter,
                    effect.IsEnabled ? (byte)1 : (byte)0);
                effect.IsEnabled = confirmedValue != 0;
                AppendLog($"{effect.DisplayName} confirmed {(effect.IsEnabled ? "On" : "Off")}.");

                if (effect.Definition.TypeParameter is not null &&
                    effect.TryGetTypeValue(effect.SelectedTypeOption, out var requestedType))
                {
                    var confirmedType = await katanaSession.WriteParameterAsync(
                        effect.Definition.TypeParameter, requestedType);
                    effect.SelectedTypeOption = effect.ToTypeOption(confirmedType);
                    AppendLog($"{effect.DisplayName} type confirmed at {effect.SelectedTypeOption}.");
                }
            }

            StatusMessage = "Panel controls updated successfully.";
            PanelControlsStatus = patchLevelWritten
                ? "Panel channel, patch level, effect toggles, and effect types were written and confirmed."
                : "Panel channel, effect toggles, and effect types were written and confirmed. Patch level mapping is still pending.";
        }
        catch (Exception ex)
        {
            PanelControlsStatus = "Panel control write failed.";
            StatusMessage = ex.Message;
            AppendLog("Panel control write failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
    }

    [RelayCommand]
    private async Task WritePedalControlsAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before writing pedal controls.";
            return;
        }

        try
        {
            AppendLog("Writing Katana pedal controls.");
            var mismatches = new List<string>();

            foreach (var parameter in PedalFx.GetManualWriteParameters())
            {
                if (!PedalFx.TryGetCurrentValue(parameter.Key, out var requestedValue)) continue;

                AppendLog($"Writing {parameter.DisplayName} = {requestedValue}.");
                var confirmedValue = await katanaSession.WriteParameterAsync(parameter, requestedValue);
                ApplyPedalValue(parameter.Key, confirmedValue);
                AppendLog($"{parameter.DisplayName} confirmed at {confirmedValue}.");

                if (confirmedValue != requestedValue)
                    mismatches.Add($"{parameter.DisplayName} ({requestedValue}->{confirmedValue})");
            }

            StatusMessage = mismatches.Count == 0
                ? "Pedal controls updated successfully."
                : "Pedal write completed, but some read-back values differed.";
            PedalControlsStatus = mismatches.Count == 0
                ? "Pedal FX values were written and confirmed."
                : $"Pedal read-back mismatches: {string.Join(", ", mismatches)}";
        }
        catch (Exception ex)
        {
            PedalControlsStatus = "Pedal control write failed.";
            StatusMessage = ex.Message;
            AppendLog("Pedal control write failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
    }

    public void Shutdown()
    {
        syncService.Shutdown();
        _disposables.Dispose();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void AppendLog(string message)
    {
        var line = $"[{DateTimeOffset.Now:HH:mm:ss}] {message}";
        DiagnosticLog = DiagnosticLog == "Diagnostic log ready."
            ? line
            : $"{DiagnosticLog}{Environment.NewLine}{line}";
        Console.WriteLine(line);
    }

    private void UpdatePanelChannelSelection()
    {
        foreach (var option in PanelChannelOptions)
            option.IsSelected = option.DisplayName == SelectedPanelChannel;
    }

    private void ApplyPedalValue(string parameterKey, byte value)
    {
        switch (parameterKey)
        {
            case "pedal-fx-switch": PedalFx.IsEnabled = value != 0; break;
            case "pedal-fx-type": PedalFx.SelectedTypeOption = PedalFxViewModel.ToPedalTypeOption(value); break;
            case "pedal-fx-position": PedalFx.SelectedPositionOption = PedalFxViewModel.ToPositionOption(value); break;
            case "pedal-fx-wah-type": PedalFx.SelectedWahTypeOption = PedalFxViewModel.ToWahTypeOption(value); break;
            case "pedal-fx-wah-position": PedalFx.PedalPosition = value; break;
            case "pedal-fx-wah-min": PedalFx.PedalMinimum = value; break;
            case "pedal-fx-wah-max": PedalFx.PedalMaximum = value; break;
            case "pedal-fx-wah-effect-level": PedalFx.EffectLevel = value; break;
            case "pedal-fx-wah-direct-mix": PedalFx.DirectMix = value; break;
            case "pedal-fx-foot-volume": PedalFx.FootVolume = value; break;
        }
    }

    private static byte[] EncodeDelayTime(int milliseconds)
    {
        var clamped = Math.Clamp(milliseconds, 1, 2000);
        return [(byte)(clamped >> 7 & 0x0F), (byte)(clamped & 0x7F)];
    }
}
