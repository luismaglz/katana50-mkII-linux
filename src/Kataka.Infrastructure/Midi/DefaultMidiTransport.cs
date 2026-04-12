using Kataka.Application.Midi;

namespace Kataka.Infrastructure.Midi;

public static class DefaultMidiTransport
{
    public static IMidiTransport Create()
    {
        // Always use DryWetMidiTransport — it keeps the MIDI ports open persistently,
        // enabling event-driven replies and amp push notifications.
        // AmidiTransport (subprocess-per-request) is preserved for debugging but is no longer the default.
        return new DryWetMidiTransport();
    }
}
