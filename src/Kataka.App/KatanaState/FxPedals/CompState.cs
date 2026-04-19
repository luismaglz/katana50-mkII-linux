using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class CompState
{
    public AmpControlState Attack;
    public AmpControlState Level;
    public AmpControlState Sustain;
    public AmpControlState Tone;

    public AmpControlState Type;

    public CompState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModCompParams : KatanaMkIIParameterCatalog.FxCompParams;
        Type = new AmpControlState(p[0], description: "Selects the compressor type.");
        Sustain = new AmpControlState(p[1],
            description:
            "Adjusts the range (time) over which low-level signals are boosted. Larger values result in longer sustain.");
        Attack = new AmpControlState(p[2],
            description:
            "Adjusts the strength of the picking attack when the strings are played. Higher values result in a sharper attack.");
        Tone = new AmpControlState(p[3], description: "Adjusts the tone.");
        Level = new AmpControlState(p[4], description: "Adjusts the volume.");
    }
}
