using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kataka.Application.Midi;
using Kataka.Domain.Midi;
using Kataka.Infrastructure.Midi;

namespace Kataka.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMidiTransport midiTransport;
    private IMidiConnection? activeConnection;
    private readonly Dictionary<string, string> inputPortIds = [];
    private readonly Dictionary<string, string> outputPortIds = [];

    public MainWindowViewModel()
        : this(DefaultMidiTransport.Create())
    {
    }

    public MainWindowViewModel(IMidiTransport midiTransport)
    {
        this.midiTransport = midiTransport;
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

    [ObservableProperty]
    public partial int AmpVolume { get; set; } = 50;

    [ObservableProperty]
    public partial string AmpVolumeStatus { get; set; } = "Amp volume has not been read yet.";

    [RelayCommand]
    private async Task ScanAsync()
    {
        if (activeConnection is not null)
        {
            await activeConnection.DisposeAsync();
            activeConnection = null;
            IsConnected = false;
        }

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
            var ports = await midiTransport.ListPortsAsync();
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

            activeConnection = await midiTransport.OpenAsync(inputPortId, outputPortId);
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
        if (activeConnection is null)
        {
            StatusMessage = "No MIDI connection is currently open.";
            return;
        }

        await activeConnection.DisposeAsync();
        activeConnection = null;
        IsConnected = false;
        StatusMessage = "Disconnected from the selected MIDI ports.";
        DetectionMessage = "Connection closed.";
        AppendLog("MIDI connection closed.");
    }

    [RelayCommand]
    private async Task RequestIdentityAsync()
    {
        if (activeConnection is null)
        {
            StatusMessage = "Connect to a MIDI port before requesting identity.";
            return;
        }

        try
        {
            AppendLog("Sending universal device identity request.");
            var reply = await activeConnection.RequestAsync(
                UniversalDeviceIdentity.CreateIdentityRequest(),
                TimeSpan.FromSeconds(1.5));

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
    private async Task ReadAmpVolumeAsync()
    {
        if (activeConnection is null)
        {
            StatusMessage = "Connect to a MIDI port before reading amp volume.";
            return;
        }

        try
        {
            AppendLog("Requesting current Katana amp volume.");
            var volume = await ReadAmpVolumeAsync(activeConnection);
            StatusMessage = "Amp volume read successfully.";
            AmpVolumeStatus = $"Amp volume is currently {volume}.";
        }
        catch (Exception ex)
        {
            AmpVolumeStatus = "Amp volume read failed.";
            StatusMessage = ex.Message;
            AppendLog("Amp volume read failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
    }

    [RelayCommand]
    private async Task SetAmpVolumeAsync()
    {
        if (activeConnection is null)
        {
            StatusMessage = "Connect to a MIDI port before writing amp volume.";
            return;
        }

        try
        {
            var requestedVolume = Math.Clamp(AmpVolume, 0, 100);
            if (requestedVolume != AmpVolume)
            {
                AmpVolume = requestedVolume;
                AppendLog($"Clamped requested amp volume to {requestedVolume}.");
            }

            AppendLog($"Sending Katana amp volume write for value {requestedVolume}.");
            await activeConnection.SendAsync(KatanaMkIIProtocol.CreateAmpVolumeWriteRequest((byte)requestedVolume));

            AppendLog("Reading amp volume back after write.");
            var confirmedVolume = await ReadAmpVolumeAsync(activeConnection);

            StatusMessage = confirmedVolume == requestedVolume
                ? "Amp volume updated successfully."
                : "Amp volume write completed, but the read-back value differed.";
            AmpVolumeStatus = confirmedVolume == requestedVolume
                ? $"Amp volume confirmed at {confirmedVolume}."
                : $"Requested amp volume {requestedVolume}, but the amp reported {confirmedVolume}.";
        }
        catch (Exception ex)
        {
            AmpVolumeStatus = "Amp volume write failed.";
            StatusMessage = ex.Message;
            AppendLog("Amp volume write failed.");
            AppendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
    }

    private async Task<int> ReadAmpVolumeAsync(IMidiConnection connection)
    {
        var reply = await connection.RequestAsync(
            KatanaMkIIProtocol.CreateAmpVolumeReadRequest(),
            TimeSpan.FromSeconds(1.5));

        AppendLog($"Amp volume reply: {reply.ToHexString()}");

        if (!KatanaMkIIProtocol.TryParseAmpVolumeReply(reply, out var volume))
        {
            throw new InvalidOperationException("Amp volume reply did not match the expected Katana MKII format.");
        }

        AmpVolume = volume;
        return volume;
    }

    private void AppendLog(string message)
    {
        var line = $"[{DateTimeOffset.Now:HH:mm:ss}] {message}";
        DiagnosticLog = DiagnosticLog == "Diagnostic log ready."
            ? line
            : $"{DiagnosticLog}{Environment.NewLine}{line}";
        Console.WriteLine(line);
    }
}
