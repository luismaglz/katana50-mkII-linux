using Kataka.Domain.Midi;

namespace Kataka.Application.Midi;

public interface IMidiTransport
{
    Task<IReadOnlyList<MidiPortDescriptor>> ListPortsAsync(CancellationToken cancellationToken = default);

    Task<IMidiConnection> OpenAsync(
        string inputPortId,
        string outputPortId,
        CancellationToken cancellationToken = default);
}
