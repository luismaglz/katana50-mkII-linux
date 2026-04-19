using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class RotaryState
{
    public AmpControlState Depth;
    public AmpControlState Level;

    public AmpControlState RateFast;

    public RotaryState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModRotaryParams : KatanaMkIIParameterCatalog.FxRotaryParams;
        RateFast = new AmpControlState(p[0], description: "Adjusts the speed of the rotation.");
        Depth = new AmpControlState(p[1], description: "Adjusts the amount of depth in the rotary effect.");
        Level = new AmpControlState(p[2], description: "Adjusts the volume.");
    }
}
