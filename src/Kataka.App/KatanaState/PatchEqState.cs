using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

/// <summary>
/// Patch-level EQ state. The Katana stores two EQ blocks per patch (Eq1 embedded in Patch_0
/// and Eq2 at block 0x0060). Each has a parametric section (SW, TYPE, bands) and a graphic EQ.
/// </summary>
public class PatchEqState
{
    public PatchEqState(bool isEq2 = false)
    {
        Sw = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Sw : KatanaMkIIParameterCatalog.PatchEq1Sw);
        Type = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Type : KatanaMkIIParameterCatalog.PatchEq1Type);
        LowCut = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2LowCut : KatanaMkIIParameterCatalog.PatchEq1LowCut);
        LowGain = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2LowGain : KatanaMkIIParameterCatalog.PatchEq1LowGain);
        LowMidFreq = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2LowMidFreq : KatanaMkIIParameterCatalog.PatchEq1LowMidFreq);
        LowMidQ = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2LowMidQ : KatanaMkIIParameterCatalog.PatchEq1LowMidQ);
        LowMidGain = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2LowMidGain : KatanaMkIIParameterCatalog.PatchEq1LowMidGain);
        HiMidFreq = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2HighMidFreq : KatanaMkIIParameterCatalog.PatchEq1HighMidFreq);
        HiMidQ = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2HighMidQ : KatanaMkIIParameterCatalog.PatchEq1HighMidQ);
        HiMidGain = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2HighMidGain : KatanaMkIIParameterCatalog.PatchEq1HighMidGain);
        HighGain = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2HighGain : KatanaMkIIParameterCatalog.PatchEq1HighGain);
        HighCut = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2HighCut : KatanaMkIIParameterCatalog.PatchEq1HighCut);
        Level = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Level : KatanaMkIIParameterCatalog.PatchEq1Level);
        Geq31Hz = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Geq31Hz : KatanaMkIIParameterCatalog.PatchEq1Geq31Hz);
        Geq62Hz = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Geq62Hz : KatanaMkIIParameterCatalog.PatchEq1Geq62Hz);
        Geq125Hz = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Geq125Hz : KatanaMkIIParameterCatalog.PatchEq1Geq125Hz);
        Geq250Hz = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Geq250Hz : KatanaMkIIParameterCatalog.PatchEq1Geq250Hz);
        Geq500Hz = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Geq500Hz : KatanaMkIIParameterCatalog.PatchEq1Geq500Hz);
        Geq1kHz = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Geq1kHz : KatanaMkIIParameterCatalog.PatchEq1Geq1kHz);
        Geq2kHz = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Geq2kHz : KatanaMkIIParameterCatalog.PatchEq1Geq2kHz);
        Geq4kHz = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Geq4kHz : KatanaMkIIParameterCatalog.PatchEq1Geq4kHz);
        Geq8kHz = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Geq8kHz : KatanaMkIIParameterCatalog.PatchEq1Geq8kHz);
        Geq16kHz = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Geq16kHz : KatanaMkIIParameterCatalog.PatchEq1Geq16kHz);
        GeqLevel = new(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2GeqLevel : KatanaMkIIParameterCatalog.PatchEq1GeqLevel);
    }

    public AmpControlState Sw;
    public AmpControlState Type;
    public AmpControlState LowCut;
    public AmpControlState LowGain;
    public AmpControlState LowMidFreq;
    public AmpControlState LowMidQ;
    public AmpControlState LowMidGain;
    public AmpControlState HiMidFreq;
    public AmpControlState HiMidQ;
    public AmpControlState HiMidGain;
    public AmpControlState HighGain;
    public AmpControlState HighCut;
    public AmpControlState Level;
    public AmpControlState Geq31Hz;
    public AmpControlState Geq62Hz;
    public AmpControlState Geq125Hz;
    public AmpControlState Geq250Hz;
    public AmpControlState Geq500Hz;
    public AmpControlState Geq1kHz;
    public AmpControlState Geq2kHz;
    public AmpControlState Geq4kHz;
    public AmpControlState Geq8kHz;
    public AmpControlState Geq16kHz;
    public AmpControlState GeqLevel;
}
