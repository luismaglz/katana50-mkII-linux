using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public class Delay2PedalState
{
    public AmpControlState EnabledState = new(KatanaMkIIParameterCatalog.Delay2Switch);
    public AmpControlState Type = new(KatanaMkIIParameterCatalog.Delay2Type);

    #region Delay 2 Parameters

    public AmpControlState Feedback = new(KatanaMkIIParameterCatalog.Delay2Feedback, description: "Adjusts the volume that is returned to the input. A higher value will increase the number of the delay repeats.");
    public AmpControlState HighCut = new(KatanaMkIIParameterCatalog.Delay2HighCut, description: "Sets the frequency at which the high cut filter begins to take effect.");
    public AmpControlState EffectLevel = new(KatanaMkIIParameterCatalog.Delay2EffectLevel, description: "Adjusts the volume of the delay sound.");
    public AmpControlState DirectMix = new(KatanaMkIIParameterCatalog.Delay2DirectMix, description: "Adjusts the volume of the direct sound.");
    public AmpControlState TapTime = new(KatanaMkIIParameterCatalog.Delay2TapTime, description: "Setting adjusts the R channel delay time relative to the L channel delay time (considered as 100%). Only when TYPE is PAN.");
    public AmpControlState ModRate = new(KatanaMkIIParameterCatalog.Delay2ModRate, description: "Adjusts the modulation rate of the delay sound. Only when TYPE is MODULATE or SDE-3000.");
    public AmpControlState ModDepth = new(KatanaMkIIParameterCatalog.Delay2ModDepth, description: "Adjusts the modulation depth of the delay sound. Only when TYPE is MODULATE or SDE-3000.");
    public AmpControlState Range = new(KatanaMkIIParameterCatalog.Delay2Range, description: "Models the way in which the SDE-3000's frequency response is affected by the delay range. Only when TYPE is SDE-3000.");
    public AmpControlState Filter = new(KatanaMkIIParameterCatalog.Delay2Filter, description: "Turns the filter on/off. When on, a natural-sounding effect is obtained when using the delay as an echo. Only when TYPE is SDE-3000.");
    public AmpControlState FeedbackPhase = new(KatanaMkIIParameterCatalog.Delay2FeedbackPhase, description: "Specifies the phase of the delay sound feedback. Selecting INV inverts the phase. Only when TYPE is SDE-3000.");
    public AmpControlState DelayPhase = new(KatanaMkIIParameterCatalog.Delay2DelayPhase, description: "Specifies the phase of the delay sound. Selecting INV inverts the phase. Only when TYPE is SDE-3000.");
    public AmpControlState ModSw = new(KatanaMkIIParameterCatalog.Delay2ModSw, description: "Turns the modulation on/off. Only when TYPE is SDE-3000.");

    #endregion
}
