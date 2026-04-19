using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class HumanizerState
{
    public AmpControlState Depth;
    public AmpControlState Level;
    public AmpControlState Manual;

    public AmpControlState Mode;
    public AmpControlState Rate;
    public AmpControlState Sens;
    public AmpControlState Vowel1;
    public AmpControlState Vowel2;

    public HumanizerState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModHumanizerParams : KatanaMkIIParameterCatalog.FxHumanizerParams;
        Mode = new AmpControlState(p[0],
            description:
            "Sets the mode that switches the vowels. PICKING switches based on picking; AUTO switches automatically by rate and depth.");
        Vowel1 = new AmpControlState(p[1], description: "Selects the first vowel (a, e, i, o, u).");
        Vowel2 = new AmpControlState(p[2], description: "Selects the second vowel (a, e, i, o, u).");
        Sens = new AmpControlState(p[3], description: "Adjusts the sensitivity of the humanizer.");
        Rate = new AmpControlState(p[4], description: "Adjusts the cycle for changing the two vowels.");
        Depth = new AmpControlState(p[5], description: "Adjusts the depth of the effect.");
        Manual = new AmpControlState(p[6], description: "Adjusts the cycle for changing the two vowels manually.");
        Level = new AmpControlState(p[7], description: "Adjusts the volume.");
    }
}
