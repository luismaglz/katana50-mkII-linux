using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
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
    private static readonly TimeSpan ReadSyncInterval = TimeSpan.FromSeconds(2);
    private readonly IKatanaSession katanaSession;
    private readonly Dictionary<string, string> inputPortIds = [];
    private readonly Dictionary<string, string> outputPortIds = [];
    private readonly Dictionary<string, AmpControlViewModel> ampControlsByKey = [];
    private readonly Dictionary<string, IBasePedal> panelEffectsByKey = [];
    private readonly Dictionary<string, IBasePedal> panelEffectsByDefinitionKey = [];
    private readonly Dictionary<string, byte> pendingWrites = [];

    // Lookup built once at connect time: maps 4-byte address key (e.g. "60-00-06-50") to an
    // action that updates the matching VM property. Used to apply unsolicited DT1 push notifications
    // from the amp without needing to know which parameter changed ahead of time.
    private Dictionary<string, Action<byte>> pushHandlerLookup = [];
    private readonly SemaphoreSlim syncOperationGate = new(1, 1);
    private readonly DispatcherTimer writeSyncTimer;
    private readonly DispatcherTimer readSyncTimer;
    private DateTimeOffset? lastDelayTapAt;
    private bool suppressChangeTracking;
    private int activeReadSyncPhase;
    private int flushRetryCount;
    private bool suspendActiveReadSync;
    private bool isShuttingDown;
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

        foreach (var effectViewModel in new IBasePedal[]
        {
            new BoosterPedalViewModel(),
            new ModFxPedalViewModel("mod"),
            new ModFxPedalViewModel("fx"),
            new DelayPedalViewModel("delay"),
            new DelayPedalViewModel("delay2"),
            new ReverbPedalViewModel(),
        })
        {
            effectViewModel.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(IBasePedal.IsEnabled))
                {
                    TrackPanelEffectChange(effectViewModel);
                }
                else if (args.PropertyName == nameof(IBasePedal.SelectedTypeOption))
                {
                    TrackPanelEffectTypeChange(effectViewModel);
                }
            };

            PanelEffects.Add(effectViewModel);
            panelEffectsByKey[effectViewModel.Definition.SwitchParameter.Key] = effectViewModel;
            panelEffectsByDefinitionKey[effectViewModel.Definition.Key] = effectViewModel;

            effectViewModel.ParameterChanged += (_, args) => TrackDetailParamChange(args.Key, args.Value);
        }

        foreach (var channel in PanelChannels)
        {
            PanelChannelOptions.Add(new PanelChannelOptionViewModel(channel));
        }

        Pedalboard = new PedalboardViewModel(panelEffectsByDefinitionKey, SelectedPanelChannel);
        Pedalboard.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(PedalboardViewModel.SelectedChainPattern)) return;
            if (suppressChangeTracking || !ActiveWriteSync || !IsConnected) return;
            var value = Pedalboard.SelectedChainPattern;
            if (value < 0 || value >= PedalboardViewModel.ChainPatternNames.Length) return;
            pendingWrites[KatanaMkIIParameterCatalog.ChainPattern.Key] = (byte)value;
            AppendLog($"Queued panel sync: Chain Pattern -> {PedalboardViewModel.ChainPatternNames[value]}.");
        };

        PedalFx.PropertyChanged += (_, args) =>
        {
            TrackPedalChange(args.PropertyName);
        };

        UpdatePanelChannelSelection();
        Pedalboard.Refresh();
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
    [NotifyPropertyChangedFor(nameof(CanWritePatch))]
    [NotifyCanExecuteChangedFor(nameof(WritePatchCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadPatchCommand))]
    [NotifyCanExecuteChangedFor(nameof(SavePatchCommand))]
    public partial bool IsConnected { get; set; }

    [ObservableProperty]
    public partial string DiagnosticLog { get; set; } = "Diagnostic log ready.";

    [ObservableProperty]
    public partial string IdentityReply { get; set; } = "Identity request has not been run yet.";

    public ObservableCollection<AmpControlViewModel> AmpControls { get; } = [];

    public ObservableCollection<IBasePedal> PanelEffects { get; } = [];

    public ObservableCollection<PanelChannelOptionViewModel> PanelChannelOptions { get; } = [];

    public PedalFxViewModel PedalFx { get; } = new();

    public PedalboardViewModel Pedalboard { get; private set; } = null!;

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

    [ObservableProperty]
    public partial int? DelayTimeMs { get; set; }

    public bool IsPatchLevelAvailable => PatchLevelMappingVerified;

    public bool CanWritePatch => IsConnected;

    [ObservableProperty]
    public partial bool ActiveReadSync { get; set; } = false;

    [ObservableProperty]
    public partial bool ActiveWriteSync { get; set; } = true;

    public static string[] AmpTypeOptions { get; } = ["ACOUSTIC", "CLEAN", "CRUNCH", "LEAD", "BROWN"];
    public static string[] CabinetResonanceOptions { get; } = ["LOW", "MIDDLE", "HIGH"];

    [ObservableProperty]
    public partial string SelectedAmpType { get; set; } = "CLEAN";

    [ObservableProperty]
    public partial string SelectedCabinetResonance { get; set; } = "MIDDLE";

    [ObservableProperty]
    public partial bool IsAmpVariation { get; set; } = false;

    partial void OnIsAmpVariationChanged(bool value)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected) return;
        pendingWrites[KatanaMkIIParameterCatalog.AmpVariation.Key] = value ? (byte)1 : (byte)0;
        AppendLog($"Queued panel sync: Amp Variation -> {(value ? "TYPE 2" : "TYPE 1")}.");
    }

    partial void OnSelectedAmpTypeChanged(string value)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected) return;
        var idx = Array.IndexOf(AmpTypeOptions, value);
        if (idx < 0) return;
        pendingWrites[KatanaMkIIParameterCatalog.AmpType.Key] = (byte)idx;
        AppendLog($"Queued panel sync: Amp Type -> {value}.");
    }

    partial void OnSelectedCabinetResonanceChanged(string value)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected) return;
        var idx = Array.IndexOf(CabinetResonanceOptions, value);
        if (idx < 0) return;
        pendingWrites[KatanaMkIIParameterCatalog.CabinetResonance.Key] = (byte)idx;
        AppendLog($"Queued panel sync: Cabinet Resonance -> {value}.");
    }

    partial void OnSelectedPanelChannelChanged(string value)
    {
        UpdatePanelChannelSelection();
        Pedalboard.SelectedChannel = value;

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
            if (pendingWrites.Count > 0 || pendingPanelChannel is not null)
            {
                AppendLog($"Clearing pending sync queue after disconnect: {DescribePendingWrites()}.");
            }

            pendingWrites.Clear();
            pendingPanelChannel = null;
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
                DelayTimeMs = confirmedDelayTime;
                DelayTapStatus = $"Delay time tapped to {confirmedDelayTime} ms.";
                AppendLog($"Delay time confirmed at {confirmedDelayTime} ms.");
            }
            else
            {
                DelayTimeMs = tappedDelayTime;
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

    public IStorageProvider? StorageProvider { get; set; }

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
                new FilePickerFileType("Boss Tone Studio Patch") { Patterns = ["*.tsl"] },
                new FilePickerFileType("All Files") { Patterns = ["*"] },
            ],
        });

        if (files.Count == 0) return;

        var file = files[0];
        try
        {
            PauseActiveReadSync("patch load");
            AppendLog($"Loading patch from {file.Name}...");

            string json;
            await using (var stream = await file.OpenReadAsync())
            using (var reader = new System.IO.StreamReader(stream))
                json = await reader.ReadToEndAsync();

            var patch = TslPatchSerializer.Deserialize(json);
            AppendLog($"Patch '{patch.Name}' parsed — {patch.Blocks.Count} block(s). Sending to amp...");

            await katanaSession.LoadPatchAsync(patch);

            AppendLog("Patch loaded. Refreshing display...");
            await TryReadAmpControlsAsync(backgroundSync: false);
            await TryReadPanelControlsAsync(backgroundSync: false);
            await TryReadPedalControlsAsync();
            StatusMessage = $"Patch '{patch.Name}' loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Patch load failed.";
            AppendLog($"Patch load failed: {ex.Message}");
            Console.Error.WriteLine(ex);
        }
        finally
        {
            ResumeActiveReadSync("patch load finished");
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
                new FilePickerFileType("Boss Tone Studio Patch") { Patterns = ["*.tsl"] },
            ],
        });

        if (file is null) return;

        try
        {
            PauseActiveReadSync("patch save");
            AppendLog("Reading all patch blocks from amp...");

            var patchName = System.IO.Path.GetFileNameWithoutExtension(file.Name);
            var patch = await katanaSession.ReadCurrentPatchAsync(patchName);
            var json = TslPatchSerializer.Serialize(patch);

            await using var stream = await file.OpenWriteAsync();
            await using var writer = new System.IO.StreamWriter(stream);
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
        finally
        {
            ResumeActiveReadSync("patch save finished");
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

            // Subscribe to amp push notifications so live parameter changes (user turning knobs, etc.)
            // update the UI without a poll round-trip. Build the address→action lookup first.
            BuildPushHandlerLookup();
            katanaSession.PushNotificationReceived += OnAmpPushNotification;
            katanaSession.PanelChannelChanged      += OnAmpPanelChannelChanged;

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
            // Push notifications (EDITOR_COMMUNICATION_MODE) handle live state updates after
            // the initial load, so there is no need to auto-start polling on connect.
            // The user can still enable the read sync checkbox manually if desired.
            if (ActiveReadSync)
            {
                UpdateReadSyncTimer();
            }
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
        katanaSession.PushNotificationReceived -= OnAmpPushNotification;
        katanaSession.PanelChannelChanged      -= OnAmpPanelChannelChanged;
        pushHandlerLookup.Clear();

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
        if (pendingWrites.Count == 0 && pendingPanelChannel is null)
        {
            return "no pending changes";
        }

        var parts = new List<string>();
        if (pendingWrites.Count > 0)
        {
            parts.Add($"{pendingWrites.Count} writes");
        }

        if (pendingPanelChannel is not null)
        {
            parts.Add("channel");
        }

        return string.Join(", ", parts);
    }

    private bool HasPendingWrites()
    {
        return pendingWrites.Count > 0 || pendingPanelChannel is not null;
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
        pendingWrites[control.Parameter.Key] = (byte)clampedValue;
        AppendLog($"Queued amp sync: {control.DisplayName} -> {clampedValue}.");
        PauseActiveReadSync("queued writes are pending");
        UpdateWriteSyncTimer();
    }

    private void TrackPanelEffectChange(IBasePedal effect)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected)
        {
            return;
        }

        pendingWrites[effect.Definition.SwitchParameter.Key] = effect.IsEnabled ? (byte)1 : (byte)0;
        AppendLog($"Queued panel sync: {effect.DisplayName} -> {(effect.IsEnabled ? "On" : "Off")}.");
        PauseActiveReadSync("queued writes are pending");
        UpdateWriteSyncTimer();
    }

    private void TrackPanelEffectTypeChange(IBasePedal effect)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected || effect.Definition.TypeParameter is null)
        {
            return;
        }

        if (!effect.TryGetTypeValue(effect.SelectedTypeOption, out var typeValue))
        {
            return;
        }

        pendingWrites[effect.Definition.TypeParameter.Key] = typeValue;
        AppendLog($"Queued panel type sync: {effect.DisplayName} -> {effect.SelectedTypeOption}.");
        PauseActiveReadSync("queued writes are pending");
        UpdateWriteSyncTimer();
    }

    private void TrackDetailParamChange(string key, int value)
    {
        if (suppressChangeTracking || !ActiveWriteSync || !IsConnected)
        {
            return;
        }

        pendingWrites[key] = (byte)Math.Clamp(value, 0, 127);
        AppendLog($"Queued detail param sync: {key} -> {value}.");
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

        pendingWrites[parameter.Key] = value;
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

    /// <summary>Call when the main window is closing to stop all background timers immediately.</summary>
    public void Shutdown()
    {
        isShuttingDown = true;
        writeSyncTimer.Stop();
        readSyncTimer.Stop();
    }

    private async Task FlushPendingWritesAsync()
    {
        if (isShuttingDown || !ActiveWriteSync || !IsConnected)
        {
            return;
        }

        if (!HasPendingWrites())
        {
            return;
        }

        if (!syncOperationGate.Wait(0))
        {
            AppendLog("Queued write sync is already running; the latest changes will wait for the next flush.");
            return;
        }

        var snapshot = new Dictionary<string, byte>(StringComparer.Ordinal);
        string? channelSnapshot = null;

        try
        {
            PauseActiveReadSync("queued write sync is flushing");

            foreach (var entry in pendingWrites)
                snapshot[entry.Key] = entry.Value;
            channelSnapshot = pendingPanelChannel;

            pendingWrites.Clear();
            pendingPanelChannel = null;

            var ampSnapshot = snapshot
                .Where(e => ampControlsByKey.ContainsKey(e.Key))
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.Ordinal);

            var panelSwitchSnapshot = snapshot
                .Where(e => panelEffectsByKey.ContainsKey(e.Key))
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.Ordinal);

            var panelTypeSnapshot = snapshot
                .Where(e => PanelEffects.Any(ef => ef.Definition.TypeParameter?.Key == e.Key))
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.Ordinal);

            var pedalKeys = new HashSet<string>(PedalFx.GetReadParameters().Select(p => p.Key), StringComparer.Ordinal);

            var pedalSnapshot = snapshot
                .Where(e => pedalKeys.Contains(e.Key))
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.Ordinal);

            var detailParamSnapshot = snapshot
                .Where(e => !ampSnapshot.ContainsKey(e.Key) && !panelSwitchSnapshot.ContainsKey(e.Key)
                         && !panelTypeSnapshot.ContainsKey(e.Key) && !pedalSnapshot.ContainsKey(e.Key))
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.Ordinal);

            AppendLog(
                $"Flushing queued sync: {ampSnapshot.Count} amp, {panelSwitchSnapshot.Count} panel, {panelTypeSnapshot.Count} panel type, {pedalSnapshot.Count} pedal, {detailParamSnapshot.Count} detail, {(channelSnapshot is null ? "no" : "1")} channel change.");

            await FlushPendingAmpWritesAsync(ampSnapshot);
            await FlushPendingPanelWritesAsync(channelSnapshot, panelSwitchSnapshot, panelTypeSnapshot, snapshot);
            await FlushPendingPedalWritesAsync(pedalSnapshot);
            await FlushPendingDetailParamWritesAsync(detailParamSnapshot);

            if (ActiveReadSync)
            {
                AppendLog("Active read sync is enabled; refreshing written state from the Katana.");
                await RefreshWrittenStateAsync(ampSnapshot, panelSwitchSnapshot, panelTypeSnapshot, pedalSnapshot, channelSnapshot);
            }
            else
            {
                AppendLog("Active read sync is disabled; skipping post-write refresh.");
            }

            if (snapshot.Count > 0 || channelSnapshot is not null)
            {
                StatusMessage = "Queued changes synced to the Katana.";
                AppendLog("Queued changes synced to the Katana.");
            }

            flushRetryCount = 0;
        }
        catch (Exception ex)
        {
            foreach (var entry in snapshot)
            {
                pendingWrites[entry.Key] = entry.Value;
            }

            pendingPanelChannel ??= channelSnapshot;
            flushRetryCount++;
            var backoffMs = Math.Min(100 * (1 << flushRetryCount), 2000);
            StatusMessage = ex.Message;
            AppendLog($"Queued write sync failed (retry {flushRetryCount}, backoff {backoffMs}ms). Re-queued {DescribePendingWrites()}.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
            await Task.Delay(backoffMs);
        }
        finally
        {
            ResumeActiveReadSync("queued write sync finished");
            syncOperationGate.Release();
        }
    }

    private async Task RunActiveReadSyncCycleAsync()
    {
        if (isShuttingDown || !ShouldRunActiveReadSync())
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
        IReadOnlyDictionary<string, byte> panelSwitchWrites,
        IReadOnlyDictionary<string, byte> panelTypeWrites,
        IReadOnlyDictionary<string, byte> allWrites)
    {
        if (!string.IsNullOrWhiteSpace(channel))
        {
            AppendLog($"Writing queued panel channel: {channel}.");
            await katanaSession.SelectPanelChannelAsync(ParsePanelChannelDisplay(channel));
        }

        foreach (var entry in panelSwitchWrites)
        {
            var parameter = panelEffectsByKey[entry.Key].Definition.SwitchParameter;
            AppendLog($"Writing queued panel effect: {panelEffectsByKey[entry.Key].DisplayName} -> {(entry.Value != 0 ? "On" : "Off")}.");
            await katanaSession.WriteBlockAsync(parameter.Address, [entry.Value]);
        }

        foreach (var entry in panelTypeWrites)
        {
            var effect = PanelEffects.First(panel => panel.Definition.TypeParameter?.Key == entry.Key);
            AppendLog($"Writing queued panel type: {effect.DisplayName} -> {effect.ToTypeOption(entry.Value)}.");
            await katanaSession.WriteBlockAsync(effect.Definition.TypeParameter!.Address, [entry.Value]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.AmpType.Key, out var ampTypeValue))
        {
            AppendLog($"Writing queued amp type: {(ampTypeValue < AmpTypeOptions.Length ? AmpTypeOptions[ampTypeValue] : ampTypeValue.ToString())}.");
            await katanaSession.WriteBlockAsync(KatanaMkIIParameterCatalog.AmpType.Address, [ampTypeValue]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.CabinetResonance.Key, out var cabinetValue))
        {
            AppendLog($"Writing queued cabinet resonance: {(cabinetValue < CabinetResonanceOptions.Length ? CabinetResonanceOptions[cabinetValue] : cabinetValue.ToString())}.");
            await katanaSession.WriteBlockAsync(KatanaMkIIParameterCatalog.CabinetResonance.Address, [cabinetValue]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.AmpVariation.Key, out var ampVariationValue))
        {
            AppendLog($"Writing queued amp variation: {(ampVariationValue == 0 ? "TYPE 1" : "TYPE 2")}.");
            await katanaSession.WriteBlockAsync(KatanaMkIIParameterCatalog.AmpVariation.Address, [ampVariationValue]);
        }

        if (allWrites.TryGetValue(KatanaMkIIParameterCatalog.ChainPattern.Key, out var chainPatternValue))
        {
            var name = chainPatternValue < PedalboardViewModel.ChainPatternNames.Length ? PedalboardViewModel.ChainPatternNames[chainPatternValue] : chainPatternValue.ToString();
            AppendLog($"Writing queued chain pattern: {name}.");
            await katanaSession.WriteBlockAsync(KatanaMkIIParameterCatalog.ChainPattern.Address, [chainPatternValue]);
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

    private async Task FlushPendingDetailParamWritesAsync(IReadOnlyDictionary<string, byte> detailWrites)
    {
        if (detailWrites.Count == 0)
        {
            return;
        }

        // Build a lookup: definition key -> KatanaParameterDefinition from all sync parameters
        var allDefinitions = PanelEffects
            .SelectMany(e => e.GetSyncParameters())
            .GroupBy(p => p.Key, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);

        var orderedParameters = detailWrites.Keys
            .Where(allDefinitions.ContainsKey)
            .Select(k => allDefinitions[k])
            .OrderBy(p => p.Address[0])
            .ThenBy(p => p.Address[1])
            .ThenBy(p => p.Address[2])
            .ThenBy(p => p.Address[3])
            .ToList();

        foreach (var parameter in orderedParameters)
        {
            var value = detailWrites[parameter.Key];
            AppendLog($"Writing detail param: {parameter.DisplayName} -> {value}.");
            await katanaSession.WriteBlockAsync(parameter.Address.ToArray(), [value]);
        }
    }

    private async Task RefreshWrittenStateAsync(
        IReadOnlyDictionary<string, byte> ampWrites,
        IReadOnlyDictionary<string, byte> panelSwitchWrites,
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

            ApplyDeviceState(() =>
            {
                foreach (var parameter in ampParameters)
                {
                    var control = ampControlsByKey[parameter.Key];
                    var expectedValue = ampWrites[parameter.Key];
                    if (pendingWrites.ContainsKey(parameter.Key) || control.Value != expectedValue)
                    {
                        AppendLog($"Skipping stale amp readback for {control.DisplayName}; a newer local value is pending.");
                        continue;
                    }

                    control.Value = values[parameter.Key];
                }
            });
        }

        if (panelSwitchWrites.Count > 0)
        {
            var panelParameters = panelSwitchWrites.Keys
                .Distinct(StringComparer.Ordinal)
                .Select(key => panelEffectsByKey[key].Definition.SwitchParameter)
                .ToArray();
            AppendLog($"Read sync refreshing panel values: {DescribePanelKeys(panelSwitchWrites.Keys)}.");
            var values = await katanaSession.ReadParametersAsync(panelParameters);

            ApplyDeviceState(() =>
            {
                foreach (var parameter in panelParameters)
                {
                    var effect = panelEffectsByKey[parameter.Key];
                    var expectedValue = panelSwitchWrites[parameter.Key];
                    if (pendingWrites.ContainsKey(parameter.Key) || effect.IsEnabled != (expectedValue != 0))
                    {
                        AppendLog($"Skipping stale panel readback for {effect.DisplayName}; a newer local value is pending.");
                        continue;
                    }

                    effect.IsEnabled = values[parameter.Key] != 0;
                }
            });
        }

        if (panelTypeWrites.Count > 0)
        {
            var typeParameters = PanelEffects
                .Where(effect => effect.Definition.TypeParameter is not null && panelTypeWrites.ContainsKey(effect.Definition.TypeParameter.Key))
                .Select(effect => effect.Definition.TypeParameter!)
                .ToArray();
            AppendLog($"Read sync refreshing panel types: {string.Join(", ", typeParameters.Select(parameter => parameter.DisplayName))}.");
            var values = await katanaSession.ReadParametersAsync(typeParameters);

            ApplyDeviceState(() =>
            {
                foreach (var parameter in typeParameters)
                {
                    var effect = PanelEffects.First(panel => panel.Definition.TypeParameter?.Key == parameter.Key);
                    var expectedValue = panelTypeWrites[parameter.Key];
                    if (pendingWrites.ContainsKey(parameter.Key) ||
                        !effect.TryGetTypeValue(effect.SelectedTypeOption, out var currentValue) ||
                        currentValue != expectedValue)
                    {
                        AppendLog($"Skipping stale panel type readback for {effect.DisplayName}; a newer local value is pending.");
                        continue;
                    }

                    effect.SelectedTypeOption = effect.ToTypeOption(values[parameter.Key]);
                }
            });
        }

        if (pedalWrites.Count > 0)
        {
            var pedalParameters = pedalWrites.Keys
                .Select(PedalFx.GetParameter)
                .DistinctBy(parameter => parameter.Key)
                .ToArray();
            AppendLog($"Read sync refreshing pedal values: {DescribePedalKeys(pedalWrites.Keys)}.");
            var values = await katanaSession.ReadParametersAsync(pedalParameters);

            ApplyDeviceState(() =>
            {
                foreach (var parameter in pedalParameters)
                {
                    var expectedValue = pedalWrites[parameter.Key];
                    if (pendingWrites.ContainsKey(parameter.Key) ||
                        !PedalFx.TryGetCurrentValue(parameter.Key, out var currentValue) ||
                        currentValue != expectedValue)
                    {
                        AppendLog($"Skipping stale pedal readback for {parameter.DisplayName}; a newer local value is pending.");
                        continue;
                    }

                    ApplyPedalValue(parameter.Key, values[parameter.Key]);
                }
            });
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

                ApplyDeviceState(() => SelectedPanelChannel = ToPanelChannelDisplay(currentChannel.Value));
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

    private void ApplyDeviceState(Action apply)
    {
        suppressChangeTracking = true;
        try { apply(); }
        finally { suppressChangeTracking = false; }
    }

    /// <summary>
    /// Builds a flat dictionary mapping each known parameter's address key to the VM action
    /// that should be called when the amp pushes a new value for that parameter.
    /// Called once after connect so subscriptions to PushNotificationReceived work immediately.
    /// </summary>
    private void BuildPushHandlerLookup()
    {
        pushHandlerLookup = new Dictionary<string, Action<byte>>(StringComparer.Ordinal);

        // Amp editor controls (gain, volume, bass, middle, treble, presence).
        foreach (var control in AmpControls)
        {
            var captured = control;
            pushHandlerLookup[AddressToKey(captured.Parameter.Address)] = value => captured.Value = value;
        }

        // All panel effects: delegate value mapping to each pedal's own ApplyAmpValues method
        // so it can interpret the raw byte according to its type tables, variation, etc.
        foreach (var effect in PanelEffects)
        {
            foreach (var param in effect.GetSyncParameters())
            {
                var capturedEffect = effect;
                var capturedKey = param.Key;
                pushHandlerLookup[AddressToKey(param.Address)] = value =>
                    capturedEffect.ApplyAmpValues(new Dictionary<string, int>(StringComparer.Ordinal)
                    {
                        [capturedKey] = value,
                    });
            }
        }

        // Standalone panel parameters.
        pushHandlerLookup[AddressToKey(KatanaMkIIParameterCatalog.AmpType.Address)] = value =>
        {
            if (value < AmpTypeOptions.Length) SelectedAmpType = AmpTypeOptions[value];
        };
        pushHandlerLookup[AddressToKey(KatanaMkIIParameterCatalog.CabinetResonance.Address)] = value =>
        {
            if (value < CabinetResonanceOptions.Length) SelectedCabinetResonance = CabinetResonanceOptions[value];
        };
        pushHandlerLookup[AddressToKey(KatanaMkIIParameterCatalog.AmpVariation.Address)] = value =>
        {
            IsAmpVariation = value != 0;
        };
        // Panel channel (PATCH NUM) — address 00-01-00-00, INTEGER2x7.
        // The amp pushes this as a 16-byte DT1 when the user presses a channel button.
        pushHandlerLookup[AddressToKey([0x00, 0x01, 0x00, 0x00])] = value =>
        {
            var displayName = value switch
            {
                0 => "Panel",
                1 => "CH A1",
                2 => "CH A2",
                5 => "CH B1",
                6 => "CH B2",
                _ => null,
            };
            if (displayName is not null)
            {
                AppendLog($"Amp channel changed (push): {displayName}");
                SelectPanelChannel(displayName);
                // Stop the background poller immediately so it doesn't race with the amp
                // while it's still settling after the channel switch. The poller will be
                // resumed after we finish the full re-read sequence below.
                PauseActiveReadSync("channel change in progress");
                // Re-read all state from the amp — the new channel has its own parameter values.
                // A brief delay lets the amp finish its internal channel switch before responding
                // to SysEx reads. Reads are sequenced (not concurrent) to avoid MIDI contention.
                // InvokeAsync (not Task.Run) keeps continuations on the UI thread so that
                // ApplyDeviceState calls never mutate ObservableCollections off-thread.
                _ = Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await Task.Delay(150);
                    await TryReadAmpControlsAsync(backgroundSync: true);
                    await TryReadPanelControlsAsync(backgroundSync: true);
                    await TryReadPedalControlsAsync(backgroundSync: true);
                    ResumeActiveReadSync("channel change re-read complete");
                });
            }
        };
    }

    /// <summary>
    /// Handles an unsolicited DT1 push from the amp (e.g., user turned a knob on the device).
    /// Only single-byte DT1 messages are handled; block pushes are ignored for now.
    /// Marshals to the UI thread since the callback fires on the MIDI background thread.
    /// </summary>
    private void OnAmpPushNotification(object? sender, SysExMessage message)
    {
        var bytes = message.Bytes;

        if (bytes[7] != 0x12) return; // not a DT1

        // Roland DT1: F0 41 DevID Model(4) 0x12 Addr(4) Data(N) Chksum F7 = 14+N bytes.
        // N=1 → 15 bytes (single-byte parameters, most common).
        // N=2 → 16 bytes (INTEGER2x7 parameters such as PATCH NUM / panel channel).
        // Reject anything that doesn't fit these two cases.
        if (bytes.Count != 15 && bytes.Count != 16) return;

        var addressKey = AddressToKey([bytes[8], bytes[9], bytes[10], bytes[11]]);

        // For 2-byte data the meaningful value is always the second (lower) byte.
        var value = bytes.Count == 16 ? bytes[13] : bytes[12];

        if (!pushHandlerLookup.TryGetValue(addressKey, out var action))
        {
            return;
        }

        // Marshal to the UI thread — this callback fires on the ALSA background thread.
        Dispatcher.UIThread.Post(() => ApplyDeviceState(() => action(value)));
    }

    /// <summary>
    /// Handles a channel-change Program Change pushed by the amp when the user presses
    /// CH1/CH2/Panel on the front panel. Updates <see cref="SelectedPanelChannel"/> without
    /// enqueuing a write back to the amp (suppressChangeTracking guard).
    /// </summary>
    private void OnAmpPanelChannelChanged(object? sender, KatanaPanelChannel channel)
    {
        var displayName = channel switch
        {
            KatanaPanelChannel.ChA1 => "CH A1",
            KatanaPanelChannel.ChA2 => "CH A2",
            KatanaPanelChannel.ChB1 => "CH B1",
            KatanaPanelChannel.ChB2 => "CH B2",
            _                       => "Panel",
        };

        // Marshal to the UI thread — this callback fires on the ALSA background thread.
        Dispatcher.UIThread.Post(() =>
        {
            AppendLog($"Amp channel changed (push): {displayName}");
            ApplyDeviceState(() => SelectPanelChannel(displayName));
        });
    }

    /// <summary>
    /// Formats a 4-byte address as a hex key string (e.g. "60-00-06-50") for dictionary lookup.
    /// Matches the format used by DryWetMidiConnection's reply correlator.
    /// </summary>
    private static string AddressToKey(IReadOnlyList<byte> address) =>
        $"{address[0]:X2}-{address[1]:X2}-{address[2]:X2}-{address[3]:X2}";

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
                DelayTimeMs = null;
                DelayTapStatus = "Delay time is not currently readable for the active effect state.";
                AppendLog("Delay time reply did not contain a usable millisecond value.");
                return false;
            }

            DelayTimeMs = delayTime;
            DelayTapStatus = $"Delay time loaded: {delayTime} ms.";
            AppendLog($"Delay time reply: {delayTime} ms.");
            return true;
        }
        catch (Exception ex)
        {
            DelayTimeMs = null;
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
            ApplyDeviceState(() =>
            {
                foreach (var control in AmpControls)
                {
                    var value = values[control.Parameter.Key];
                    control.Value = value;
                    AppendLog($"{control.DisplayName} reply: {value}");
                }
            });

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
                ApplyDeviceState(() =>
                {
                    SelectedPanelChannel = ToPanelChannelDisplay(currentChannel.Value);
                    AppendLog($"Current panel channel: {SelectedPanelChannel}");
                });
            }

            var parameterList = PanelEffects
                .SelectMany(e => e.GetSyncParameters())
                .Append(KatanaMkIIParameterCatalog.AmpType)
                .Append(KatanaMkIIParameterCatalog.CabinetResonance)
                .Append(KatanaMkIIParameterCatalog.AmpVariation)
                .Append(KatanaMkIIParameterCatalog.ChainPattern)
                .ToArray();
            var values = await katanaSession.ReadParametersAsync(parameterList);

            ApplyDeviceState(() =>
            {
                var intValues = values.ToDictionary(kvp => kvp.Key, kvp => (int)kvp.Value, StringComparer.Ordinal);
                foreach (var effect in PanelEffects)
                {
                    effect.ApplyAmpValues(intValues);
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
                if (values.TryGetValue(KatanaMkIIParameterCatalog.AmpVariation.Key, out var varValue))
                {
                    IsAmpVariation = varValue != 0;
                }
                if (values.TryGetValue(KatanaMkIIParameterCatalog.ChainPattern.Key, out var chainValue) &&
                    chainValue < PedalboardViewModel.ChainPatternNames.Length)
                {
                    Pedalboard.SelectedChainPattern = chainValue;
                }
                AppendLog($"Amp Type: {SelectedAmpType} ({(IsAmpVariation ? "TYPE 2" : "TYPE 1")}) / Cabinet Resonance: {SelectedCabinetResonance} / Chain: {PedalboardViewModel.ChainPatternNames[Pedalboard.SelectedChainPattern]}");
            });

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

            ApplyDeviceState(() =>
            {
                PedalFx.ApplyValues(values);
                AppendLog($"Pedal FX: {(PedalFx.IsEnabled ? "On" : "Off")} / {PedalFx.SelectedTypeOption} / {PedalFx.SelectedPositionOption} / Foot Volume {PedalFx.FootVolume}");
                if (PedalFx.IsWahMode)
                {
                    AppendLog($"Pedal Wah: {PedalFx.SelectedWahTypeOption} / Pos {PedalFx.PedalPosition} / Min {PedalFx.PedalMinimum} / Max {PedalFx.PedalMaximum} / Level {PedalFx.EffectLevel} / Direct {PedalFx.DirectMix}");
                }
            });

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
