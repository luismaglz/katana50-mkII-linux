using Kataka.Application.Midi;
using Kataka.Domain.Midi;
using Melanchall.DryWetMidi.Multimedia;

namespace Kataka.Infrastructure.Midi;

public sealed class DryWetMidiTransport : IMidiTransport
{
    public Task<IReadOnlyList<MidiPortDescriptor>> ListPortsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var ports = new List<MidiPortDescriptor>();

        ports.AddRange(InputDevice
            .GetAll()
            .Select(device => new MidiPortDescriptor(device.Name, device.Name, MidiPortDirection.Input)));

        ports.AddRange(OutputDevice
            .GetAll()
            .Select(device => new MidiPortDescriptor(device.Name, device.Name, MidiPortDirection.Output)));

        return Task.FromResult<IReadOnlyList<MidiPortDescriptor>>(ports);
    }

    public Task<IMidiConnection> OpenAsync(
        string inputPortId,
        string outputPortId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentException.ThrowIfNullOrWhiteSpace(inputPortId);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPortId);

        var inputDevice = InputDevice.GetByName(inputPortId);
        var outputDevice = OutputDevice.GetByName(outputPortId);

        return Task.FromResult<IMidiConnection>(new DryWetMidiConnection(inputDevice, outputDevice));
    }
}
