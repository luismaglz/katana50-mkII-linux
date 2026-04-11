using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kataka.Application.Katana;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IKatanaSession katanaSession;
    private readonly Dictionary<string, string> inputPortIds = [];
    private readonly Dictionary<string, string> outputPortIds = [];

    public MainWindowViewModel()
        : this(new KatanaSession(Kataka.Infrastructure.Midi.DefaultMidiTransport.Create()))
    {
    }

    public MainWindowViewModel(IKatanaSession katanaSession)
    {
        this.katanaSession = katanaSession;

        foreach (var parameter in KatanaMkIIParameterCatalog.AmpEditorControls)
        {
            AmpControls.Add(new AmpControlViewModel(parameter));
        }

        foreach (var effect in KatanaMkIIParameterCatalog.PanelEffects)
        {
            PanelEffects.Add(new PanelEffectViewModel(effect));
        }
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
    public partial string SelectedPanelChannel { get; set; } = "Panel";

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

        try
        {
            AppendLog("Reading Katana amp editor controls.");

            foreach (var control in AmpControls)
            {
                var value = await katanaSession.ReadParameterAsync(control.Parameter);
                control.Value = value;
                AppendLog($"{control.DisplayName} reply: {value}");
            }

            StatusMessage = "Amp editor controls read successfully.";
            AmpEditorStatus = "Amp editor values were loaded from the Katana.";
        }
        catch (Exception ex)
        {
            AmpEditorStatus = "Amp editor read failed.";
            StatusMessage = ex.Message;
            AppendLog("Amp editor read failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
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
    }

    [RelayCommand]
    private async Task ReadPanelControlsAsync()
    {
        if (!katanaSession.IsConnected)
        {
            StatusMessage = "Connect to a MIDI port before reading panel controls.";
            return;
        }

        try
        {
            AppendLog("Reading Katana panel controls.");

            var currentChannel = await katanaSession.ReadCurrentPanelChannelAsync();
            if (currentChannel is not null)
            {
                SelectedPanelChannel = ToPanelChannelDisplay(currentChannel.Value);
                AppendLog($"Current panel channel: {SelectedPanelChannel}");
            }

            foreach (var effect in PanelEffects)
            {
                effect.IsEnabled = await katanaSession.ReadParameterAsync(effect.Definition.SwitchParameter) != 0;
                effect.Variation = effect.Definition.VariationParameter is null
                    ? "N/A"
                    : ToVariationDisplay(await katanaSession.ReadParameterAsync(effect.Definition.VariationParameter));
                AppendLog($"{effect.DisplayName}: {(effect.IsEnabled ? "On" : "Off")} / {effect.Variation}");
            }

            StatusMessage = "Panel controls read successfully.";
            PanelControlsStatus = "Panel channel, effect toggles, and variation colors were loaded.";
        }
        catch (Exception ex)
        {
            PanelControlsStatus = "Panel control read failed.";
            StatusMessage = ex.Message;
            AppendLog("Panel control read failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
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

            var channel = ParsePanelChannelDisplay(SelectedPanelChannel);
            await katanaSession.SelectPanelChannelAsync(channel);
            AppendLog($"Selected panel channel: {SelectedPanelChannel}");

            foreach (var effect in PanelEffects)
            {
                var confirmedValue = await katanaSession.WriteParameterAsync(
                    effect.Definition.SwitchParameter,
                    effect.IsEnabled ? (byte)1 : (byte)0);
                effect.IsEnabled = confirmedValue != 0;
                AppendLog($"{effect.DisplayName} confirmed {(effect.IsEnabled ? "On" : "Off")}.");
            }

            StatusMessage = "Panel controls updated successfully.";
            PanelControlsStatus = "Panel channel and effect toggles were written and confirmed.";
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

    private void AppendLog(string message)
    {
        var line = $"[{DateTimeOffset.Now:HH:mm:ss}] {message}";
        DiagnosticLog = DiagnosticLog == "Diagnostic log ready."
            ? line
            : $"{DiagnosticLog}{Environment.NewLine}{line}";
        Console.WriteLine(line);
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
