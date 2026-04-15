using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class AutoWahState
{
    public AutoWahState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModAutoWahParams : KatanaMkIIParameterCatalog.FxAutoWahParams;
        Mode        = new AmpControlState(p[0], description: "Selects the wah mode. LPF provides a wah effect over a wide frequency range; BPF provides a wah effect in a narrow frequency range.");
        Freq        = new AmpControlState(p[1], description: "Adjusts the center frequency of the Wah effect.");
        Peak        = new AmpControlState(p[2], description: "Adjusts the way in which the wah effect applies to the area around the center frequency.");
        Rate        = new AmpControlState(p[3], description: "Adjusts the frequency (speed) of the change.");
        Depth       = new AmpControlState(p[4], description: "Adjusts the depth of the effect.");
        DirectMix   = new AmpControlState(p[5], description: "Adjusts the volume of the direct sound.");
        EffectLevel = new AmpControlState(p[6], description: "Adjusts the volume of the effect sound.");
    }

    public AmpControlState Mode;
    public AmpControlState Freq;
    public AmpControlState Peak;
    public AmpControlState Rate;
    public AmpControlState Depth;
    public AmpControlState DirectMix;
    public AmpControlState EffectLevel;
}
