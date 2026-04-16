using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class FlangerState
{
    public FlangerState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModFlangerParams : KatanaMkIIParameterCatalog.FxFlangerParams;
        Rate = new AmpControlState(p[0], description: "This sets the rate of the flanging effect.");
        Depth = new AmpControlState(p[1], description: "Determines the depth of the flanging effect.");
        Manual = new AmpControlState(p[2], description: "Adjusts the center frequency at which to apply the effect.");
        Resonance = new AmpControlState(p[3], description: "Determines the amount of resonance (feedback). Increasing the value will emphasize the effect, creating a more unusual sound.");
        LowCut = new AmpControlState(p[4], description: "Sets the frequency at which the low cut filter begins to take effect.");
        EffectLevel = new AmpControlState(p[5], description: "Adjusts the volume of the flanger.");
        DirectMix = new AmpControlState(p[6], description: "Adjusts the volume of the direct sound.");
    }

    public AmpControlState Rate;
    public AmpControlState Depth;
    public AmpControlState Manual;
    public AmpControlState Resonance;
    public AmpControlState LowCut;
    public AmpControlState EffectLevel;
    public AmpControlState DirectMix;
}
