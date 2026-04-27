using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public class GlobalEqState
{
    public EqBankState Bank1 = new();
    public EqBankState Bank2 = new();
    public EqBankState Bank3 = new();

    /// <summary>Selector: which bank is active (0=bank1, 1=bank2, 2=bank3).</summary>
    public AmpControlState Select = new(KatanaMkIIParameterCatalog.GlobalEqSelect,
        description: "Selects which Global EQ bank is active (0=bank1,1=bank2,2=bank3)");

    /// <summary>Master on/off switch (PRM_SYS_EQ_SW) — shared across all banks.</summary>
    public AmpControlState Sw = new(KatanaMkIIParameterCatalog.GlobalEqSw);

    public GlobalEqState()
    {
        // Bank 1
        Bank1.Type = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Type);
        Bank1.Position = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Position);
        Bank1.LowCut = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1LowCut);
        Bank1.LowGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1LowGain);
        Bank1.LowMidFreq = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1LowMidFreq);
        Bank1.LowMidQ = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1LowMidQ);
        Bank1.LowMidGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1LowMidGain);
        Bank1.HiMidFreq = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1HiMidFreq);
        Bank1.HiMidQ = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1HiMidQ);
        Bank1.HiMidGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1HiMidGain);
        Bank1.HighGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1HighGain);
        Bank1.HighCut = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1HighCut);
        Bank1.Level = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Level);
        Bank1.Geq31Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Geq31Hz);
        Bank1.Geq62Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Geq62Hz);
        Bank1.Geq125Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Geq125Hz);
        Bank1.Geq250Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Geq250Hz);
        Bank1.Geq500Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Geq500Hz);
        Bank1.Geq1kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Geq1kHz);
        Bank1.Geq2kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Geq2kHz);
        Bank1.Geq4kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Geq4kHz);
        Bank1.Geq8kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Geq8kHz);
        Bank1.Geq16kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1Geq16kHz);
        Bank1.GeqLevel = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq1GeqLevel);

        // Bank 2
        Bank2.Type = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Type);
        Bank2.Position = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Position);
        Bank2.LowCut = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2LowCut);
        Bank2.LowGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2LowGain);
        Bank2.LowMidFreq = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2LowMidFreq);
        Bank2.LowMidQ = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2LowMidQ);
        Bank2.LowMidGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2LowMidGain);
        Bank2.HiMidFreq = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2HiMidFreq);
        Bank2.HiMidQ = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2HiMidQ);
        Bank2.HiMidGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2HiMidGain);
        Bank2.HighGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2HighGain);
        Bank2.HighCut = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2HighCut);
        Bank2.Level = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Level);
        Bank2.Geq31Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Geq31Hz);
        Bank2.Geq62Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Geq62Hz);
        Bank2.Geq125Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Geq125Hz);
        Bank2.Geq250Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Geq250Hz);
        Bank2.Geq500Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Geq500Hz);
        Bank2.Geq1kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Geq1kHz);
        Bank2.Geq2kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Geq2kHz);
        Bank2.Geq4kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Geq4kHz);
        Bank2.Geq8kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Geq8kHz);
        Bank2.Geq16kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2Geq16kHz);
        Bank2.GeqLevel = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq2GeqLevel);

        // Bank 3
        Bank3.Type = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Type);
        Bank3.Position = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Position);
        Bank3.LowCut = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3LowCut);
        Bank3.LowGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3LowGain);
        Bank3.LowMidFreq = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3LowMidFreq);
        Bank3.LowMidQ = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3LowMidQ);
        Bank3.LowMidGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3LowMidGain);
        Bank3.HiMidFreq = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3HiMidFreq);
        Bank3.HiMidQ = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3HiMidQ);
        Bank3.HiMidGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3HiMidGain);
        Bank3.HighGain = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3HighGain);
        Bank3.HighCut = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3HighCut);
        Bank3.Level = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Level);
        Bank3.Geq31Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Geq31Hz);
        Bank3.Geq62Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Geq62Hz);
        Bank3.Geq125Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Geq125Hz);
        Bank3.Geq250Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Geq250Hz);
        Bank3.Geq500Hz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Geq500Hz);
        Bank3.Geq1kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Geq1kHz);
        Bank3.Geq2kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Geq2kHz);
        Bank3.Geq4kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Geq4kHz);
        Bank3.Geq8kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Geq8kHz);
        Bank3.Geq16kHz = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3Geq16kHz);
        Bank3.GeqLevel = new AmpControlState(KatanaMkIIParameterCatalog.GlobalEq3GeqLevel);
    }

    public class EqBankState
    {
        public AmpControlState? Geq125Hz;
        public AmpControlState? Geq16kHz;
        public AmpControlState? Geq1kHz;
        public AmpControlState? Geq250Hz;
        public AmpControlState? Geq2kHz;
        public AmpControlState? Geq31Hz;
        public AmpControlState? Geq4kHz;
        public AmpControlState? Geq500Hz;
        public AmpControlState? Geq62Hz;
        public AmpControlState? Geq8kHz;
        public AmpControlState? GeqLevel;
        public AmpControlState? HighCut;
        public AmpControlState? HighGain;
        public AmpControlState? HiMidFreq;
        public AmpControlState? HiMidGain;
        public AmpControlState? HiMidQ;
        public AmpControlState? Level;
        public AmpControlState? LowCut;
        public AmpControlState? LowGain;
        public AmpControlState? LowMidFreq;
        public AmpControlState? LowMidGain;
        public AmpControlState? LowMidQ;
        public AmpControlState? Position;
        public AmpControlState? Type;
    }
}
