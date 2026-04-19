using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class OctaveState
{
    public AmpControlState DirectMix;
    public AmpControlState EffectLevel;

    public AmpControlState Range;

    public OctaveState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModOctaveParams : KatanaMkIIParameterCatalog.FxOctaveParams;
        Range = new AmpControlState(p[0], description: "Selects the register to which the effect is applied.");
        EffectLevel = new AmpControlState(p[1], description: "Adjusts the volume of the sound one octave lower.");
        DirectMix = new AmpControlState(p[2], description: "Adjusts the volume of the direct sound.");
    }
}
