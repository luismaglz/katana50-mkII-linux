using Kataka.Application.Midi;

namespace Kataka.Infrastructure.Midi;

public static class DefaultMidiTransport
{
    public static IMidiTransport Create() => new AmidiTransport();
}
