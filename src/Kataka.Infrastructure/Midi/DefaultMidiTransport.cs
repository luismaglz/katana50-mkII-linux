using System.Runtime.InteropServices;
using Kataka.Application.Midi;

namespace Kataka.Infrastructure.Midi;

public static class DefaultMidiTransport
{
    public static IMidiTransport Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && AmidiTransport.IsSupported())
        {
            return new AmidiTransport();
        }

        return new DryWetMidiTransport();
    }
}
