using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Platform.Storage;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Kataka.App.Services;
using Kataka.Application.Katana;
using Kataka.Domain.KatanaState;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IAmpSyncContext
{
    private static readonly TimeSpan TapResetThreshold = TimeSpan.FromSeconds(2.5);
    private readonly IKatanaState _katanaState;
    private readonly Dictionary<string, string> inputPortIds = [];
    private readonly IKatanaSession katanaSession;
    private readonly Dictionary<string, string> outputPortIds = [];
    private readonly IAmpSyncService syncService;

    private DateTimeOffset? lastDelayTapAt;

    public MainWindowViewModel(IKatanaSession katanaSession, IKatanaState katanaState, IAmpSyncService ampSyncService)
    {
        this.katanaSession = katanaSession;
        _katanaState = katanaState;
        syncService = ampSyncService;

        // AmpControls are driven by domain state — VMs wrap AmpControlState directly.
        // var ampStatesByKey = katanaState.GetAmpControlsByKey();
        // foreach (var parameter in KatanaMkIIParameterCatalog.AmpEditorControls)
        // {
        //     AmpControls.Add(new AmpControlViewModel(ampStatesByKey[parameter.Key]));
        // }


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
                     new BoosterPedalViewModel(katanaState), new ModFxPedalViewModel("mod"), new ModFxPedalViewModel("fx"), new DelayPedalViewModel("delay", katanaState),
                     new DelayPedalViewModel("delay2", katanaState), new ReverbPedalViewModel(katanaState)
                 })
        {
            PanelEffects.Add(effectViewModel);
        }

        foreach (var channel in PanelChannels)
        {
            PanelChannelOptions.Add(new PanelChannelOptionViewModel(channel));
        }

        var panelEffectsByDefinitionKey = PanelEffects.ToDictionary(e => e.Definition.Key);
        Pedalboard = new PedalboardViewModel(panelEffectsByDefinitionKey, SelectedPanelChannel);

        syncService.Initialize(this);

        UpdatePanelChannelSelection();
        Pedalboard.Refresh();
    }

    public ObservableCollection<string> InputPorts { get; } = [];

    public ObservableCollection<string> OutputPorts { get; } = [];

    [ObservableProperty]
    public partial string DetectionMessage { get; set; } = "No scan has been run yet.";

    [ObservableProperty]
    public partial bool IsScanning { get; set; }

    [ObservableProperty]
    public partial bool IsKatanaDetected { get; set; }

    [ObservableProperty]
    public partial string? SelectedInputPort { get; set; }

    [ObservableProperty]
    public partial string? SelectedOutputPort { get; set; }

    [ObservableProperty]
    public partial string DiagnosticLog { get; set; } = "Diagnostic log ready.";

    [ObservableProperty]
    public partial string IdentityReply { get; set; } = "Identity request has not been run yet.";

    public ObservableCollection<PanelChannelOptionViewModel> PanelChannelOptions { get; } = [];

    public ObservableCollection<string> PanelChannels { get; } =
    [
        "Panel",
        "CH A1",
        "CH A2",
        "CH B1",
        "CH B2"
    ];

    public bool IsPatchLevelAvailable => false;

    public bool CanWritePatch => IsConnected;

    public static string[] AmpTypeOptions { get; } = ["ACOUSTIC", "CLEAN", "CRUNCH", "LEAD", "BROWN"];
    public static string[] CabinetResonanceOptions { get; } = ["LOW", "MIDDLE", "HIGH"];

    public IStorageProvider? StorageProvider { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "Ready to scan for MIDI devices.";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanWritePatch))]
    [NotifyCanExecuteChangedFor(nameof(WritePatchCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadPatchCommand))]
    [NotifyCanExecuteChangedFor(nameof(SavePatchCommand))]
    public partial bool IsConnected { get; set; }

    // //TODO: Remove
    // public ObservableCollection<AmpControlViewModel> AmpControls { get; } = [];

    public ObservableCollection<AmpControlViewModel> AmpControls { get; }
    public ObservableCollection<IBasePedal> PanelEffects { get; } = [];

    public PedalFxViewModel PedalFx { get; } = new();

    public PedalboardViewModel Pedalboard { get; } = null!;

    [ObservableProperty]
    public partial string AmpEditorStatus { get; set; } = "Amp editor values have not been read yet.";

    [ObservableProperty]
    public partial string PanelControlsStatus { get; set; } = "Panel controls have not been read yet.";

    [ObservableProperty]
    public partial string PedalControlsStatus { get; set; } = "Pedal controls have not been read yet.";

    [ObservableProperty]
    public partial string SelectedPanelChannel { get; set; } = "Panel";

    [ObservableProperty]
    public partial int PatchLevel { get; set; } = 100;

    [ObservableProperty]
    public partial string DelayTapStatus { get; set; } = "Delay time has not been read yet.";

    [ObservableProperty]
    public partial int? DelayTimeMs { get; set; }

    [ObservableProperty]
    public partial bool ActiveWriteSync { get; set; } = true;

    [ObservableProperty]
    public partial string SelectedAmpType { get; set; } = "CLEAN";

    [ObservableProperty]
    public partial string SelectedCabinetResonance { get; set; } = "MIDDLE";

    [ObservableProperty]
    public partial bool IsAmpVariation { get; set; } = false;

    // ── IAmpSyncContext explicit interface members ────────────────────────────

    bool IAmpSyncContext.SuppressChangeTracking { get; set; }

    bool IAmpSyncContext.PatchLevelMappingVerified => false;

    string[] IAmpSyncContext.AmpTypeOptions => AmpTypeOptions;

    string[] IAmpSyncContext.CabinetResonanceOptions => CabinetResonanceOptions;

    void IAmpSyncContext.Log(string message) => AppendLog(message);

    void IAmpSyncContext.ApplyPedalValue(string key, byte value) => ApplyPedalValue(key, value);

    void IAmpSyncContext.ResetDelayTap() => lastDelayTapAt = null;

    partial void OnIsAmpVariationChanged(bool value)
    {
        // Write to domain — service subscribes to domain.ValueChanged and queues the write.
        _katanaState.AmpVariation.Value = value ? 1 : 0;
    }

    partial void OnSelectedAmpTypeChanged(string value)
    {
        var idx = Array.IndexOf(AmpTypeOptions, value);
        if (idx < 0) return;
        _katanaState.AmpType.Value = idx;
    }

    partial void OnSelectedCabinetResonanceChanged(string value)
    {
        var idx = Array.IndexOf(CabinetResonanceOptions, value);
        if (idx < 0) return;
        _katanaState.CabinetResonance.Value = idx;
    }

    partial void OnSelectedPanelChannelChanged(string value)
    {
        UpdatePanelChannelSelection();
        Pedalboard.SelectedChannel = value;
        syncService.TrackPanelChannelChange(value);
    }

    partial void OnActiveWriteSyncChanged(bool value)
    {
        AppendLog($"Active write sync {(value ? "enabled" : "disabled")}.");
        syncService.UpdateWriteSyncTimer();
    }

    partial void OnIsConnectedChanged(bool value) => syncService.OnConnectionChanged(value);

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
            Math.Round((now - lastDelayTapAt.Value).TotalMilliseconds),
            1,
            2000);
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

    [RelayCommand(CanExecute = nameof(CanWritePatch))]
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
            // Slot 0 = write to current temp buffer / active patch location (patch 0)
            await katanaSession.WriteBlockAsync(
                KatanaMkIIParameterCatalog.PatchWriteAddress,
                [0x00, 0x00]);
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

    [RelayCommand(CanExecute = nameof(CanWritePatch))]
    private async Task LoadPatchAsync()
    {
        if (StorageProvider is null)
        {
            StatusMessage = "File dialog not available.";
            return;
        }

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Patch File",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Boss Tone Studio Patch")
                {
                    Patterns = ["*.tsl"]
                },
                new FilePickerFileType("All Files")
                {
                    Patterns = ["*"]
                }
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
            {
                json = await reader.ReadToEndAsync();
            }

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

    [RelayCommand(CanExecute = nameof(CanWritePatch))]
    private async Task SavePatchAsync()
    {
        if (StorageProvider is null)
        {
            StatusMessage = "File dialog not available.";
            return;
        }

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Patch File",
            SuggestedFileName = "patch.tsl",
            DefaultExtension = "tsl",
            FileTypeChoices =
            [
                new FilePickerFileType("Boss Tone Studio Patch")
                {
                    Patterns = ["*.tsl"]
                }
            ]
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

            foreach (var port in ports.Where(port => port.Direction == MidiPortDirection.Input).OrderBy(port => port.Name))
            {
                InputPorts.Add(port.Name);
                inputPortIds[port.Name] = port.Id;
                AppendLog($"Input: {port.Name}");
            }

            foreach (var port in ports.Where(port => port.Direction == MidiPortDirection.Output).OrderBy(port => port.Name))
            {
                OutputPorts.Add(port.Name);
                outputPortIds[port.Name] = port.Id;
                AppendLog($"Output: {port.Name}");
            }

            var katanaInput = ports.FirstOrDefault(port =>
                port.Direction == MidiPortDirection.Input &&
                port.Name.Contains("katana", StringComparison.OrdinalIgnoreCase));
            var katanaOutput = ports.FirstOrDefault(port =>
                port.Direction == MidiPortDirection.Output &&
                port.Name.Contains("katana", StringComparison.OrdinalIgnoreCase));

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
            if (!inputPortIds.TryGetValue(SelectedInputPort, out var inputPortId)) throw new InvalidOperationException($"Input port '{SelectedInputPort}' is not available.");

            if (!outputPortIds.TryGetValue(SelectedOutputPort, out var outputPortId)) throw new InvalidOperationException($"Output port '{SelectedOutputPort}' is not available.");

            await katanaSession.ConnectAsync(inputPortId, outputPortId);
            IsConnected = true;

            // Subscribe to amp push notifications so live parameter changes update the UI
            // without a poll round-trip. Builds the address→action lookup.
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

            // StatusMessage = (ampLoaded, panelLoaded, pedalLoaded) switch
            // {
            //     (true, true, true) => "MIDI ports opened and the current amp, panel, and pedal state was loaded.",
            //     (true, true, false) => "MIDI ports opened. Amp and panel state loaded, but pedal refresh failed.",
            //     (true, false, true) => "MIDI ports opened. Amp and pedal state loaded, but panel refresh failed.",
            //     (false, true, true) => "MIDI ports opened. Panel and pedal state loaded, but amp refresh failed.",
            //     _ => "MIDI ports opened, but part of the initial state refresh failed."
            // };
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

        // Unsubscribe from push notifications before disconnecting.
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
    private async Task ReadAmpControlsAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before reading amp controls.";
            return;
        }

        await syncService.TryRefreshAmpStateAsync();
    }

    // [RelayCommand]
    // private async Task WriteAmpControlsAsync()
    // {
    //     if (!katanaSession.IsConnected)
    //     {
    //         StatusMessage = "Connect to a MIDI port before writing amp controls.";
    //         return;
    //     }
    //
    //     try
    //     {
    //         AppendLog("Writing Katana amp editor controls.");
    //         var mismatches = new List<string>();
    //
    //         foreach (var control in AmpControls)
    //         {
    //             var requestedValue = Math.Clamp(control.Value, control.Minimum, control.Maximum);
    //             if (requestedValue != control.Value)
    //             {
    //                 control.Value = requestedValue;
    //                 AppendLog($"Clamped {control.DisplayName} to {requestedValue}.");
    //             }
    //
    //             AppendLog($"Writing {control.DisplayName} = {requestedValue}.");
    //             var confirmedValue = await katanaSession.WriteParameterAsync(control.Parameter, requestedValue);
    //             control.Value = confirmedValue;
    //             AppendLog($"{control.DisplayName} confirmed at {confirmedValue}.");
    //
    //             if (confirmedValue != requestedValue) mismatches.Add($"{control.DisplayName} ({requestedValue}->{confirmedValue})");
    //         }
    //
    //         StatusMessage = mismatches.Count == 0
    //             ? "Amp editor controls updated successfully."
    //             : "Amp editor write completed, but some read-back values differed.";
    //         AmpEditorStatus = mismatches.Count == 0
    //             ? "Amp editor values were written and confirmed."
    //             : $"Read-back mismatches: {string.Join(", ", mismatches)}";
    //     }
    //     catch (Exception ex)
    //     {
    //         AmpEditorStatus = "Amp editor write failed.";
    //         StatusMessage = ex.Message;
    //         AppendLog("Amp editor write failed.");
    //         AppendLog(ex.ToString());
    //         Console.Error.WriteLine(ex);
    //     }
    // }

    [RelayCommand]
    private async Task ReadPanelControlsAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before reading panel controls.";
            return;
        }

        await syncService.TryRefreshAmpStateAsync();
    }

    [RelayCommand]
    private async Task ReadPedalControlsAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before reading pedal controls.";
            return;
        }

        await syncService.TryRefreshAmpStateAsync();
    }

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

            var channel = IAmpSyncContext.ParsePanelChannelDisplay(SelectedPanelChannel);
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
                        effect.Definition.TypeParameter,
                        requestedType);
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

                if (confirmedValue != requestedValue) mismatches.Add($"{parameter.DisplayName} ({requestedValue}->{confirmedValue})");
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

    /// <summary>Call when the main window is closing to stop all background timers immediately.</summary>
    public void Shutdown() => syncService.Shutdown();

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
        {
            option.IsSelected = option.DisplayName == SelectedPanelChannel;
        }
    }

    private void ApplyPedalValue(string parameterKey, byte value)
    {
        switch (parameterKey)
        {
            case "pedal-fx-switch":
                PedalFx.IsEnabled = value != 0;
                break;
            case "pedal-fx-type":
                PedalFx.SelectedTypeOption = PedalFxViewModel.ToPedalTypeOption(value);
                break;
            case "pedal-fx-position":
                PedalFx.SelectedPositionOption = PedalFxViewModel.ToPositionOption(value);
                break;
            case "pedal-fx-wah-type":
                PedalFx.SelectedWahTypeOption = PedalFxViewModel.ToWahTypeOption(value);
                break;
            case "pedal-fx-wah-position":
                PedalFx.PedalPosition = value;
                break;
            case "pedal-fx-wah-min":
                PedalFx.PedalMinimum = value;
                break;
            case "pedal-fx-wah-max":
                PedalFx.PedalMaximum = value;
                break;
            case "pedal-fx-wah-effect-level":
                PedalFx.EffectLevel = value;
                break;
            case "pedal-fx-wah-direct-mix":
                PedalFx.DirectMix = value;
                break;
            case "pedal-fx-foot-volume":
                PedalFx.FootVolume = value;
                break;
        }
    }

    private static byte[] EncodeDelayTime(int milliseconds)
    {
        var clamped = Math.Clamp(milliseconds, 1, 2000);
        return [(byte)(clamped >> 7 & 0x0F), (byte)(clamped & 0x7F)];
    }

    private static int DecodeDelayTime(IReadOnlyList<byte> data)
    {
        if (data.Count != 2) throw new ArgumentException("Delay time data must contain exactly 2 bytes.", nameof(data));

        return (data[0] & 0x0F) << 7 | data[1] & 0x7F;
    }
}
