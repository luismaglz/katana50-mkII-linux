using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

/// <summary>
///     State for the hardware expression pedal FX (Wah / Pedal Bend / EVH95 / Foot Volume).
///     BTS block: prm_prop_patch_1 at patch offset 0x550.
/// </summary>
public class HardwarePedalState
{
    public AmpControlState EnabledState =
        new(KatanaMkIIParameterCatalog.PedalFxSwitch, description: "Pedal FX on/off.");

    public AmpControlState FootVolume =
        new(KatanaMkIIParameterCatalog.FootVolume, description: "Foot volume pedal position.");

    public AmpControlState Position = new(KatanaMkIIParameterCatalog.PedalFxPosition,
        description: "Pedal FX position in the chain.");

    public AmpControlState Type = new(KatanaMkIIParameterCatalog.PedalFxType,
        description: "Pedal type: 0=WAH, 1=BEND, 2=EVH95.");

    #region Wah

    public AmpControlState WahType = new(KatanaMkIIParameterCatalog.PedalFxWahType, description: "Wah type.");

    public AmpControlState WahPedalPosition = new(KatanaMkIIParameterCatalog.PedalFxWahPedalPosition,
        description: "Current pedal position.");

    public AmpControlState WahMinimum =
        new(KatanaMkIIParameterCatalog.PedalFxWahPedalMinimum, description: "Wah pedal minimum.");

    public AmpControlState WahMaximum =
        new(KatanaMkIIParameterCatalog.PedalFxWahPedalMaximum, description: "Wah pedal maximum.");

    public AmpControlState WahEffectLevel =
        new(KatanaMkIIParameterCatalog.PedalFxWahEffectLevel, description: "Wah effect level.");

    public AmpControlState WahDirectMix =
        new(KatanaMkIIParameterCatalog.PedalFxWahDirectMix, description: "Wah direct mix.");

    #endregion

    #region Pedal Bend

    public AmpControlState BendPitch = new(KatanaMkIIParameterCatalog.PedalFxBendPitch, 0, 48,
        description: "Pitch range in semitones.");

    public AmpControlState BendPedalPosition = new(KatanaMkIIParameterCatalog.PedalFxBendPedalPosition,
        description: "Current pedal position.");

    public AmpControlState BendEffectLevel =
        new(KatanaMkIIParameterCatalog.PedalFxBendEffectLevel, description: "Bend effect level.");

    public AmpControlState BendDirectMix =
        new(KatanaMkIIParameterCatalog.PedalFxBendDirectMix, description: "Bend direct mix.");

    #endregion

    #region EVH95 Wah

    public AmpControlState Evh95Position = new(KatanaMkIIParameterCatalog.PedalFxEvh95Position,
        description: "Current pedal position.");

    public AmpControlState Evh95Minimum =
        new(KatanaMkIIParameterCatalog.PedalFxEvh95Minimum, description: "EVH95 pedal minimum.");

    public AmpControlState Evh95Maximum =
        new(KatanaMkIIParameterCatalog.PedalFxEvh95Maximum, description: "EVH95 pedal maximum.");

    public AmpControlState Evh95EffectLevel =
        new(KatanaMkIIParameterCatalog.PedalFxEvh95EffectLevel, description: "EVH95 effect level.");

    public AmpControlState Evh95DirectMix =
        new(KatanaMkIIParameterCatalog.PedalFxEvh95DirectMix, description: "EVH95 direct mix.");

    #endregion
}
