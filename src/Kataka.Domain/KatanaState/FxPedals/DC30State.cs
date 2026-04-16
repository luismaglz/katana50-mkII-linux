using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class DC30State
{
    public DC30State(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModDC30Params : KatanaMkIIParameterCatalog.FxDC30Params;
        Selector = new AmpControlState(p[0], description: "Selects the effect type (CHORUS or ECHO).");
        InputVolume = new AmpControlState(p[1], description: "Adjusts the input level.");
        ChorusIntensity = new AmpControlState(p[2], description: "Adjusts the depth of the chorus effect. Only when TYPE is CHORUS.");
        EchoIntensity = new AmpControlState(p[3], description: "Adjusts the amount that is returned to the input. A higher value will increase the number of the delay repeats. Only when TYPE is ECHO.");
        EchoVolume = new AmpControlState(p[4], description: "Adjusts the volume of the delay sound. Only when TYPE is ECHO.");
        Tone = new AmpControlState(p[5], description: "Adjusts the tone.");
        Output = new AmpControlState(p[6], description: "Selects how the direct and effect sounds are output (D/E, D+E, or D→E).");
    }

    public AmpControlState Selector;
    public AmpControlState InputVolume;
    public AmpControlState ChorusIntensity;
    public AmpControlState EchoIntensity;
    public AmpControlState EchoVolume;
    public AmpControlState Tone;
    public AmpControlState Output;
}
