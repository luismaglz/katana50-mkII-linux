using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class TWahState
{
    public AmpControlState DirectMix;
    public AmpControlState EffectLevel;
    public AmpControlState Freq;

    public AmpControlState Mode;
    public AmpControlState Peak;
    public AmpControlState Polarity;
    public AmpControlState Sens;

    public TWahState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModTWahParams : KatanaMkIIParameterCatalog.FxTWahParams;
        Mode = new AmpControlState(p[0],
            description:
            "Selects the wah mode. LPF provides a wah effect over a wide frequency range; BPF provides a wah effect in a narrow frequency range.");
        Polarity = new AmpControlState(p[1],
            description:
            "Selects the direction in which the filter will change. DOWN cuts the frequency; UP raises the frequency.");
        Sens = new AmpControlState(p[2],
            description:
            "Specifies the sensitivity with which the filter changes. Higher values produce a stronger tone which emphasizes the wah effect.");
        Freq = new AmpControlState(p[3], description: "Adjusts the center frequency of the Wah effect.");
        Peak = new AmpControlState(p[4],
            description: "Adjusts the way in which the wah effect applies to the area around the center frequency.");
        DirectMix = new AmpControlState(p[5], description: "Adjusts the volume of the direct sound.");
        EffectLevel = new AmpControlState(p[6], description: "Adjusts the volume of the effect sound.");
    }
}
