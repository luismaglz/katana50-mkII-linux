using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class RingModState
{
    public RingModState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModRingModParams : KatanaMkIIParameterCatalog.FxRingModParams;
        Mode = new AmpControlState(p[0], description: "Selects the mode for the ring modulator. NORMAL is a standard ring modulator; INTELLIGENT modulates the oscillation frequency according to the pitch of the input sound.");
        Frequency = new AmpControlState(p[1], description: "Adjusts the frequency of the internal oscillator.");
        EffectLevel = new AmpControlState(p[2], description: "Adjusts the volume of the effect sound.");
        DirectMix = new AmpControlState(p[3], description: "Adjusts the volume of the direct sound.");
    }

    public AmpControlState Mode;
    public AmpControlState Frequency;
    public AmpControlState EffectLevel;
    public AmpControlState DirectMix;
}
