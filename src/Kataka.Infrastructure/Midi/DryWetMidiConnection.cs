using Kataka.Application.Midi;
using Kataka.Domain.Midi;
using Melanchall.DryWetMidi.Multimedia;

namespace Kataka.Infrastructure.Midi;

internal sealed class DryWetMidiConnection(InputDevice inputDevice, OutputDevice outputDevice) : IMidiConnection
{
    public ValueTask DisposeAsync()
    {
        inputDevice.Dispose();
        outputDevice.Dispose();
        return ValueTask.CompletedTask;
    }

    public Task SendAsync(SysExMessage message, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("SysEx send is not wired yet. The current slice only supports MIDI port discovery.");
    }

    public Task<SysExMessage> RequestAsync(
        SysExMessage request,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("SysEx request/response is not wired yet. The current slice only supports MIDI port discovery.");
    }
}
