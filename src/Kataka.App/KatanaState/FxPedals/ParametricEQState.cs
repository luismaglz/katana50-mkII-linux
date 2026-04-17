using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class ParametricEQState
{
    public ParametricEQState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModParametricEqParams : KatanaMkIIParameterCatalog.FxParametricEqParams;
        LowCut = new AmpControlState(p[0], description: "Sets the frequency at which the low cut filter begins to take effect.");
        LowGain = new AmpControlState(p[1], description: "Adjusts the low frequency range tone.");
        LoMidFreq = new AmpControlState(p[2], description: "Specifies the center of the frequency range that will be adjusted by the LO-MID GAIN.");
        LoMidQ = new AmpControlState(p[3], description: "Adjusts the width of the area affected by the EQ centered at the LO-MID FREQ. Higher values will narrow the area.");
        LoMidGain = new AmpControlState(p[4], description: "Adjusts the low-middle frequency range tone.");
        HiMidFreq = new AmpControlState(p[5], description: "Specifies the center of the frequency range that will be adjusted by the HI-MID GAIN.");
        HiMidQ = new AmpControlState(p[6], description: "Adjusts the width of the area affected by the EQ centered at the HI-MID FREQ. Higher values will narrow the area.");
        HiMidGain = new AmpControlState(p[7], description: "Adjusts the high-middle frequency range tone.");
        HighGain = new AmpControlState(p[8], description: "Adjusts the high frequency range tone.");
        HighCut = new AmpControlState(p[9], description: "Sets the frequency at which the high cut filter begins to take effect.");
        Level = new AmpControlState(p[10], description: "Adjusts the overall volume level of the equalizer.");
    }

    public AmpControlState LowCut;
    public AmpControlState LowGain;
    public AmpControlState LoMidFreq;
    public AmpControlState LoMidQ;
    public AmpControlState LoMidGain;
    public AmpControlState HiMidFreq;
    public AmpControlState HiMidQ;
    public AmpControlState HiMidGain;
    public AmpControlState HighGain;
    public AmpControlState HighCut;
    public AmpControlState Level;
}
