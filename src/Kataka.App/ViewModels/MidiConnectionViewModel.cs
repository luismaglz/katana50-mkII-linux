using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using Kataka.App.Services;
using Kataka.Application.Katana;
using Kataka.Domain.Midi;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class MidiConnectionViewModel : ViewModelBase
{
    private readonly IKatanaSession _katanaSession;
    private readonly IAmpSyncService _syncService;
    private readonly Action<string> _appendStatus;
    private readonly Action<string> _appendLog;
    private readonly Dictionary<string, string> _inputPortIds = [];
    private readonly Dictionary<string, string> _outputPortIds = [];

    public MidiConnectionViewModel(
        IKatanaSession katanaSession,
        IAmpSyncService syncService,
        Action<string> appendStatus,
        Action<string> appendLog)
    {
        _katanaSession = katanaSession;
        _syncService = syncService;
        _appendStatus = appendStatus;
        _appendLog = appendLog;

        this.WhenAnyValue(x => x.IsConnected)
            .Subscribe(v => syncService.OnConnectionChanged(v));
    }

    public ObservableCollection<string> InputPorts { get; } = [];
    public ObservableCollection<string> OutputPorts { get; } = [];

    [Reactive] public string? SelectedInputPort { get; set; }
    [Reactive] public string? SelectedOutputPort { get; set; }
    [Reactive] public bool IsScanning { get; set; }
    [Reactive] public bool IsKatanaDetected { get; set; }
    [Reactive] public string DetectionMessage { get; set; } = "No scan has been run yet.";
    [Reactive] public bool IsConnected { get; set; }
    [Reactive] public string IdentityReply { get; set; } = "Identity request has not been run yet.";

    [RelayCommand]
    private async Task ScanAsync()
    {
        await _katanaSession.DisconnectAsync();
        IsConnected = false;
        IsScanning = true;
        _appendStatus("Scanning MIDI ports...");
        DetectionMessage = "Looking for Katana input/output ports.";
        IsKatanaDetected = false;
        _appendLog("Starting MIDI port scan.");
        SelectedInputPort = null;
        SelectedOutputPort = null;
        InputPorts.Clear();
        OutputPorts.Clear();
        _inputPortIds.Clear();
        _outputPortIds.Clear();

        try
        {
            var ports = await _katanaSession.ListPortsAsync();
            _appendLog($"Port scan returned {ports.Count} total port(s).");

            foreach (var port in ports.Where(p => p.Direction == MidiPortDirection.Input).OrderBy(p => p.Name))
            {
                InputPorts.Add(port.Name);
                _inputPortIds[port.Name] = port.Id;
                _appendLog($"Input: {port.Name}");
            }

            foreach (var port in ports.Where(p => p.Direction == MidiPortDirection.Output).OrderBy(p => p.Name))
            {
                OutputPorts.Add(port.Name);
                _outputPortIds[port.Name] = port.Id;
                _appendLog($"Output: {port.Name}");
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
                _appendStatus("Katana-style MIDI ports found. You can connect to the selected pair.");
                _appendLog("Katana-style port pair auto-selected.");
                return;
            }

            DetectionMessage = "No Katana MIDI port pair was detected.";
            _appendStatus($"Scan complete. Found {InputPorts.Count} input port(s) and {OutputPorts.Count} output port(s).");
            _appendLog("No Katana-style port pair found.");
        }
        catch (Exception ex)
        {
            DetectionMessage = "MIDI scan failed.";
            _appendStatus(ex.Message);
            _appendLog("MIDI scan failed.");
            _appendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
        finally
        {
            IsScanning = false;
            _appendLog("MIDI port scan finished.");
        }
    }

    [RelayCommand]
    private async Task ConnectAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedInputPort) || string.IsNullOrWhiteSpace(SelectedOutputPort))
        {
            _appendStatus("Select both an input port and an output port before connecting.");
            return;
        }

        try
        {
            _appendLog($"Opening input '{SelectedInputPort}' and output '{SelectedOutputPort}'.");
            if (!_inputPortIds.TryGetValue(SelectedInputPort, out var inputPortId))
                throw new InvalidOperationException($"Input port '{SelectedInputPort}' is not available.");
            if (!_outputPortIds.TryGetValue(SelectedOutputPort, out var outputPortId))
                throw new InvalidOperationException($"Output port '{SelectedOutputPort}' is not available.");

            await _katanaSession.ConnectAsync(inputPortId, outputPortId);
            IsConnected = true;
            _syncService.Activate();

            var looksLikeKatana =
                SelectedInputPort.Contains("katana", StringComparison.OrdinalIgnoreCase) &&
                SelectedOutputPort.Contains("katana", StringComparison.OrdinalIgnoreCase);

            DetectionMessage = looksLikeKatana
                ? $"Connected to Katana-style ports: {SelectedInputPort} / {SelectedOutputPort}"
                : $"Connected to selected MIDI ports: {SelectedInputPort} / {SelectedOutputPort}";
            _appendStatus("MIDI ports opened successfully.");
            _appendLog("MIDI ports opened successfully.");

            _appendLog("Auto-loading amp, panel, and pedal state.");
            await _syncService.TryRefreshAmpStateAsync();
        }
        catch (Exception ex)
        {
            IsConnected = false;
            DetectionMessage = "Connection failed.";
            _appendStatus(ex.Message);
            _appendLog("Connection failed.");
            _appendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
    }

    [RelayCommand]
    private async Task DisconnectAsync()
    {
        if (!_katanaSession.IsConnected)
        {
            _appendStatus("No MIDI connection is currently open.");
            return;
        }

        _syncService.Deactivate();
        await _katanaSession.DisconnectAsync();
        IsConnected = false;
        _appendStatus("Disconnected from the selected MIDI ports.");
        DetectionMessage = "Connection closed.";
        _appendLog("MIDI connection closed.");
    }

    [RelayCommand]
    private async Task RequestIdentityAsync()
    {
        if (!_katanaSession.IsConnected)
        {
            _appendStatus("Connect to a MIDI port before requesting identity.");
            return;
        }

        try
        {
            _appendLog("Sending universal device identity request.");
            var reply = await _katanaSession.RequestIdentityAsync();
            IdentityReply = reply.ToHexString();
            DetectionMessage = UniversalDeviceIdentity.IsIdentityReply(reply)
                ? "Identity reply received."
                : "A SysEx reply was received, but it did not match the standard identity pattern.";
            _appendStatus("Identity request completed.");
            _appendLog($"Identity reply: {IdentityReply}");
        }
        catch (Exception ex)
        {
            IdentityReply = "Identity request failed.";
            DetectionMessage = "Identity request failed.";
            _appendStatus(ex.Message);
            _appendLog("Identity request failed.");
            _appendLog(ex.ToString());
            Console.Error.WriteLine(ex);
        }
    }
}
