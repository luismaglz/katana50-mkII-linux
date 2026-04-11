using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kataka.Application.Katana;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private static readonly TimeSpan TapResetThreshold = TimeSpan.FromSeconds(2.5);
    private static readonly TimeSpan WriteSyncDebounce = TimeSpan.FromMilliseconds(125);
    private static readonly TimeSpan ReadSyncInterval = TimeSpan.FromSeconds(4);
    private readonly IKatanaSession katanaSession;
    private readonly Dictionary<string, string> inputPortIds = [];
    private readonly Dictionary<string, string> outputPortIds = [];
    private readonly Dictionary<string, AmpControlViewModel> ampControlsByKey = [];
    private readonly Dictionary<string, PanelEffectViewModel> panelEffectsByKey = [];
    private readonly Dictionary<string, PanelEffectViewModel> panelEffectsByDefinitionKey = [];
    private readonly Dictionary<string, byte> pendingAmpWrites = [];
    private readonly Dictionary<string, bool> pendingPanelEffectWrites = [];
    private readonly Dictionary<string, byte> pendingPanelTypeWrites = [];
    private readonly Dictionary<string, byte> pendingPanelLevelWrites = [];
    private readonly Dictionary<string, byte> pendingPedalWrites = [];
    private byte? pendingAmpTypeWrite;
    private byte? pendingCabinetResonanceWrite;
    private readonly SemaphoreSlim syncOperationGate = new(1, 1);
    private readonly DispatcherTimer writeSyncTimer;
    private readonly DispatcherTimer readSyncTimer;
    private DateTimeOffset? lastDelayTapAt;
    private bool suppressChangeTracking;
    private int activeReadSyncPhase;
    private bool suspendActiveReadSync;
    private string? pendingPanelChannel;

    public MainWindowViewModel()
        : this(new KatanaSession(Kataka.Infrastructure.Midi.DefaultMidiTransport.Create()))
    {
    }

    public MainWindowViewModel(IKatanaSession katanaSession)
    {
        this.katanaSession = katanaSession;
        writeSyncTimer = new DispatcherTimer { Interval = WriteSyncDebounce };
        writeSyncTimer.Tick += async (_, _) =>
        {
            writeSyncTimer.Stop();
            await FlushPendingWritesAsync();

            if (HasPendingWrites())
            {
                UpdateWriteSyncTimer();
            }
        };
        readSyncTimer = new DispatcherTimer { Interval = ReadSyncInterval };
        readSyncTimer.Tick += async (_, _) =>
        {
            readSyncTimer.Stop();
            await RunActiveReadSyncCycleAsync();

            if (ShouldRunActiveReadSync())
            {
                UpdateReadSyncTimer();
            }
        };

        foreach (var parameter in KatanaMkIIParameterCatalog.AmpEditorControls)
        {
            var control = new AmpControlViewModel(parameter);
            control.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(AmpControlViewModel.Value))
                {
                    TrackAmpControlChange(control);
                }
            };

            AmpControls.Add(control);
            ampControlsByKey[parameter.Key] = control;
        }

        foreach (var effect in KatanaMkIIParameterCatalog.PanelEffects)
        {
            var effectViewModel = new PanelEffectViewModel(effect);
            effectViewModel.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(PanelEffectViewModel.IsEnabled))
                {
                    TrackPanelEffectChange(effectViewModel);
                }
                else if (args.PropertyName == nameof(PanelEffectViewModel.SelectedTypeOption))
                {
                    TrackPanelEffectTypeChange(effectViewModel);
                }
                else if (args.PropertyName == nameof(PanelEffectViewModel.Level))
                {
                    TrackPanelEffectLevelChange(effectViewModel);
                }

                RefreshPedalboard();
            };

            PanelEffects.Add(effectViewModel);
            panelEffectsByKey[effect.SwitchParameter.Key] = effectViewModel;
            panelEffectsByDefinitionKey[effect.Key] = effectViewModel;
        }

        foreach (var channel in PanelChannels)
        {
            PanelChannelOptions.Add(new PanelChannelOptionViewModel(channel));
        }

        PedalFx.PropertyChanged += (_, args) =>
        {
            TrackPedalChange(args.PropertyName);
            RefreshPedalboard();
        };

        UpdatePanelChannelSelection();
        RefreshPedalboard();
        UpdateWriteSyncTimer();
        UpdateReadSyncTimer();
    }

    public ObservableCollection<string> InputPorts { get; } = [];

    public ObservableCollection<string> OutputPorts { get; } = [];

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "Ready to scan for MIDI devices.";

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
    public partial bool IsConnected { get; set; }

    [ObservableProperty]
    public partial string DiagnosticLog { get; set; } = "Diagnostic log ready.";

    [ObservableProperty]
    public partial string IdentityReply { get; set; } = "Identity request has not been run yet.";

    public ObservableCollection<AmpControlViewModel> AmpControls { get; } = [];

    public ObservableCollection<PanelEffectViewModel> PanelEffects { get; } = [];

    public ObservableCollection<PanelChannelOptionViewModel> PanelChannelOptions { get; } = [];

    public PedalFxViewModel PedalFx { get; } = new();

    public ObservableCollection<PedalboardItemViewModel> PedalboardItems { get; } = [];

    public ObservableCollection<string> PanelChannels { get; } =
    [
        "Panel",
        "CH A1",
        "CH A2",
        "CH B1",
        "CH B2",
    ];

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

    public bool IsPatchLevelAvailable => PatchLevelMappingVerified;

    [ObservableProperty]
    public partial bool ActiveReadSync { get; set; } = true;

    [ObservableProperty]
    public partial bool ActiveWriteSync { get; set; } = true;

    public static string[] AmpTypeOptions { get; } = ["ACOUSTIC", "CLEAN", "CRUNCH", "LEAD", "BROWN"];
    public static string[] CabinetResonanceOptions { get; } = ["LOW", "MIDDLE", "HIGH"];

    [ObservableProperty]
    public partial string SelectedAmpType { get; set; } = "CLEAN";

    [ObservableProperty]
    public partial string SelectedCabinetResonance { get; set; } = "MIDDLE";

    partial void OnSelectedAmpTypeChanged(string value)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected) return;
        var idx = Array.IndexOf(AmpTypeOptions, value);
        if (idx < 0) return;
        pendingAmpTypeWrite = (byte)idx;
        AppendLog($"Queued panel sync: Amp Type -> {value}.");
    }

    partial void OnSelectedCabinetResonanceChanged(string value)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected) return;
        var idx = Array.IndexOf(CabinetResonanceOptions, value);
        if (idx < 0) return;
        pendingCabinetResonanceWrite = (byte)idx;
        AppendLog($"Queued panel sync: Cabinet Resonance -> {value}.");
    }

    [ObservableProperty]
    public partial string? SelectedPedalboardKey { get; set; }

    partial void OnSelectedPedalboardKeyChanged(string? value)
    {
        OnPropertyChanged(nameof(SelectedPanelEffectDetail));
        OnPropertyChanged(nameof(SelectedIsPedalFx));
        OnPropertyChanged(nameof(SelectedHasDetail));
        OnPropertyChanged(nameof(SelectedDetailTitle));
    }

    public PanelEffectViewModel? SelectedPanelEffectDetail =>
        SelectedPedalboardKey is not null &&
        panelEffectsByDefinitionKey.TryGetValue(SelectedPedalboardKey, out var vm) ? vm : null;

    public bool SelectedIsPedalFx =>
        string.Equals(SelectedPedalboardKey, "pedal-fx", StringComparison.Ordinal);

    public bool SelectedHasDetail => SelectedPanelEffectDetail is not null || SelectedIsPedalFx;

    public string SelectedDetailTitle =>
        SelectedPanelEffectDetail?.DisplayName ??
        (SelectedIsPedalFx ? "Pedal FX" : "Select a pedal in the chain below to edit its settings");

    [RelayCommand]
    private void SelectPedalboardItem(string? key)
    {
        SelectedPedalboardKey = string.Equals(key, SelectedPedalboardKey, StringComparison.Ordinal) ? null : key;
    }

    partial void OnSelectedPanelChannelChanged(string value)
    {
        UpdatePanelChannelSelection();
        RefreshPedalboard();

        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected)
        {
            return;
        }

        pendingPanelChannel = value;
        AppendLog($"Queued panel channel sync: {value}.");
        PauseActiveReadSync("queued writes are pending");
        UpdateWriteSyncTimer();
    }

    partial void OnActiveReadSyncChanged(bool value)
    {
        AppendLog($"Active read sync {(value ? "enabled" : "disabled")}.");
        UpdateReadSyncTimer();
    }

    partial void OnActiveWriteSyncChanged(bool value)
    {
        AppendLog($"Active write sync {(value ? "enabled" : "disabled")}.");
        UpdateWriteSyncTimer();
    }

    partial void OnIsConnectedChanged(bool value)
    {
        if (!value)
        {
            if (pendingAmpWrites.Count > 0 || pendingPanelEffectWrites.Count > 0 || pendingPanelTypeWrites.Count > 0 || pendingPanelLevelWrites.Count > 0 || pendingPedalWrites.Count > 0 || pendingPanelChannel is not null || pendingAmpTypeWrite.HasValue || pendingCabinetResonanceWrite.HasValue)
            {
                AppendLog($"Clearing pending sync queue after disconnect: {DescribePendingWrites()}.");
            }

            pendingAmpWrites.Clear();
            pendingPanelEffectWrites.Clear();
            pendingPanelTypeWrites.Clear();
            pendingPanelLevelWrites.Clear();
            pendingPedalWrites.Clear();
            pendingPanelChannel = null;
            pendingAmpTypeWrite = null;
            pendingCabinetResonanceWrite = null;
        }

        UpdateWriteSyncTimer();
        UpdateReadSyncTimer();
    }

    [RelayCommand]
    private void SelectPanelChannel(string? channel)
    {
        if (!string.IsNullOrWhiteSpace(channel))
        {
            SelectedPanelChannel = channel;
        }
    }

    [RelayCommand]
    private void TogglePedalboardItem(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        if (string.Equals(key, "pedal-fx", StringComparison.Ordinal))
        {
            PedalFx.IsEnabled = !PedalFx.IsEnabled;
            RefreshPedalboard();
            return;
        }

        var effect = PanelEffects.FirstOrDefault(item => string.Equals(item.Definition.Key, key, StringComparison.Ordinal));
        if (effect is null)
        {
            return;
        }

        effect.IsEnabled = !effect.IsEnabled;
        RefreshPedalboard();
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
            PauseActiveReadSync("tap delay write is in progress");
            AppendLog($"Writing tapped delay time: {tappedDelayTime} ms.");
            await katanaSession.WriteBlockAsync(
                KatanaMkIIParameterCatalog.DelayTimeAddress,
                EncodeDelayTime(tappedDelayTime));
            if (ActiveReadSync)
            {
                var confirmedDelayTime = DecodeDelayTime(
                    await katanaSession.ReadBlockAsync(KatanaMkIIParameterCatalog.DelayTimeAddress, 2));
                DelayTapStatus = $"Delay time tapped to {confirmedDelayTime} ms.";
                AppendLog($"Delay time confirmed at {confirmedDelayTime} ms.");
            }
            else
            {
                DelayTapStatus = $"Delay time tapped to {tappedDelayTime} ms.";
            }

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
        finally
        {
            ResumeActiveReadSync("tap delay write finished");
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
            suspendActiveReadSync = true;
            AppendLog($"Opening input '{SelectedInputPort}' and output '{SelectedOutputPort}'.");
            if (!inputPortIds.TryGetValue(SelectedInputPort, out var inputPortId))
            {
                throw new InvalidOperationException($"Input port '{SelectedInputPort}' is not available.");
            }

            if (!outputPortIds.TryGetValue(SelectedOutputPort, out var outputPortId))
            {
                throw new InvalidOperationException($"Output port '{SelectedOutputPort}' is not available.");
            }

            await katanaSession.ConnectAsync(inputPortId, outputPortId);
            IsConnected = true;

            var looksLikeKatana =
                SelectedInputPort.Contains("katana", StringComparison.OrdinalIgnoreCase) &&
                SelectedOutputPort.Contains("katana", StringComparison.OrdinalIgnoreCase);

            DetectionMessage = looksLikeKatana
                ? $"Connected to Katana-style ports: {SelectedInputPort} / {SelectedOutputPort}"
                : $"Connected to selected MIDI ports: {SelectedInputPort} / {SelectedOutputPort}";
            StatusMessage = "MIDI ports opened successfully.";
            AppendLog("MIDI ports opened successfully.");

            AppendLog("Auto-loading amp, panel, and pedal state.");
            var ampLoaded = await TryReadAmpControlsAsync();
            var panelLoaded = await TryReadPanelControlsAsync();
            var pedalLoaded = await TryReadPedalControlsAsync();

            StatusMessage = (ampLoaded, panelLoaded, pedalLoaded) switch
            {
                (true, true, true) => "MIDI ports opened and the current amp, panel, and pedal state was loaded.",
                (true, true, false) => "MIDI ports opened. Amp and panel state loaded, but pedal refresh failed.",
                (true, false, true) => "MIDI ports opened. Amp and pedal state loaded, but panel refresh failed.",
                (false, true, true) => "MIDI ports opened. Panel and pedal state loaded, but amp refresh failed.",
                _ => "MIDI ports opened, but part of the initial state refresh failed.",
            };
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
        finally
        {
            suspendActiveReadSync = false;
            UpdateReadSyncTimer();
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

        await TryReadAmpControlsAsync();
    }

    [RelayCommand]
    private async Task WriteAmpControlsAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before writing amp controls.";
            return;
        }

        try
        {
            PauseActiveReadSync("manual amp write is in progress");
            AppendLog("Writing Katana amp editor controls.");
            var mismatches = new List<string>();

            foreach (var control in AmpControls)
            {
                var requestedValue = Math.Clamp(control.Value, control.Minimum, control.Maximum);
                if (requestedValue != control.Value)
                {
                    control.Value = requestedValue;
                    AppendLog($"Clamped {control.DisplayName} to {requestedValue}.");
                }

                AppendLog($"Writing {control.DisplayName} = {requestedValue}.");
                var confirmedValue = await katanaSession.WriteParameterAsync(control.Parameter, (byte)requestedValue);
                control.Value = confirmedValue;
                AppendLog($"{control.DisplayName} confirmed at {confirmedValue}.");

                if (confirmedValue != requestedValue)
                {
                    mismatches.Add($"{control.DisplayName} ({requestedValue}->{confirmedValue})");
                }
            }

            StatusMessage = mismatches.Count == 0
                ? "Amp editor controls updated successfully."
                : "Amp editor write completed, but some read-back values differed.";
            AmpEditorStatus = mismatches.Count == 0
                ? "Amp editor values were written and confirmed."
                : $"Read-back mismatches: {string.Join(", ", mismatches)}";
        }
        catch (Exception ex)
        {
            AmpEditorStatus = "Amp editor write failed.";
            StatusMessage = ex.Message;
            AppendLog("Amp editor write failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
        finally
        {
            ResumeActiveReadSync("manual amp write finished");
        }
    }

    [RelayCommand]
    private async Task ReadPanelControlsAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before reading panel controls.";
            return;
        }

        await TryReadPanelControlsAsync();
    }

    [RelayCommand]
    private async Task ReadPedalControlsAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before reading pedal controls.";
            return;
        }

        await TryReadPedalControlsAsync();
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
            PauseActiveReadSync("manual panel write is in progress");
            AppendLog("Writing Katana panel controls.");

            var channel = ParsePanelChannelDisplay(SelectedPanelChannel);
            await katanaSession.SelectPanelChannelAsync(channel);
            AppendLog($"Selected panel channel: {SelectedPanelChannel}");
            var patchLevelWritten = await TryWritePatchLevelAsync();

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
        finally
        {
            ResumeActiveReadSync("manual panel write finished");
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
            PauseActiveReadSync("manual pedal write is in progress");
            AppendLog("Writing Katana pedal controls.");
            var mismatches = new List<string>();

            foreach (var parameter in PedalFx.GetManualWriteParameters())
            {
                if (!PedalFx.TryGetCurrentValue(parameter.Key, out var requestedValue))
                {
                    continue;
                }

                AppendLog($"Writing {parameter.DisplayName} = {requestedValue}.");
                var confirmedValue = await katanaSession.WriteParameterAsync(parameter, requestedValue);
                ApplyPedalValue(parameter.Key, confirmedValue);
                AppendLog($"{parameter.DisplayName} confirmed at {confirmedValue}.");

                if (confirmedValue != requestedValue)
                {
                    mismatches.Add($"{parameter.DisplayName} ({requestedValue}->{confirmedValue})");
                }
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
        finally
        {
            ResumeActiveReadSync("manual pedal write finished");
        }
    }

    private void AppendLog(string message)
    {
        var line = $"[{DateTimeOffset.Now:HH:mm:ss}] {message}";
        DiagnosticLog = DiagnosticLog == "Diagnostic log ready."
            ? line
            : $"{DiagnosticLog}{Environment.NewLine}{line}";
        Console.WriteLine(line);
    }

    private string DescribePendingWrites()
    {
        var parts = new List<string>();
        if (pendingAmpWrites.Count > 0)
        {
            parts.Add($"{pendingAmpWrites.Count} amp");
        }

        if (pendingPanelEffectWrites.Count > 0)
        {
            parts.Add($"{pendingPanelEffectWrites.Count} panel");
        }

        if (pendingPanelTypeWrites.Count > 0)
        {
            parts.Add($"{pendingPanelTypeWrites.Count} panel type");
        }

        if (pendingPanelLevelWrites.Count > 0)
        {
            parts.Add($"{pendingPanelLevelWrites.Count} panel level");
        }

        if (pendingAmpTypeWrite.HasValue)
        {
            parts.Add("amp type");
        }

        if (pendingCabinetResonanceWrite.HasValue)
        {
            parts.Add("cabinet resonance");
        }

        if (pendingPedalWrites.Count > 0)
        {
            parts.Add($"{pendingPedalWrites.Count} pedal");
        }

        if (pendingPanelChannel is not null)
        {
            parts.Add("channel");
        }

        return parts.Count == 0 ? "no pending changes" : string.Join(", ", parts);
    }

    private bool HasPendingWrites()
    {
        return pendingAmpWrites.Count > 0 ||
               pendingPanelEffectWrites.Count > 0 ||
               pendingPanelTypeWrites.Count > 0 ||
               pendingPanelLevelWrites.Count > 0 ||
               pendingAmpTypeWrite.HasValue ||
               pendingCabinetResonanceWrite.HasValue ||
               pendingPedalWrites.Count > 0 ||
               pendingPanelChannel is not null;
    }

    private bool ShouldRunActiveReadSync()
    {
        return ActiveReadSync && IsConnected && !suspendActiveReadSync && !HasPendingWrites();
    }

    private void PauseActiveReadSync(string reason)
    {
        if (!suspendActiveReadSync)
        {
            AppendLog($"Active read sync paused: {reason}.");
        }

        suspendActiveReadSync = true;
        UpdateReadSyncTimer();
    }

    private void ResumeActiveReadSync(string reason)
    {
        var wasSuspended = suspendActiveReadSync;
        suspendActiveReadSync = false;
        UpdateReadSyncTimer();

        if (wasSuspended && ActiveReadSync && IsConnected)
        {
            AppendLog($"Active read sync resumed: {reason}.");
        }
    }

    private string DescribeAmpKeys(IEnumerable<string> keys)
    {
        return string.Join(", ", keys
            .Distinct(StringComparer.Ordinal)
            .Select(key => ampControlsByKey[key].DisplayName));
    }

    private string DescribePanelKeys(IEnumerable<string> keys)
    {
        return string.Join(", ", keys
            .Distinct(StringComparer.Ordinal)
            .Select(key => panelEffectsByKey[key].DisplayName));
    }

    private string DescribePedalKeys(IEnumerable<string> keys)
    {
        return string.Join(", ", keys
            .Distinct(StringComparer.Ordinal)
            .Select(key => PedalFx.GetParameter(key).DisplayName));
    }

    private void RefreshPedalboard()
    {
        var items = new List<PedalboardItemViewModel>
        {
            new()
            {
                Key = "input",
                DisplayName = "INPUT",
                Detail = "Guitar",
                IsActive = true,
                IsEndpoint = true,
                Family = "io",
            },
        };

        AddPanelEffects(items, beforeAmp: true);
        if (string.Equals(PedalFx.SelectedPositionOption, "Input", StringComparison.Ordinal))
        {
            items.Add(CreatePedalFxBoardItem(items.Count > 0));
        }

        items.Add(new PedalboardItemViewModel
        {
            Key = "amp",
            DisplayName = "AMP",
            Detail = SelectedPanelChannel,
            IsActive = true,
            IsAmp = true,
            IsConnectedFromPrevious = items.Count > 0,
            Family = "amp",
        });

        if (string.Equals(PedalFx.SelectedPositionOption, "Post Amp", StringComparison.Ordinal))
        {
            items.Add(CreatePedalFxBoardItem(items.Count > 0));
        }

        AddPanelEffects(items, beforeAmp: false);
        items.Add(new PedalboardItemViewModel
        {
            Key = "output",
            DisplayName = "OUTPUT",
            Detail = "Speaker / Rec Out",
            IsActive = true,
            IsEndpoint = true,
            IsConnectedFromPrevious = items.Count > 0,
            Family = "io",
        });

        PedalboardItems.Clear();
        foreach (var item in items)
        {
            PedalboardItems.Add(item);
        }
    }

    private void AddPanelEffects(List<PedalboardItemViewModel> items, bool beforeAmp)
    {
        foreach (var effect in PanelEffects)
        {
            var isPreAmp = effect.Definition.Key is "booster" or "mod" or "fx";
            if (isPreAmp != beforeAmp)
            {
                continue;
            }

            items.Add(new PedalboardItemViewModel
            {
                Key = effect.Definition.Key,
                DisplayName = effect.DisplayName.ToUpperInvariant(),
                Detail = effect.Definition.TypeParameter is null
                    ? effect.VariationCaption
                    : $"{effect.TypeCaption} / {effect.VariationCaption}",
                IsActive = effect.IsEnabled,
                IsConnectedFromPrevious = items.Count > 0,
                CanToggle = true,
                Family = effect.Definition.Key,
            });
        }
    }

    private PedalboardItemViewModel CreatePedalFxBoardItem(bool isConnectedFromPrevious)
    {
        return new PedalboardItemViewModel
        {
            Key = "pedal-fx",
            DisplayName = "PEDAL FX",
            Detail = $"{PedalFx.SelectedTypeOption} / {PedalFx.SelectedPositionOption}",
            IsActive = PedalFx.IsEnabled,
            IsConnectedFromPrevious = isConnectedFromPrevious,
            CanToggle = true,
            Family = "pedal",
        };
    }

    private void UpdatePanelChannelSelection()
    {
        foreach (var option in PanelChannelOptions)
        {
            option.IsSelected = option.DisplayName == SelectedPanelChannel;
        }
    }

    private void TrackAmpControlChange(AmpControlViewModel control)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected)
        {
            return;
        }

        var clampedValue = Math.Clamp(control.Value, control.Minimum, control.Maximum);
        pendingAmpWrites[control.Parameter.Key] = (byte)clampedValue;
        AppendLog($"Queued amp sync: {control.DisplayName} -> {clampedValue}.");
        PauseActiveReadSync("queued writes are pending");
        UpdateWriteSyncTimer();
    }

    private void TrackPanelEffectChange(PanelEffectViewModel effect)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected)
        {
            return;
        }

        pendingPanelEffectWrites[effect.Definition.SwitchParameter.Key] = effect.IsEnabled;
        AppendLog($"Queued panel sync: {effect.DisplayName} -> {(effect.IsEnabled ? "On" : "Off")}.");
        PauseActiveReadSync("queued writes are pending");
        UpdateWriteSyncTimer();
    }

    private void TrackPanelEffectTypeChange(PanelEffectViewModel effect)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected || effect.Definition.TypeParameter is null)
        {
            return;
        }

        if (!effect.TryGetTypeValue(effect.SelectedTypeOption, out var typeValue))
        {
            return;
        }

        pendingPanelTypeWrites[effect.Definition.TypeParameter.Key] = typeValue;
        AppendLog($"Queued panel type sync: {effect.DisplayName} -> {effect.SelectedTypeOption}.");
        PauseActiveReadSync("queued writes are pending");
        UpdateWriteSyncTimer();
    }

    private void TrackPanelEffectLevelChange(PanelEffectViewModel effect)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected || effect.Definition.LevelParameter is null)
        {
            return;
        }

        pendingPanelLevelWrites[effect.Definition.LevelParameter.Key] = (byte)Math.Clamp(effect.Level, 0, 100);
        AppendLog($"Queued panel level sync: {effect.DisplayName} level -> {effect.Level}.");
        PauseActiveReadSync("queued writes are pending");
        UpdateWriteSyncTimer();
    }

    private void TrackPedalChange(string? propertyName)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected)
        {
            return;
        }

        if (!PedalFx.TryGetParameterValue(propertyName, out var parameter, out var value, out var description))
        {
            return;
        }

        pendingPedalWrites[parameter.Key] = value;
        AppendLog($"Queued pedal sync: {parameter.DisplayName} -> {description}.");
        PauseActiveReadSync("queued writes are pending");
        UpdateWriteSyncTimer();
    }

    private void UpdateWriteSyncTimer()
    {
        writeSyncTimer.Stop();

        if (ActiveWriteSync && IsConnected && HasPendingWrites())
        {
            writeSyncTimer.Start();
        }
    }

    private void UpdateReadSyncTimer()
    {
        readSyncTimer.Stop();

        if (ShouldRunActiveReadSync())
        {
            readSyncTimer.Start();
        }
    }

    private async Task FlushPendingWritesAsync()
    {
        if (!ActiveWriteSync || !IsConnected)
        {
            return;
        }

        if (pendingAmpWrites.Count == 0 && pendingPanelEffectWrites.Count == 0 && pendingPanelTypeWrites.Count == 0 && pendingPanelLevelWrites.Count == 0 && pendingPedalWrites.Count == 0 && pendingPanelChannel is null && !pendingAmpTypeWrite.HasValue && !pendingCabinetResonanceWrite.HasValue)
        {
            return;
        }

        if (!syncOperationGate.Wait(0))
        {
            AppendLog("Queued write sync is already running; the latest changes will wait for the next flush.");
            return;
        }

        var ampSnapshot = new Dictionary<string, byte>(StringComparer.Ordinal);
        var panelSnapshot = new Dictionary<string, bool>(StringComparer.Ordinal);
        var panelTypeSnapshot = new Dictionary<string, byte>(StringComparer.Ordinal);
        var panelLevelSnapshot = new Dictionary<string, byte>(StringComparer.Ordinal);
        var pedalSnapshot = new Dictionary<string, byte>(StringComparer.Ordinal);
        string? channelSnapshot = null;
        byte? ampTypeSnapshot = null;
        byte? cabinetSnapshot = null;

        try
        {
            PauseActiveReadSync("queued write sync is flushing");
            ampSnapshot = new Dictionary<string, byte>(pendingAmpWrites, StringComparer.Ordinal);
            panelSnapshot = new Dictionary<string, bool>(pendingPanelEffectWrites, StringComparer.Ordinal);
            panelTypeSnapshot = new Dictionary<string, byte>(pendingPanelTypeWrites, StringComparer.Ordinal);
            panelLevelSnapshot = new Dictionary<string, byte>(pendingPanelLevelWrites, StringComparer.Ordinal);
            pedalSnapshot = new Dictionary<string, byte>(pendingPedalWrites, StringComparer.Ordinal);
            channelSnapshot = pendingPanelChannel;
            ampTypeSnapshot = pendingAmpTypeWrite;
            cabinetSnapshot = pendingCabinetResonanceWrite;

            pendingAmpWrites.Clear();
            pendingPanelEffectWrites.Clear();
            pendingPanelTypeWrites.Clear();
            pendingPanelLevelWrites.Clear();
            pendingPedalWrites.Clear();
            pendingPanelChannel = null;
            pendingAmpTypeWrite = null;
            pendingCabinetResonanceWrite = null;

            AppendLog(
                $"Flushing queued sync: {ampSnapshot.Count} amp, {panelSnapshot.Count} panel, {panelTypeSnapshot.Count} panel type, {panelLevelSnapshot.Count} panel level, {pedalSnapshot.Count} pedal, {(channelSnapshot is null ? "no" : "1")} channel change.");

            await FlushPendingAmpWritesAsync(ampSnapshot);
            await FlushPendingPanelWritesAsync(channelSnapshot, panelSnapshot, panelTypeSnapshot, panelLevelSnapshot, ampTypeSnapshot, cabinetSnapshot);
            await FlushPendingPedalWritesAsync(pedalSnapshot);

            if (ActiveReadSync)
            {
                AppendLog("Active read sync is enabled; refreshing written state from the Katana.");
                await RefreshWrittenStateAsync(ampSnapshot, panelSnapshot, panelTypeSnapshot, pedalSnapshot, channelSnapshot);
            }
            else
            {
                AppendLog("Active read sync is disabled; skipping post-write refresh.");
            }

            if (ampSnapshot.Count > 0 || panelSnapshot.Count > 0 || panelTypeSnapshot.Count > 0 || pedalSnapshot.Count > 0 || channelSnapshot is not null)
            {
                StatusMessage = "Queued changes synced to the Katana.";
                AppendLog("Queued changes synced to the Katana.");
            }
        }
        catch (Exception ex)
        {
            foreach (var entry in ampSnapshot)
            {
                pendingAmpWrites[entry.Key] = entry.Value;
            }

            foreach (var entry in panelSnapshot)
            {
                pendingPanelEffectWrites[entry.Key] = entry.Value;
            }

            foreach (var entry in panelTypeSnapshot)
            {
                pendingPanelTypeWrites[entry.Key] = entry.Value;
            }

            foreach (var entry in pedalSnapshot)
            {
                pendingPedalWrites[entry.Key] = entry.Value;
            }

            pendingPanelChannel ??= channelSnapshot;
            StatusMessage = ex.Message;
            AppendLog($"Queued write sync failed. Re-queued {DescribePendingWrites()}.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
        finally
        {
            ResumeActiveReadSync("queued write sync finished");
            syncOperationGate.Release();
        }
    }

    private async Task RunActiveReadSyncCycleAsync()
    {
        if (!ShouldRunActiveReadSync())
        {
            return;
        }

        if (HasPendingWrites())
        {
            AppendLog("Active read sync deferred because queued writes are waiting to flush.");
            return;
        }

        if (!syncOperationGate.Wait(0))
        {
            AppendLog("Active read sync skipped because another sync operation is already running.");
            return;
        }

        try
        {
            switch (activeReadSyncPhase)
            {
                case 0:
                    AppendLog("Active read sync polling amp controls.");
                    await TryReadAmpControlsAsync(backgroundSync: true);
                    break;
                case 1:
                    AppendLog("Active read sync polling panel state.");
                    await TryReadPanelControlsAsync(backgroundSync: true);
                    break;
                default:
                    AppendLog("Active read sync polling pedal state.");
                    await TryReadPedalControlsAsync(backgroundSync: true);
                    break;
            }

            activeReadSyncPhase = (activeReadSyncPhase + 1) % 3;
        }
        finally
        {
            syncOperationGate.Release();
        }
    }

    private async Task FlushPendingAmpWritesAsync(IReadOnlyDictionary<string, byte> ampWrites)
    {
        if (ampWrites.Count == 0)
        {
            return;
        }

        var orderedParameters = ampWrites.Keys
            .Select(key => ampControlsByKey[key].Parameter)
            .OrderBy(parameter => parameter.Address[0])
            .ThenBy(parameter => parameter.Address[1])
            .ThenBy(parameter => parameter.Address[2])
            .ThenBy(parameter => parameter.Address[3])
            .ToList();

        var groupStart = 0;
        while (groupStart < orderedParameters.Count)
        {
            var group = new List<KatanaParameterDefinition> { orderedParameters[groupStart] };
            var groupIndex = groupStart + 1;

            while (groupIndex < orderedParameters.Count && CanBatchWrite(group[^1], orderedParameters[groupIndex]))
            {
                group.Add(orderedParameters[groupIndex]);
                groupIndex++;
            }

            var startAddress = group[0].Address.ToArray();
            var data = group
                .Select(parameter => ampWrites[parameter.Key])
                .ToArray();

            AppendLog($"Writing amp sync batch: {DescribeAmpKeys(group.Select(parameter => parameter.Key))}.");
            await katanaSession.WriteBlockAsync(startAddress, data);
            groupStart = groupIndex;
        }
    }

    private async Task FlushPendingPanelWritesAsync(
        string? channel,
        IReadOnlyDictionary<string, bool> panelWrites,
        IReadOnlyDictionary<string, byte> panelTypeWrites,
        IReadOnlyDictionary<string, byte> panelLevelWrites,
        byte? ampTypeWrite,
        byte? cabinetResonanceWrite)
    {
        if (!string.IsNullOrWhiteSpace(channel))
        {
            AppendLog($"Writing queued panel channel: {channel}.");
            await katanaSession.SelectPanelChannelAsync(ParsePanelChannelDisplay(channel));
        }

        foreach (var entry in panelWrites)
        {
            var parameter = panelEffectsByKey[entry.Key].Definition.SwitchParameter;
            AppendLog($"Writing queued panel effect: {panelEffectsByKey[entry.Key].DisplayName} -> {(entry.Value ? "On" : "Off")}.");
            await katanaSession.WriteBlockAsync(parameter.Address, [entry.Value ? (byte)1 : (byte)0]);
        }

        foreach (var entry in panelTypeWrites)
        {
            var effect = PanelEffects.First(panel => panel.Definition.TypeParameter?.Key == entry.Key);
            AppendLog($"Writing queued panel type: {effect.DisplayName} -> {effect.ToTypeOption(entry.Value)}.");
            await katanaSession.WriteBlockAsync(effect.Definition.TypeParameter!.Address, [entry.Value]);
        }

        foreach (var entry in panelLevelWrites)
        {
            var effect = PanelEffects.First(panel => panel.Definition.LevelParameter?.Key == entry.Key);
            AppendLog($"Writing queued panel level: {effect.DisplayName} level -> {entry.Value}.");
            await katanaSession.WriteBlockAsync(effect.Definition.LevelParameter!.Address, [entry.Value]);
        }

        if (ampTypeWrite.HasValue)
        {
            AppendLog($"Writing queued amp type: {(ampTypeWrite.Value < AmpTypeOptions.Length ? AmpTypeOptions[ampTypeWrite.Value] : ampTypeWrite.Value.ToString())}.");
            await katanaSession.WriteBlockAsync(KatanaMkIIParameterCatalog.AmpType.Address, [ampTypeWrite.Value]);
        }

        if (cabinetResonanceWrite.HasValue)
        {
            AppendLog($"Writing queued cabinet resonance: {(cabinetResonanceWrite.Value < CabinetResonanceOptions.Length ? CabinetResonanceOptions[cabinetResonanceWrite.Value] : cabinetResonanceWrite.Value.ToString())}.");
            await katanaSession.WriteBlockAsync(KatanaMkIIParameterCatalog.CabinetResonance.Address, [cabinetResonanceWrite.Value]);
        }
    }

    private async Task FlushPendingPedalWritesAsync(IReadOnlyDictionary<string, byte> pedalWrites)
    {
        if (pedalWrites.Count == 0)
        {
            return;
        }

        var orderedParameters = pedalWrites.Keys
            .Select(PedalFx.GetParameter)
            .OrderBy(parameter => parameter.Address[0])
            .ThenBy(parameter => parameter.Address[1])
            .ThenBy(parameter => parameter.Address[2])
            .ThenBy(parameter => parameter.Address[3])
            .ToList();

        var groupStart = 0;
        while (groupStart < orderedParameters.Count)
        {
            var group = new List<KatanaParameterDefinition> { orderedParameters[groupStart] };
            var groupIndex = groupStart + 1;

            while (groupIndex < orderedParameters.Count && CanBatchWrite(group[^1], orderedParameters[groupIndex]))
            {
                group.Add(orderedParameters[groupIndex]);
                groupIndex++;
            }

            var startAddress = group[0].Address.ToArray();
            var data = group
                .Select(parameter => pedalWrites[parameter.Key])
                .ToArray();

            AppendLog($"Writing pedal sync batch: {DescribePedalKeys(group.Select(parameter => parameter.Key))}.");
            await katanaSession.WriteBlockAsync(startAddress, data);
            groupStart = groupIndex;
        }
    }

    private async Task RefreshWrittenStateAsync(
        IReadOnlyDictionary<string, byte> ampWrites,
        IReadOnlyDictionary<string, bool> panelWrites,
        IReadOnlyDictionary<string, byte> panelTypeWrites,
        IReadOnlyDictionary<string, byte> pedalWrites,
        string? channelWrite)
    {
        if (ampWrites.Count > 0)
        {
            var ampParameters = ampWrites.Keys
                .Distinct(StringComparer.Ordinal)
                .Select(key => ampControlsByKey[key].Parameter)
                .ToArray();
            AppendLog($"Read sync refreshing amp values: {DescribeAmpKeys(ampWrites.Keys)}.");
            var values = await katanaSession.ReadParametersAsync(ampParameters);

            suppressChangeTracking = true;
            try
            {
                foreach (var parameter in ampParameters)
                {
                    var control = ampControlsByKey[parameter.Key];
                    var expectedValue = ampWrites[parameter.Key];
                    if (pendingAmpWrites.ContainsKey(parameter.Key) || control.Value != expectedValue)
                    {
                        AppendLog($"Skipping stale amp readback for {control.DisplayName}; a newer local value is pending.");
                        continue;
                    }

                    control.Value = values[parameter.Key];
                }
            }
            finally
            {
                suppressChangeTracking = false;
            }
        }

        if (panelWrites.Count > 0)
        {
            var panelParameters = panelWrites.Keys
                .Distinct(StringComparer.Ordinal)
                .Select(key => panelEffectsByKey[key].Definition.SwitchParameter)
                .ToArray();
            AppendLog($"Read sync refreshing panel values: {DescribePanelKeys(panelWrites.Keys)}.");
            var values = await katanaSession.ReadParametersAsync(panelParameters);

            suppressChangeTracking = true;
            try
            {
                foreach (var parameter in panelParameters)
                {
                    var effect = panelEffectsByKey[parameter.Key];
                    var expectedValue = panelWrites[parameter.Key];
                    if (pendingPanelEffectWrites.ContainsKey(parameter.Key) || effect.IsEnabled != expectedValue)
                    {
                        AppendLog($"Skipping stale panel readback for {effect.DisplayName}; a newer local value is pending.");
                        continue;
                    }

                    effect.IsEnabled = values[parameter.Key] != 0;
                }
            }
            finally
            {
                suppressChangeTracking = false;
            }
        }

        if (panelTypeWrites.Count > 0)
        {
            var typeParameters = PanelEffects
                .Where(effect => effect.Definition.TypeParameter is not null && panelTypeWrites.ContainsKey(effect.Definition.TypeParameter.Key))
                .Select(effect => effect.Definition.TypeParameter!)
                .ToArray();
            AppendLog($"Read sync refreshing panel types: {string.Join(", ", typeParameters.Select(parameter => parameter.DisplayName))}.");
            var values = await katanaSession.ReadParametersAsync(typeParameters);

            suppressChangeTracking = true;
            try
            {
                foreach (var parameter in typeParameters)
                {
                    var effect = PanelEffects.First(panel => panel.Definition.TypeParameter?.Key == parameter.Key);
                    var expectedValue = panelTypeWrites[parameter.Key];
                    if (pendingPanelTypeWrites.ContainsKey(parameter.Key) ||
                        !effect.TryGetTypeValue(effect.SelectedTypeOption, out var currentValue) ||
                        currentValue != expectedValue)
                    {
                        AppendLog($"Skipping stale panel type readback for {effect.DisplayName}; a newer local value is pending.");
                        continue;
                    }

                    effect.SelectedTypeOption = effect.ToTypeOption(values[parameter.Key]);
                }
            }
            finally
            {
                suppressChangeTracking = false;
            }
        }

        if (pedalWrites.Count > 0)
        {
            var pedalParameters = pedalWrites.Keys
                .Select(PedalFx.GetParameter)
                .DistinctBy(parameter => parameter.Key)
                .ToArray();
            AppendLog($"Read sync refreshing pedal values: {DescribePedalKeys(pedalWrites.Keys)}.");
            var values = await katanaSession.ReadParametersAsync(pedalParameters);

            suppressChangeTracking = true;
            try
            {
                foreach (var parameter in pedalParameters)
                {
                    var expectedValue = pedalWrites[parameter.Key];
                    if (pendingPedalWrites.ContainsKey(parameter.Key) ||
                        !PedalFx.TryGetCurrentValue(parameter.Key, out var currentValue) ||
                        currentValue != expectedValue)
                    {
                        AppendLog($"Skipping stale pedal readback for {parameter.DisplayName}; a newer local value is pending.");
                        continue;
                    }

                    ApplyPedalValue(parameter.Key, values[parameter.Key]);
                }
            }
            finally
            {
                suppressChangeTracking = false;
            }
        }

        if (channelWrite is not null)
        {
            AppendLog("Read sync refreshing current panel channel.");
            var currentChannel = await katanaSession.ReadCurrentPanelChannelAsync();
            if (currentChannel is not null)
            {
                if (pendingPanelChannel is not null || !string.Equals(SelectedPanelChannel, channelWrite, StringComparison.Ordinal))
                {
                    AppendLog("Skipping stale panel channel readback; a newer local channel selection is pending.");
                    return;
                }

                suppressChangeTracking = true;
                try
                {
                    SelectedPanelChannel = ToPanelChannelDisplay(currentChannel.Value);
                }
                finally
                {
                    suppressChangeTracking = false;
                }
            }
        }
    }

    private static bool CanBatchWrite(KatanaParameterDefinition previous, KatanaParameterDefinition current)
    {
        return previous.Address[0] == current.Address[0] &&
               previous.Address[1] == current.Address[1] &&
               previous.Address[2] == current.Address[2] &&
               previous.Address[3] + 1 == current.Address[3];
    }

    private static byte[] EncodeDelayTime(int milliseconds)
    {
        var clamped = Math.Clamp(milliseconds, 1, 2000);
        return [(byte)((clamped >> 7) & 0x0F), (byte)(clamped & 0x7F)];
    }

    private static int DecodeDelayTime(IReadOnlyList<byte> data)
    {
        if (data.Count != 2)
        {
            throw new ArgumentException("Delay time data must contain exactly 2 bytes.", nameof(data));
        }

        return ((data[0] & 0x0F) << 7) | (data[1] & 0x7F);
    }

    private bool PatchLevelMappingVerified => false;

    private async Task<bool> TryRefreshPatchLevelAsync()
    {
        if (!PatchLevelMappingVerified)
        {
            return false;
        }

        try
        {
            PatchLevel = await katanaSession.ReadParameterAsync(KatanaMkIIParameterCatalog.PatchLevel);
            AppendLog($"Patch Level reply: {PatchLevel}");
            return true;
        }
        catch (Exception ex)
        {
            AppendLog("Patch level read failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    private async Task<bool> TryRefreshDelayTimeAsync()
    {
        try
        {
            var delayTime = DecodeDelayTime(await katanaSession.ReadBlockAsync(KatanaMkIIParameterCatalog.DelayTimeAddress, 2));
            if (delayTime <= 0)
            {
                DelayTapStatus = "Delay time is not currently readable for the active effect state.";
                AppendLog("Delay time reply did not contain a usable millisecond value.");
                return false;
            }

            DelayTapStatus = $"Delay time loaded: {delayTime} ms.";
            AppendLog($"Delay time reply: {delayTime} ms.");
            return true;
        }
        catch (Exception ex)
        {
            DelayTapStatus = "Delay time refresh failed.";
            AppendLog("Delay time read failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    private async Task<bool> TryWritePatchLevelAsync()
    {
        if (!PatchLevelMappingVerified)
        {
            return false;
        }

        try
        {
            var requestedPatchLevel = Math.Clamp(
                PatchLevel,
                KatanaMkIIParameterCatalog.PatchLevel.Minimum,
                KatanaMkIIParameterCatalog.PatchLevel.Maximum);
            if (requestedPatchLevel != PatchLevel)
            {
                PatchLevel = requestedPatchLevel;
                AppendLog($"Clamped Patch Level to {requestedPatchLevel}.");
            }

            PatchLevel = await katanaSession.WriteParameterAsync(KatanaMkIIParameterCatalog.PatchLevel, (byte)requestedPatchLevel);
            AppendLog($"Patch Level confirmed at {PatchLevel}.");
            return true;
        }
        catch (Exception ex)
        {
            AppendLog("Patch level write failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    private async Task<bool> TryReadAmpControlsAsync(bool backgroundSync = false)
    {
        try
        {
            AppendLog(backgroundSync
                ? "Refreshing Katana amp editor controls from active read sync."
                : "Reading Katana amp editor controls.");
            var values = await katanaSession.ReadParametersAsync(AmpControls.Select(control => control.Parameter).ToArray());
            suppressChangeTracking = true;
            try
            {
                foreach (var control in AmpControls)
                {
                    var value = values[control.Parameter.Key];
                    control.Value = value;
                    AppendLog($"{control.DisplayName} reply: {value}");
                }
            }
            finally
            {
                suppressChangeTracking = false;
            }

            if (!backgroundSync)
            {
                StatusMessage = "Amp editor controls read successfully.";
                AmpEditorStatus = "Amp editor values were loaded from the Katana.";
            }

            return true;
        }
        catch (Exception ex)
        {
            if (!backgroundSync)
            {
                AmpEditorStatus = "Amp editor read failed.";
                StatusMessage = ex.Message;
            }

            AppendLog("Amp editor read failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    private async Task<bool> TryReadPanelControlsAsync(bool backgroundSync = false)
    {
        try
        {
            AppendLog(backgroundSync
                ? "Refreshing Katana panel controls from active read sync."
                : "Reading Katana panel controls.");

            var currentChannel = await katanaSession.ReadCurrentPanelChannelAsync();
            if (currentChannel is not null)
            {
                suppressChangeTracking = true;
                try
                {
                    SelectedPanelChannel = ToPanelChannelDisplay(currentChannel.Value);
                    AppendLog($"Current panel channel: {SelectedPanelChannel}");
                }
                finally
                {
                    suppressChangeTracking = false;
                }
            }

            var parameterList = PanelEffects
                .Select(effect => effect.Definition.SwitchParameter)
                .Concat(PanelEffects
                    .Where(effect => effect.Definition.VariationParameter is not null)
                    .Select(effect => effect.Definition.VariationParameter!))
                .Concat(PanelEffects
                    .Where(effect => effect.Definition.TypeParameter is not null)
                    .Select(effect => effect.Definition.TypeParameter!))
                .Concat(PanelEffects
                    .Where(effect => effect.Definition.LevelParameter is not null)
                    .Select(effect => effect.Definition.LevelParameter!))
                .Append(KatanaMkIIParameterCatalog.AmpType)
                .Append(KatanaMkIIParameterCatalog.CabinetResonance)
                .ToArray();
            var values = await katanaSession.ReadParametersAsync(parameterList);

            suppressChangeTracking = true;
            try
            {
                foreach (var effect in PanelEffects)
                {
                    effect.IsEnabled = values[effect.Definition.SwitchParameter.Key] != 0;
                    effect.Variation = effect.Definition.VariationParameter is null
                        ? "N/A"
                        : ToVariationDisplay(values[effect.Definition.VariationParameter.Key]);
                    effect.SelectedTypeOption = effect.Definition.TypeParameter is null
                        ? "N/A"
                        : effect.ToTypeOption(values[effect.Definition.TypeParameter.Key]);
                    if (effect.HasLevel && effect.Definition.LevelParameter is not null &&
                        values.TryGetValue(effect.Definition.LevelParameter.Key, out var levelValue))
                    {
                        effect.Level = levelValue;
                    }
                    AppendLog($"{effect.DisplayName}: {(effect.IsEnabled ? "On" : "Off")} / {effect.Variation} / {effect.TypeCaption}");
                }

                if (values.TryGetValue(KatanaMkIIParameterCatalog.AmpType.Key, out var ampTypeValue) &&
                    ampTypeValue < AmpTypeOptions.Length)
                {
                    SelectedAmpType = AmpTypeOptions[ampTypeValue];
                }
                if (values.TryGetValue(KatanaMkIIParameterCatalog.CabinetResonance.Key, out var cabValue) &&
                    cabValue < CabinetResonanceOptions.Length)
                {
                    SelectedCabinetResonance = CabinetResonanceOptions[cabValue];
                }
                AppendLog($"Amp Type: {SelectedAmpType} / Cabinet Resonance: {SelectedCabinetResonance}");
            }
            finally
            {
                suppressChangeTracking = false;
            }

            lastDelayTapAt = null;
            var patchLevelLoaded = backgroundSync
                ? false
                : await TryRefreshPatchLevelAsync();
            var delayTimeLoaded = backgroundSync
                ? false
                : await TryRefreshDelayTimeAsync();

            if (!backgroundSync)
            {
                StatusMessage = "Panel controls read successfully.";
                PanelControlsStatus = (patchLevelLoaded, delayTimeLoaded) switch
                {
                    (true, true) => "Panel channel, patch level, effect toggles, variation colors, effect types, and delay time were loaded.",
                    (true, false) => "Panel channel, patch level, effect toggles, variation colors, and effect types were loaded. Delay time refresh failed.",
                    (false, true) => "Panel channel, effect toggles, variation colors, effect types, and delay time were loaded. Patch level mapping is still pending.",
                    _ => "Panel channel, effect toggles, variation colors, and effect types were loaded. Patch level mapping is still pending, and delay time refresh failed.",
                };
            }

            return true;
        }
        catch (Exception ex)
        {
            if (!backgroundSync)
            {
                PanelControlsStatus = "Panel control read failed.";
                StatusMessage = ex.Message;
            }

            AppendLog("Panel control read failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    private async Task<bool> TryReadPedalControlsAsync(bool backgroundSync = false)
    {
        try
        {
            AppendLog(backgroundSync
                ? "Refreshing Katana pedal controls from active read sync."
                : "Reading Katana pedal controls.");
            var values = await katanaSession.ReadParametersAsync(PedalFx.GetReadParameters().ToArray());

            suppressChangeTracking = true;
            try
            {
                PedalFx.ApplyValues(values);
                AppendLog($"Pedal FX: {(PedalFx.IsEnabled ? "On" : "Off")} / {PedalFx.SelectedTypeOption} / {PedalFx.SelectedPositionOption} / Foot Volume {PedalFx.FootVolume}");
                if (PedalFx.IsWahMode)
                {
                    AppendLog($"Pedal Wah: {PedalFx.SelectedWahTypeOption} / Pos {PedalFx.PedalPosition} / Min {PedalFx.PedalMinimum} / Max {PedalFx.PedalMaximum} / Level {PedalFx.EffectLevel} / Direct {PedalFx.DirectMix}");
                }
            }
            finally
            {
                suppressChangeTracking = false;
            }

            if (!backgroundSync)
            {
                StatusMessage = "Pedal controls read successfully.";
                PedalControlsStatus = PedalFx.IsWahMode
                    ? "Pedal FX, wah controls, and foot volume were loaded."
                    : "Pedal FX type and foot volume were loaded. Non-wah subtype controls are not surfaced yet.";
            }

            return true;
        }
        catch (Exception ex)
        {
            if (!backgroundSync)
            {
                PedalControlsStatus = "Pedal control read failed.";
                StatusMessage = ex.Message;
            }

            AppendLog("Pedal control read failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
            return false;
        }
    }

    private static string ToVariationDisplay(byte value)
    {
        return value switch
        {
            0 => "Green",
            1 => "Red",
            2 => "Yellow",
            _ => $"Value {value}",
        };
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

    private static string ToPanelChannelDisplay(KatanaPanelChannel channel)
    {
        return channel switch
        {
            KatanaPanelChannel.Panel => "Panel",
            KatanaPanelChannel.ChA1 => "CH A1",
            KatanaPanelChannel.ChA2 => "CH A2",
            KatanaPanelChannel.ChB1 => "CH B1",
            KatanaPanelChannel.ChB2 => "CH B2",
            _ => "Panel",
        };
    }

    private static KatanaPanelChannel ParsePanelChannelDisplay(string channel)
    {
        return channel switch
        {
            "CH A1" => KatanaPanelChannel.ChA1,
            "CH A2" => KatanaPanelChannel.ChA2,
            "CH B1" => KatanaPanelChannel.ChB1,
            "CH B2" => KatanaPanelChannel.ChB2,
            _ => KatanaPanelChannel.Panel,
        };
    }
}
