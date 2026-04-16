using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class ChorusState
{
    public ChorusState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModChorusParams : KatanaMkIIParameterCatalog.FxChorusParams;
        XoverFreq = new AmpControlState(p[0], description: "Sets the crossover frequency dividing the low- and high-frequency ranges.");
        LowRate = new AmpControlState(p[1], description: "Adjusts the speed of the chorus effect for the low-frequency range.");
        LowDepth = new AmpControlState(p[2], description: "Adjusts the depth of the chorus effect for the low-frequency range.");
        LowPreDelay = new AmpControlState(p[3], description: "Adjusts the delay of the effect sound in the low-frequency range.");
        LowLevel = new AmpControlState(p[4], description: "Adjusts the volume of the effect sound in the low-frequency range.");
        HighRate = new AmpControlState(p[5], description: "Adjusts the speed of the chorus effect for the high-frequency range.");
        HighDepth = new AmpControlState(p[6], description: "Adjusts the depth of the chorus effect for the high-frequency range.");
        HighPreDelay = new AmpControlState(p[7], description: "Adjusts the delay of the effect sound in the high-frequency range.");
        HighLevel = new AmpControlState(p[8], description: "Adjusts the volume of the effect sound in the high-frequency range.");
        DirectMix = new AmpControlState(p[9], description: "Adjusts the volume of the direct sound.");
    }

    public AmpControlState XoverFreq;
    public AmpControlState LowRate;
    public AmpControlState LowDepth;
    public AmpControlState LowPreDelay;
    public AmpControlState LowLevel;
    public AmpControlState HighRate;
    public AmpControlState HighDepth;
    public AmpControlState HighPreDelay;
    public AmpControlState HighLevel;
    public AmpControlState DirectMix;
}
