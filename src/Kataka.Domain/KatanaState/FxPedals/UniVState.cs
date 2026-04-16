using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class UniVState
{
    public UniVState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModUniVParams : KatanaMkIIParameterCatalog.FxUniVParams;
        Rate = new AmpControlState(p[0], description: "Adjusts the rate of the UNI-V effect.");
        Depth = new AmpControlState(p[1], description: "Adjusts the depth of the UNI-V effect.");
        Level = new AmpControlState(p[2], description: "Adjusts the volume.");
    }

    public AmpControlState Rate;
    public AmpControlState Depth;
    public AmpControlState Level;
}
