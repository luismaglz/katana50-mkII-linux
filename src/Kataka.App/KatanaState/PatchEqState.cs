using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

/// <summary>
///     Patch-level EQ state. The Katana stores two EQ blocks per patch (Eq1 embedded in Patch_0
///     and Eq2 at block 0x0060). Each has a parametric section (SW, TYPE, bands) and a graphic EQ.
/// </summary>
public class PatchEqState
{
    public AmpControlState Geq125Hz;
    public AmpControlState Geq16kHz;
    public AmpControlState Geq1kHz;
    public AmpControlState Geq250Hz;
    public AmpControlState Geq2kHz;
    public AmpControlState Geq31Hz;
    public AmpControlState Geq4kHz;
    public AmpControlState Geq500Hz;
    public AmpControlState Geq62Hz;
    public AmpControlState Geq8kHz;
    public AmpControlState GeqLevel;
    public AmpControlState HighCut;
    public AmpControlState HighGain;
    public AmpControlState HiMidFreq;
    public AmpControlState HiMidGain;
    public AmpControlState HiMidQ;
    public AmpControlState Level;
    public AmpControlState LowCut;
    public AmpControlState LowGain;
    public AmpControlState LowMidFreq;
    public AmpControlState LowMidGain;
    public AmpControlState LowMidQ;

    public AmpControlState Sw;
    public AmpControlState Type;

    public PatchEqState(bool isEq2 = false)
    {
        Sw = new AmpControlState(isEq2 ? KatanaMkIIParameterCatalog.PatchEq2Sw : KatanaMkIIParameterCatalog.PatchEq1Sw);
        Type = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Type
            : KatanaMkIIParameterCatalog.PatchEq1Type);
        LowCut = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2LowCut
            : KatanaMkIIParameterCatalog.PatchEq1LowCut);
        LowGain = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2LowGain
            : KatanaMkIIParameterCatalog.PatchEq1LowGain);
        LowMidFreq = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2LowMidFreq
            : KatanaMkIIParameterCatalog.PatchEq1LowMidFreq);
        LowMidQ = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2LowMidQ
            : KatanaMkIIParameterCatalog.PatchEq1LowMidQ);
        LowMidGain = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2LowMidGain
            : KatanaMkIIParameterCatalog.PatchEq1LowMidGain);
        HiMidFreq = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2HighMidFreq
            : KatanaMkIIParameterCatalog.PatchEq1HighMidFreq);
        HiMidQ = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2HighMidQ
            : KatanaMkIIParameterCatalog.PatchEq1HighMidQ);
        HiMidGain = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2HighMidGain
            : KatanaMkIIParameterCatalog.PatchEq1HighMidGain);
        HighGain = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2HighGain
            : KatanaMkIIParameterCatalog.PatchEq1HighGain);
        HighCut = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2HighCut
            : KatanaMkIIParameterCatalog.PatchEq1HighCut);
        Level = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Level
            : KatanaMkIIParameterCatalog.PatchEq1Level);
        Geq31Hz = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Geq31Hz
            : KatanaMkIIParameterCatalog.PatchEq1Geq31Hz);
        Geq62Hz = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Geq62Hz
            : KatanaMkIIParameterCatalog.PatchEq1Geq62Hz);
        Geq125Hz = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Geq125Hz
            : KatanaMkIIParameterCatalog.PatchEq1Geq125Hz);
        Geq250Hz = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Geq250Hz
            : KatanaMkIIParameterCatalog.PatchEq1Geq250Hz);
        Geq500Hz = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Geq500Hz
            : KatanaMkIIParameterCatalog.PatchEq1Geq500Hz);
        Geq1kHz = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Geq1kHz
            : KatanaMkIIParameterCatalog.PatchEq1Geq1kHz);
        Geq2kHz = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Geq2kHz
            : KatanaMkIIParameterCatalog.PatchEq1Geq2kHz);
        Geq4kHz = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Geq4kHz
            : KatanaMkIIParameterCatalog.PatchEq1Geq4kHz);
        Geq8kHz = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Geq8kHz
            : KatanaMkIIParameterCatalog.PatchEq1Geq8kHz);
        Geq16kHz = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2Geq16kHz
            : KatanaMkIIParameterCatalog.PatchEq1Geq16kHz);
        GeqLevel = new AmpControlState(isEq2
            ? KatanaMkIIParameterCatalog.PatchEq2GeqLevel
            : KatanaMkIIParameterCatalog.PatchEq1GeqLevel);
    }
}
