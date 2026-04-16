using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class AcGuitarSimState
{
    public AcGuitarSimState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModAcGuitarSimParams : KatanaMkIIParameterCatalog.FxAcGuitarSimParams;
        High = new AmpControlState(p[0], description: "Specifies the sense of volume for the high-frequency range.");
        Body = new AmpControlState(p[1], description: "Adjusts the body resonance.");
        Low = new AmpControlState(p[2], description: "Specifies the sense of volume for the low-frequency range.");
        Level = new AmpControlState(p[3], description: "Specifies the volume of the effect.");
    }

    public AmpControlState High;
    public AmpControlState Body;
    public AmpControlState Low;
    public AmpControlState Level;
}
