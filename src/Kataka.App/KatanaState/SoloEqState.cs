using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

/// <summary>
/// Solo EQ and Solo Delay (Ver200+ / Ver210+ Mk2V2 block, patch offset 0xF10).
/// These apply during Solo mode activated by the booster or preamp solo switch.
/// </summary>
public class SoloEqState
{
    #region Solo EQ (Ver200+)

    /// <summary>Solo EQ position in the signal chain (0=PRE, 1=POST).</summary>
    public AmpControlState EqPosition = new(KatanaMkIIParameterCatalog.SoloEqPosition, description: "Solo EQ position: 0=PRE, 1=POST.");
    public AmpControlState EqSw = new(KatanaMkIIParameterCatalog.SoloEqSw, description: "Solo EQ on/off.");
    public AmpControlState EqLowCut = new(KatanaMkIIParameterCatalog.SoloEqLowCut, description: "Low cut frequency.");
    /// <summary>Raw 0-48, display -24 to +24 dB.</summary>
    public AmpControlState EqLowGain = new(KatanaMkIIParameterCatalog.SoloEqLowGain, -24, 24, "dB", description: "Low shelf gain.");
    public AmpControlState EqMidFreq = new(KatanaMkIIParameterCatalog.SoloEqMidFreq, description: "Mid EQ frequency.");
    public AmpControlState EqMidQ = new(KatanaMkIIParameterCatalog.SoloEqMidQ, description: "Mid EQ Q (bandwidth).");
    /// <summary>Raw 0-48, display -24 to +24 dB.</summary>
    public AmpControlState EqMidGain = new(KatanaMkIIParameterCatalog.SoloEqMidGain, -24, 24, "dB", description: "Mid peaking gain.");
    /// <summary>Raw 0-48, display -24 to +24 dB.</summary>
    public AmpControlState EqHighGain = new(KatanaMkIIParameterCatalog.SoloEqHighGain, -24, 24, "dB", description: "High shelf gain.");
    public AmpControlState EqHighCut = new(KatanaMkIIParameterCatalog.SoloEqHighCut, description: "High cut frequency.");
    /// <summary>Raw 0-48, display -24 to +24 dB.</summary>
    public AmpControlState EqLevel = new(KatanaMkIIParameterCatalog.SoloEqLevel, -24, 24, "dB", description: "Solo EQ output level.");

    #endregion

    #region Solo Delay (Ver210+)

    public AmpControlState DelaySw = new(KatanaMkIIParameterCatalog.SoloDelaySw, description: "Solo delay on/off.");
    public AmpControlState DelayCarryover = new(KatanaMkIIParameterCatalog.SoloDelayCarryover, description: "Carry over repeats when solo is turned off.");
    public AmpControlState DelayFeedback = new(KatanaMkIIParameterCatalog.SoloDelayFeedback, description: "Delay repeat count.");
    public AmpControlState DelayEffectLevel = new(KatanaMkIIParameterCatalog.SoloDelayEffectLevel, 0, 120, description: "Solo delay effect volume.");
    public AmpControlState DelayDirectLevel = new(KatanaMkIIParameterCatalog.SoloDelayDirectLevel, description: "Solo delay direct level.");
    public AmpControlState DelayFilter = new(KatanaMkIIParameterCatalog.SoloDelayFilter, description: "Delay filter mode.");
    public AmpControlState DelayHighCut = new(KatanaMkIIParameterCatalog.SoloDelayHighCut, description: "Delay high cut.");
    public AmpControlState DelayModSw = new(KatanaMkIIParameterCatalog.SoloDelayModSw, description: "Delay modulation on/off.");
    public AmpControlState DelayModRate = new(KatanaMkIIParameterCatalog.SoloDelayModRate, description: "Delay modulation rate.");
    public AmpControlState DelayModDepth = new(KatanaMkIIParameterCatalog.SoloDelayModDepth, description: "Delay modulation depth.");

    #endregion
}
