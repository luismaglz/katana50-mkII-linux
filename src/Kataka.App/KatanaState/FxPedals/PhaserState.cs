using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class PhaserState
{
    public PhaserState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModPhaserParams : KatanaMkIIParameterCatalog.FxPhaserParams;
        Type = new AmpControlState(p[0], description: "Selects the number of stages that the phaser effect will use.");
        Rate = new AmpControlState(p[1], description: "This sets the rate of the phaser effect.");
        Depth = new AmpControlState(p[2], description: "Determines the depth of the phaser effect.");
        Manual = new AmpControlState(p[3], description: "Adjusts the center frequency of the phaser effect.");
        Resonance = new AmpControlState(p[4], description: "Determines the amount of resonance (feedback). Increasing the value will emphasize the effect.");
        StepRate = new AmpControlState(p[5], description: "Sets the cycle of the step function that changes the rate and depth. Set to Off when not using the step function.");
        EffectLevel = new AmpControlState(p[6], description: "Adjusts the volume of the phaser.");
        DirectMix = new AmpControlState(p[7], description: "Adjusts the volume of the direct sound.");
    }

    public AmpControlState Type;
    public AmpControlState Rate;
    public AmpControlState Depth;
    public AmpControlState Manual;
    public AmpControlState Resonance;
    public AmpControlState StepRate;
    public AmpControlState EffectLevel;
    public AmpControlState DirectMix;
}
