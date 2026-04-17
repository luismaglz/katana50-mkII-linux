using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public class BoostPedalState
{
    /// <summary>
    ///     On/Off state for the booster pedal
    /// </summary>
    public AmpControlState EnabledState = new(KatanaMkIIParameterCatalog.BoosterSwitch);

    /// <summary>
    ///     Type of boost pedal selected
    /// </summary>
    public AmpControlState Type = new(KatanaMkIIParameterCatalog.BoosterType);

    /// <summary>
    ///     Maps to the LED that maps to the variation, used in the app iself not necessarily useful to us
    /// </summary>
    public AmpControlState Variation = new(KatanaMkIIParameterCatalog.BoosterVariation);

    /// <summary>Front-panel booster level knob position (PRM_KNOB_POS_BOOST).</summary>
    public AmpControlState Level = new(KatanaMkIIParameterCatalog.BoostLevel);


    #region Pedal Parameters

    public AmpControlState Drive = new(KatanaMkIIParameterCatalog.BoosterDrive, 0, 120, description: "Adjusts the depth of distortion.");
    public AmpControlState Tone = new(KatanaMkIIParameterCatalog.BoosterTone, -50, 50, description: "Adjusts the tone.");

    public AmpControlState Bottom = new(KatanaMkIIParameterCatalog.BoosterBottom, -50, 50,
        description:
        "Adjusts the tone for the low frequency range.\nTurning this to the left (counterclockwise)\nproduces a sound with the low end cut; turning\nit to the right boosts the low end in the sound.");

    public AmpControlState SoloSw = new(KatanaMkIIParameterCatalog.BoosterSoloSw, description: "Switches to a tone that is suitable for solos.");
    public AmpControlState SoloLevel = new(KatanaMkIIParameterCatalog.BoosterSoloLevel, description: "Adjusts the volume level when the Solo Sw is ON.");
    public AmpControlState EffectLevel = new(KatanaMkIIParameterCatalog.BoosterEffectLevel, description: "Adjusts the volume of the effect sound.");
    public AmpControlState BoosterDirectMix = new(KatanaMkIIParameterCatalog.BoosterDirectMix, description: "Adjusts the volume of the direct sound.");

    #endregion
}
