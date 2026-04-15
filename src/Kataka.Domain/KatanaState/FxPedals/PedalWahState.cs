using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class PedalWahState
{
    public PedalWahState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModPedalWahParams : KatanaMkIIParameterCatalog.FxPedalWahParams;
        Type        = new AmpControlState(p[0], description: "Selects the wah mode.");
        PedalPos    = new AmpControlState(p[1], description: "Adjusts the position of the wah pedal. This parameter is used after it has been assigned to an EXP Pedal or similar controller.");
        PedalMin    = new AmpControlState(p[2], description: "Selects the tone produced when the heel of the EXP Pedal is depressed.");
        PedalMax    = new AmpControlState(p[3], description: "Selects the tone produced when the toe of the EXP Pedal is depressed.");
        EffectLevel = new AmpControlState(p[4], description: "Adjusts the volume of the effect sound.");
        DirectMix   = new AmpControlState(p[5], description: "Adjusts the volume of the direct sound.");
    }

    public AmpControlState Type;
    public AmpControlState PedalPos;
    public AmpControlState PedalMin;
    public AmpControlState PedalMax;
    public AmpControlState EffectLevel;
    public AmpControlState DirectMix;
}
