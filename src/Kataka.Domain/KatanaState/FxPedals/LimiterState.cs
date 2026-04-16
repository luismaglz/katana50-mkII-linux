using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class LimiterState
{
    public LimiterState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModLimiterParams : KatanaMkIIParameterCatalog.FxLimiterParams;
        Type = new AmpControlState(p[0], description: "Selects the limiter type.");
        Attack = new AmpControlState(p[1], description: "Adjusts the strength of the picking attack when the strings are played. Higher values result in a sharper attack.");
        Threshold = new AmpControlState(p[2], description: "Adjust this as appropriate for the input signal from your guitar. When the input signal level exceeds this threshold level, limiting will be applied.");
        Ratio = new AmpControlState(p[3], description: "Selects the compression ratio used with signals in excess of the threshold level.");
        Release = new AmpControlState(p[4], description: "Adjusts the release time.");
        Level = new AmpControlState(p[5], description: "Adjusts the volume.");
    }

    public AmpControlState Type;
    public AmpControlState Attack;
    public AmpControlState Threshold;
    public AmpControlState Ratio;
    public AmpControlState Release;
    public AmpControlState Level;
}
