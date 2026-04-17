using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class TremoloState
{
    public TremoloState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModTremoloParams : KatanaMkIIParameterCatalog.FxTremoloParams;
        WaveShape = new AmpControlState(p[0], description: "Adjusts changes in volume level. A higher value will steepen waves shape.");
        Rate = new AmpControlState(p[1], description: "Adjusts the frequency (speed) of the change.");
        Depth = new AmpControlState(p[2], description: "Adjusts the depth of the effect.");
        Level = new AmpControlState(p[3], description: "Adjusts the volume.");
    }

    public AmpControlState WaveShape;
    public AmpControlState Rate;
    public AmpControlState Depth;
    public AmpControlState Level;
}
