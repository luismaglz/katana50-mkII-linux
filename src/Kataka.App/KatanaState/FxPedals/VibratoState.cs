using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class VibratoState
{
    public VibratoState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModVibratoParams : KatanaMkIIParameterCatalog.FxVibratoParams;
        Rate = new AmpControlState(p[0], description: "Adjusts the rate of the vibrato.");
        Depth = new AmpControlState(p[1], description: "Adjusts the depth of the vibrato.");
        Level = new AmpControlState(p[2], description: "Adjusts the volume.");
    }

    public AmpControlState Rate;
    public AmpControlState Depth;
    public AmpControlState Level;
}
