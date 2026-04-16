using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class SlowGearState
{
    public SlowGearState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModSlowGearParams : KatanaMkIIParameterCatalog.FxSlowGearParams;
        Sens = new AmpControlState(p[0], description: "Adjusts the sensitivity of the slow gear. Lower values require stronger picking; higher values allow softer picking to trigger the effect.");
        RiseTime = new AmpControlState(p[1], description: "Sets the time needed for the volume to reach its maximum from the moment you begin picking.");
        Level = new AmpControlState(p[2], description: "Adjusts the volume of the effect sound.");
    }

    public AmpControlState Sens;
    public AmpControlState RiseTime;
    public AmpControlState Level;
}
