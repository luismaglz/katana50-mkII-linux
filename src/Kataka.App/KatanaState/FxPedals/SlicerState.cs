using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class SlicerState
{
    public SlicerState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModSlicerParams : KatanaMkIIParameterCatalog.FxSlicerParams;
        Pattern = new AmpControlState(p[0], description: "Select the slice pattern that will be used to cut the sound.");
        Rate = new AmpControlState(p[1], description: "Adjust the rate at which the sound will be cut.");
        TriggerSens = new AmpControlState(p[2], description: "Adjust the sensitivity of triggering.");
        EffectLevel = new AmpControlState(p[3], description: "Adjusts the volume of the effect sound.");
        DirectMix = new AmpControlState(p[4], description: "Adjusts the volume of the direct sound.");
    }

    public AmpControlState Pattern;
    public AmpControlState Rate;
    public AmpControlState TriggerSens;
    public AmpControlState EffectLevel;
    public AmpControlState DirectMix;
}
