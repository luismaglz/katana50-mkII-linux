using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public class ReverbPedalState
{
    public AmpControlState EnabledState = new(KatanaMkIIParameterCatalog.ReverbSwitch);
    public AmpControlState Type = new(KatanaMkIIParameterCatalog.ReverbType);
    public AmpControlState Variation = new(KatanaMkIIParameterCatalog.ReverbVariation);
    public AmpControlState Level = new(KatanaMkIIParameterCatalog.ReverbLevel);

    #region Reverb Parameters

    public AmpControlState Time = new(KatanaMkIIParameterCatalog.ReverbTime, description: "Adjusts the length (time) of reverberation.");
    public AmpControlState PreDelay = new(KatanaMkIIParameterCatalog.ReverbPreDelay, description: "Adjusts the time until the reverb sound appears.");
    public AmpControlState LowCut = new(KatanaMkIIParameterCatalog.ReverbLowCut, description: "Sets the frequency at which the low cut filter begins to take effect.");
    public AmpControlState HighCut = new(KatanaMkIIParameterCatalog.ReverbHighCut, description: "Sets the frequency at which the high cut filter begins to take effect.");
    public AmpControlState Density = new(KatanaMkIIParameterCatalog.ReverbDensity, description: "Adjusts the density of the reverb sound.");
    public AmpControlState Color = new(KatanaMkIIParameterCatalog.ReverbColor, description: "Adjusts the unique tone of the spring reverb. Only when TYPE is SPRING.");
    public AmpControlState EffectLevel = new(KatanaMkIIParameterCatalog.ReverbEffectLevel, description: "Adjusts the volume of the reverb sound.");
    public AmpControlState DirectMix = new(KatanaMkIIParameterCatalog.ReverbDirectMix, description: "Adjusts the volume of the direct sound.");

    #endregion
}
