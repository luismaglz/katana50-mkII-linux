using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

/// <summary>
///     Channel-mode stored preamp parameters (PRM_PREAMP_A_*).
///     These govern the amp sound when the amp is in channel/patch mode.
///     In panel mode, the KNOB_POS values in KatanaState (Gain/Bass/etc.) are used instead.
/// </summary>
public class PreampState
{
    public AmpControlState Bass = new(KatanaMkIIParameterCatalog.PreampBass, description: "Bass EQ in channel mode.");

    public AmpControlState Bright = new(KatanaMkIIParameterCatalog.PreampBright, description: "Bright switch.");

    public AmpControlState Gain = new(KatanaMkIIParameterCatalog.PreampGain, 0, 120,
        description: "Gain in channel mode.");

    /// <summary>Preamp output level (not the master Volume knob).</summary>
    public AmpControlState Level = new(KatanaMkIIParameterCatalog.PreampLevel,
        description: "Preamp output level in channel mode.");

    public AmpControlState Middle = new(KatanaMkIIParameterCatalog.PreampMiddle,
        description: "Middle EQ in channel mode.");

    public AmpControlState Presence = new(KatanaMkIIParameterCatalog.PreampPresence,
        description: "Presence in channel mode.");

    public AmpControlState SoloLevel = new(KatanaMkIIParameterCatalog.PreampSoloLevel,
        description: "Volume level when Solo is on.");

    public AmpControlState SoloSw = new(KatanaMkIIParameterCatalog.PreampSoloSw, description: "Solo switch.");

    public AmpControlState Treble = new(KatanaMkIIParameterCatalog.PreampTreble,
        description: "Treble EQ in channel mode.");

    /// <summary>Amp character type (0-32). Finer-grained than the 5-position panel type knob.</summary>
    public AmpControlState Type = new(KatanaMkIIParameterCatalog.PreampType, 0, 32,
        description: "Amp character type in channel mode.");
}
