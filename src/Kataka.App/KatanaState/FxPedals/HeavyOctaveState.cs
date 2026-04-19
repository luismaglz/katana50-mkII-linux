using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class HeavyOctaveState
{
    public AmpControlState DirectMix;

    public AmpControlState Oct1Level;
    public AmpControlState Oct2Level;

    public HeavyOctaveState(bool isMod)
    {
        var p = isMod
            ? KatanaMkIIParameterCatalog.ModHeavyOctaveParams
            : KatanaMkIIParameterCatalog.FxHeavyOctaveParams;
        Oct1Level = new AmpControlState(p[0], description: "Adjusts the volume of the sound one octave below.");
        Oct2Level = new AmpControlState(p[1], description: "Adjusts the volume of the sound two octaves below.");
        DirectMix = new AmpControlState(p[2], description: "Adjusts the volume of the direct sound.");
    }
}
