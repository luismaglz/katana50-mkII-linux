using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class GuitarSimState
{
    public GuitarSimState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModGuitarSimParams : KatanaMkIIParameterCatalog.FxGuitarSimParams;
        Type  = new AmpControlState(p[0], description: "Selects the type of the guitar simulator.");
        Low   = new AmpControlState(p[1], description: "Adjusts the low frequency range tone.");
        High  = new AmpControlState(p[2], description: "Adjusts the high frequency range tone.");
        Level = new AmpControlState(p[3], description: "Adjusts the volume of the effect sound.");
        Body  = new AmpControlState(p[4], description: "Adjusts the way the body sounds when TYPE is set to a hollow or acoustic type.");
    }

    public AmpControlState Type;
    public AmpControlState Low;
    public AmpControlState High;
    public AmpControlState Level;
    public AmpControlState Body;
}
