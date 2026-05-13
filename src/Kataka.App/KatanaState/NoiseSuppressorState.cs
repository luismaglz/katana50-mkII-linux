using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public class NoiseSuppressorState
{
    public AmpControlState Sw = new(KatanaMkIIParameterCatalog.NoiseSuppressorSw);
    public AmpControlState Threshold = new(KatanaMkIIParameterCatalog.NoiseSuppressorThreshold);
    public AmpControlState Release = new(KatanaMkIIParameterCatalog.NoiseSuppressorRelease);
}
