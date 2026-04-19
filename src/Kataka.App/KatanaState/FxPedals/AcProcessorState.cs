using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class AcProcessorState
{
    public AmpControlState Bass;
    public AmpControlState Level;
    public AmpControlState Mid;
    public AmpControlState MidFreq;
    public AmpControlState Presence;
    public AmpControlState Treble;

    public AmpControlState Type;

    public AcProcessorState(bool isMod)
    {
        var p = isMod
            ? KatanaMkIIParameterCatalog.ModAcProcessorParams
            : KatanaMkIIParameterCatalog.FxAcProcessorParams;
        Type = new AmpControlState(p[0],
            description:
            "Selects the modeling type. Small models a small-bodied acoustic; Medium is a standard acoustic; Bright is a bright acoustic; Power is a powerful acoustic.");
        Bass = new AmpControlState(p[1], description: "Adjusts the tone for the low frequency range.");
        Mid = new AmpControlState(p[2], description: "Adjusts the midrange balance.");
        MidFreq = new AmpControlState(p[3],
            description: "Specifies the frequency range to be adjusted with the Mid control.");
        Treble = new AmpControlState(p[4], description: "Adjusts the tone for the high frequency range.");
        Presence = new AmpControlState(p[5], description: "Adjusts the balance in the extended upper range.");
        Level = new AmpControlState(p[6], description: "Adjusts the volume.");
    }
}
