using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class PedalBendState
{
    public PedalBendState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModPedalBendParams : KatanaMkIIParameterCatalog.FxPedalBendParams;
        Pitch = new AmpControlState(p[0], description: "This sets the pitch at the point where the expression pedal is all the way down.");
        PedalPos = new AmpControlState(p[1], description: "Adjusts the pedal position for pedal bend. This parameter is used after it has been assigned to an expression pedal or similar controller.");
        EffectLevel = new AmpControlState(p[2], description: "Adjusts the volume of the effect sound.");
        DirectMix = new AmpControlState(p[3], description: "Adjusts the volume of the direct sound.");
    }

    public AmpControlState Pitch;
    public AmpControlState PedalPos;
    public AmpControlState EffectLevel;
    public AmpControlState DirectMix;
}
