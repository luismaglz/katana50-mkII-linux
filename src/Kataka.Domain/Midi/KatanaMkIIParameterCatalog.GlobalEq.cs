namespace Kataka.Domain.Midi;

/// <summary>Auto-generated: static partial class KatanaMkIIParameterCatalog</summary>
public static partial class KatanaMkIIParameterCatalog
{
    /// <summary> Global EQ (System block) ───────────────────────────────────────────────── </summary>
    // Three independently-stored EQ banks. GlobalEqSelect picks which bank is active.
    // Addresses: System(0x00) + SysEqN block offset + parameter offset.

    /// <summary>Parameter definition for Global Eq Select.</summary>
    public static KatanaParameterDefinition GlobalEqSelect { get; } =
        new("global-eq-select", "Global EQ Select",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEqSelect, 0x00),
            maximum: 2,
            description: "Selects which Global EQ bank is active (0=bank 1, 1=bank 2, 2=bank 3).");

    /// <summary> Global EQ Bank 1 (SysEq1, block 0x0030) ───────────────────────────────── </summary>
    public static KatanaParameterDefinition GlobalEq1Sw { get; } =
        new("global-eq1-sw", "Global EQ On/Off",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.SystemMainParams.EqSw),
            maximum: 1,
            description: "Turns Global EQ bank 1 on/off.");

    /// <summary>Parameter definition for Global Eq 1 Type.</summary>
    public static KatanaParameterDefinition GlobalEq1Type { get; } =
        new("global-eq1-type", "Global EQ Type",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Type),
            maximum: 1,
            description: "Selects the EQ type: PARAMETRIC EQ or GE-10 (graphic equalizer).");

    /// <summary>Parameter definition for Global Eq 1 Position.</summary>
    public static KatanaParameterDefinition GlobalEq1Position { get; } =
        new("global-eq1-position", "Global EQ Position",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Position),
            maximum: 3,
            description: "Positions the Global EQ in the signal chain (Ver200+).");

    /// <summary>Parameter definition for Global Eq 1 Low Cut.</summary>
    public static KatanaParameterDefinition GlobalEq1LowCut { get; } =
        new("global-eq1-low-cut", "Global EQ Low Cut",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.LowCut),
            maximum: 17,
            description: "Sets the frequency at which the low cut filter begins to take effect (FLAT or 20-800 Hz).");

    /// <summary>Parameter definition for Global Eq 1 Low Gain.</summary>
    public static KatanaParameterDefinition GlobalEq1LowGain { get; } =
        new("global-eq1-low-gain", "Global EQ Low",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.LowGain),
            0, 40,
            description: "Adjusts the low frequency range tone (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 1 Low Mid Freq.</summary>
    public static KatanaParameterDefinition GlobalEq1LowMidFreq { get; } =
        new("global-eq1-lomid-freq", "Global EQ Low-Mid Freq",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.LowMidFreq),
            maximum: 27,
            description: "Specifies the center frequency adjusted by the Low-Mid Gain (20 Hz-10.0 kHz).");

    /// <summary>Parameter definition for Global Eq 1 Low Mid Q.</summary>
    public static KatanaParameterDefinition GlobalEq1LowMidQ { get; } =
        new("global-eq1-lomid-q", "Global EQ Low-Mid Q",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.LowMidQ),
            maximum: 5,
            description: "Adjusts the bandwidth of the low-mid band.");

    /// <summary>Parameter definition for Global Eq 1 Low Mid Gain.</summary>
    public static KatanaParameterDefinition GlobalEq1LowMidGain { get; } =
        new("global-eq1-lomid-gain", "Global EQ Low-Mid",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.LowMidGain),
            0, 40,
            description: "Adjusts the low-middle frequency range tone (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 1 Hi Mid Freq.</summary>
    public static KatanaParameterDefinition GlobalEq1HiMidFreq { get; } =
        new("global-eq1-himid-freq", "Global EQ High-Mid Freq",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.HiMidFreq),
            maximum: 27,
            description: "Specifies the center frequency adjusted by the High-Mid Gain (20 Hz-10.0 kHz).");

    /// <summary>Parameter definition for Global Eq 1 Hi Mid Q.</summary>
    public static KatanaParameterDefinition GlobalEq1HiMidQ { get; } =
        new("global-eq1-himid-q", "Global EQ High-Mid Q",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.HiMidQ),
            maximum: 5,
            description: "Adjusts the bandwidth of the high-mid band.");

    /// <summary>Parameter definition for Global Eq 1 Hi Mid Gain.</summary>
    public static KatanaParameterDefinition GlobalEq1HiMidGain { get; } =
        new("global-eq1-himid-gain", "Global EQ High-Mid",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.HiMidGain),
            0, 40,
            description: "Adjusts the high-middle frequency range tone (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 1 High Gain.</summary>
    public static KatanaParameterDefinition GlobalEq1HighGain { get; } =
        new("global-eq1-high-gain", "Global EQ High",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.HighGain),
            0, 40,
            description: "Adjusts the high frequency range tone (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 1 High Cut.</summary>
    public static KatanaParameterDefinition GlobalEq1HighCut { get; } =
        new("global-eq1-high-cut", "Global EQ High Cut",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.HighCut),
            maximum: 14,
            description: "Sets the frequency at which the high cut filter begins to take effect (630 Hz-FLAT).");

    /// <summary>Parameter definition for Global Eq 1 Level.</summary>
    public static KatanaParameterDefinition GlobalEq1Level { get; } =
        new("global-eq1-level", "Global EQ Level",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Level),
            0, 40,
            description: "Adjusts the overall volume level of the Global EQ (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq 3 1 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq1Geq31Hz { get; } =
        new("global-eq1-geq-31hz", "Global EQ 31 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Geq31Hz),
            maximum: 48, description: "GE-10 graphic EQ band at 31 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq6 2 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq1Geq62Hz { get; } =
        new("global-eq1-geq-62hz", "Global EQ 62 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Geq62Hz),
            maximum: 48, description: "GE-10 graphic EQ band at 62 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq 1 25 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq1Geq125Hz { get; } =
        new("global-eq1-geq-125hz", "Global EQ 125 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Geq125Hz),
            maximum: 48, description: "GE-10 graphic EQ band at 125 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq 250 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq1Geq250Hz { get; } =
        new("global-eq1-geq-250hz", "Global EQ 250 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Geq250Hz),
            maximum: 48, description: "GE-10 graphic EQ band at 250 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq500 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq1Geq500Hz { get; } =
        new("global-eq1-geq-500hz", "Global EQ 500 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Geq500Hz),
            maximum: 48, description: "GE-10 graphic EQ band at 500 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq 1k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq1Geq1kHz { get; } =
        new("global-eq1-geq-1khz", "Global EQ 1 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Geq1kHz),
            maximum: 48, description: "GE-10 graphic EQ band at 1 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq 2k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq1Geq2kHz { get; } =
        new("global-eq1-geq-2khz", "Global EQ 2 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Geq2kHz),
            maximum: 48, description: "GE-10 graphic EQ band at 2 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq4k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq1Geq4kHz { get; } =
        new("global-eq1-geq-4khz", "Global EQ 4 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Geq4kHz),
            maximum: 48, description: "GE-10 graphic EQ band at 4 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq8k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq1Geq8kHz { get; } =
        new("global-eq1-geq-8khz", "Global EQ 8 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Geq8kHz),
            maximum: 48, description: "GE-10 graphic EQ band at 8 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq 16k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq1Geq16kHz { get; } =
        new("global-eq1-geq-16khz", "Global EQ 16 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.Geq16kHz),
            maximum: 48, description: "GE-10 graphic EQ band at 16 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 1 Geq Level.</summary>
    public static KatanaParameterDefinition GlobalEq1GeqLevel { get; } =
        new("global-eq1-geq-level", "Global EQ GEQ Level",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq1, KatanaAddressMap.GlobalEqParams.GeqLevel),
            maximum: 48, description: "Overall output level of the GE-10 graphic EQ (-24 to +24 dB).");

    /// <summary> Global EQ Bank 2 (SysEq2, block 0x0050) ───────────────────────────────── </summary>
    public static KatanaParameterDefinition GlobalEq2Sw { get; } =
        new("global-eq2-sw", "Global EQ 2 On/Off",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.SystemMainParams.EqSw),
            maximum: 1, description: "Turns Global EQ bank 2 on/off.");

    /// <summary>Parameter definition for Global Eq 2 Type.</summary>
    public static KatanaParameterDefinition GlobalEq2Type { get; } =
        new("global-eq2-type", "Global EQ 2 Type",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Type),
            maximum: 1, description: "Selects the EQ type for bank 2: PARAMETRIC EQ or GE-10.");

    /// <summary>Parameter definition for Global Eq 2 Position.</summary>
    public static KatanaParameterDefinition GlobalEq2Position { get; } =
        new("global-eq2-position", "Global EQ 2 Position",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Position),
            maximum: 3, description: "Positions Global EQ bank 2 in the signal chain (Ver200+).");

    /// <summary>Parameter definition for Global Eq 2 Low Cut.</summary>
    public static KatanaParameterDefinition GlobalEq2LowCut { get; } =
        new("global-eq2-low-cut", "Global EQ 2 Low Cut",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.LowCut),
            maximum: 17, description: "Sets the low cut frequency for Global EQ bank 2.");

    /// <summary>Parameter definition for Global Eq 2 Low Gain.</summary>
    public static KatanaParameterDefinition GlobalEq2LowGain { get; } =
        new("global-eq2-low-gain", "Global EQ 2 Low",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.LowGain),
            0, 40, description: "Adjusts the low frequency tone for bank 2 (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 2 Low Mid Freq.</summary>
    public static KatanaParameterDefinition GlobalEq2LowMidFreq { get; } =
        new("global-eq2-lomid-freq", "Global EQ 2 Low-Mid Freq",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.LowMidFreq),
            maximum: 27, description: "Low-mid center frequency for Global EQ bank 2 (20 Hz-10.0 kHz).");

    /// <summary>Parameter definition for Global Eq 2 Low Mid Q.</summary>
    public static KatanaParameterDefinition GlobalEq2LowMidQ { get; } =
        new("global-eq2-lomid-q", "Global EQ 2 Low-Mid Q",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.LowMidQ),
            maximum: 5, description: "Adjusts the bandwidth of the low-mid band for bank 2.");

    /// <summary>Parameter definition for Global Eq 2 Low Mid Gain.</summary>
    public static KatanaParameterDefinition GlobalEq2LowMidGain { get; } =
        new("global-eq2-lomid-gain", "Global EQ 2 Low-Mid",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.LowMidGain),
            0, 40, description: "Adjusts the low-mid frequency tone for bank 2 (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 2 Hi Mid Freq.</summary>
    public static KatanaParameterDefinition GlobalEq2HiMidFreq { get; } =
        new("global-eq2-himid-freq", "Global EQ 2 High-Mid Freq",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.HiMidFreq),
            maximum: 27, description: "High-mid center frequency for Global EQ bank 2 (20 Hz-10.0 kHz).");

    /// <summary>Parameter definition for Global Eq 2 Hi Mid Q.</summary>
    public static KatanaParameterDefinition GlobalEq2HiMidQ { get; } =
        new("global-eq2-himid-q", "Global EQ 2 High-Mid Q",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.HiMidQ),
            maximum: 5, description: "Adjusts the bandwidth of the high-mid band for bank 2.");

    /// <summary>Parameter definition for Global Eq 2 Hi Mid Gain.</summary>
    public static KatanaParameterDefinition GlobalEq2HiMidGain { get; } =
        new("global-eq2-himid-gain", "Global EQ 2 High-Mid",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.HiMidGain),
            0, 40, description: "Adjusts the high-mid frequency tone for bank 2 (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 2 High Gain.</summary>
    public static KatanaParameterDefinition GlobalEq2HighGain { get; } =
        new("global-eq2-high-gain", "Global EQ 2 High",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.HighGain),
            0, 40, description: "Adjusts the high frequency tone for bank 2 (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 2 High Cut.</summary>
    public static KatanaParameterDefinition GlobalEq2HighCut { get; } =
        new("global-eq2-high-cut", "Global EQ 2 High Cut",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.HighCut),
            maximum: 14, description: "Sets the high cut frequency for Global EQ bank 2 (630 Hz-FLAT).");

    /// <summary>Parameter definition for Global Eq 2 Level.</summary>
    public static KatanaParameterDefinition GlobalEq2Level { get; } =
        new("global-eq2-level", "Global EQ 2 Level",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Level),
            0, 40, description: "Overall volume level of Global EQ bank 2 (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq 3 1 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq2Geq31Hz { get; } =
        new("global-eq2-geq-31hz", "Global EQ 2 31 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Geq31Hz),
            maximum: 48, description: "GE-10 graphic EQ 2 band at 31 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq6 2 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq2Geq62Hz { get; } =
        new("global-eq2-geq-62hz", "Global EQ 2 62 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Geq62Hz),
            maximum: 48, description: "GE-10 graphic EQ 2 band at 62 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq 1 25 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq2Geq125Hz { get; } =
        new("global-eq2-geq-125hz", "Global EQ 2 125 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Geq125Hz),
            maximum: 48, description: "GE-10 graphic EQ 2 band at 125 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq 250 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq2Geq250Hz { get; } =
        new("global-eq2-geq-250hz", "Global EQ 2 250 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Geq250Hz),
            maximum: 48, description: "GE-10 graphic EQ 2 band at 250 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq500 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq2Geq500Hz { get; } =
        new("global-eq2-geq-500hz", "Global EQ 2 500 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Geq500Hz),
            maximum: 48, description: "GE-10 graphic EQ 2 band at 500 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq 1k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq2Geq1kHz { get; } =
        new("global-eq2-geq-1khz", "Global EQ 2 1 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Geq1kHz),
            maximum: 48, description: "GE-10 graphic EQ 2 band at 1 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq 2k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq2Geq2kHz { get; } =
        new("global-eq2-geq-2khz", "Global EQ 2 2 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Geq2kHz),
            maximum: 48, description: "GE-10 graphic EQ 2 band at 2 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq4k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq2Geq4kHz { get; } =
        new("global-eq2-geq-4khz", "Global EQ 2 4 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Geq4kHz),
            maximum: 48, description: "GE-10 graphic EQ 2 band at 4 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq8k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq2Geq8kHz { get; } =
        new("global-eq2-geq-8khz", "Global EQ 2 8 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Geq8kHz),
            maximum: 48, description: "GE-10 graphic EQ 2 band at 8 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq 16k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq2Geq16kHz { get; } =
        new("global-eq2-geq-16khz", "Global EQ 2 16 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.Geq16kHz),
            maximum: 48, description: "GE-10 graphic EQ 2 band at 16 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 2 Geq Level.</summary>
    public static KatanaParameterDefinition GlobalEq2GeqLevel { get; } =
        new("global-eq2-geq-level", "Global EQ 2 GEQ Level",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq2, KatanaAddressMap.GlobalEqParams.GeqLevel),
            maximum: 48, description: "Overall output level of GE-10 graphic EQ bank 2 (-24 to +24 dB).");

    /// <summary> Global EQ Bank 3 (SysEq3, block 0x0070) ───────────────────────────────── </summary>
    public static KatanaParameterDefinition GlobalEq3Sw { get; } =
        new("global-eq3-sw", "Global EQ 3 On/Off",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.SystemMainParams.EqSw),
            maximum: 1, description: "Turns Global EQ bank 3 on/off.");

    /// <summary>Parameter definition for Global Eq 3 Type.</summary>
    public static KatanaParameterDefinition GlobalEq3Type { get; } =
        new("global-eq3-type", "Global EQ 3 Type",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Type),
            maximum: 1, description: "Selects the EQ type for bank 3: PARAMETRIC EQ or GE-10.");

    /// <summary>Parameter definition for Global Eq 3 Position.</summary>
    public static KatanaParameterDefinition GlobalEq3Position { get; } =
        new("global-eq3-position", "Global EQ 3 Position",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Position),
            maximum: 3, description: "Positions Global EQ bank 3 in the signal chain (Ver200+).");

    /// <summary>Parameter definition for Global Eq 3 Low Cut.</summary>
    public static KatanaParameterDefinition GlobalEq3LowCut { get; } =
        new("global-eq3-low-cut", "Global EQ 3 Low Cut",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.LowCut),
            maximum: 17, description: "Sets the low cut frequency for Global EQ bank 3.");

    /// <summary>Parameter definition for Global Eq 3 Low Gain.</summary>
    public static KatanaParameterDefinition GlobalEq3LowGain { get; } =
        new("global-eq3-low-gain", "Global EQ 3 Low",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.LowGain),
            0, 40, description: "Adjusts the low frequency tone for bank 3 (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 3 Low Mid Freq.</summary>
    public static KatanaParameterDefinition GlobalEq3LowMidFreq { get; } =
        new("global-eq3-lomid-freq", "Global EQ 3 Low-Mid Freq",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.LowMidFreq),
            maximum: 27, description: "Low-mid center frequency for Global EQ bank 3 (20 Hz-10.0 kHz).");

    /// <summary>Parameter definition for Global Eq 3 Low Mid Q.</summary>
    public static KatanaParameterDefinition GlobalEq3LowMidQ { get; } =
        new("global-eq3-lomid-q", "Global EQ 3 Low-Mid Q",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.LowMidQ),
            maximum: 5, description: "Adjusts the bandwidth of the low-mid band for bank 3.");

    /// <summary>Parameter definition for Global Eq 3 Low Mid Gain.</summary>
    public static KatanaParameterDefinition GlobalEq3LowMidGain { get; } =
        new("global-eq3-lomid-gain", "Global EQ 3 Low-Mid",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.LowMidGain),
            0, 40, description: "Adjusts the low-mid frequency tone for bank 3 (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 3 Hi Mid Freq.</summary>
    public static KatanaParameterDefinition GlobalEq3HiMidFreq { get; } =
        new("global-eq3-himid-freq", "Global EQ 3 High-Mid Freq",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.HiMidFreq),
            maximum: 27, description: "High-mid center frequency for Global EQ bank 3 (20 Hz-10.0 kHz).");

    /// <summary>Parameter definition for Global Eq 3 Hi Mid Q.</summary>
    public static KatanaParameterDefinition GlobalEq3HiMidQ { get; } =
        new("global-eq3-himid-q", "Global EQ 3 High-Mid Q",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.HiMidQ),
            maximum: 5, description: "Adjusts the bandwidth of the high-mid band for bank 3.");

    /// <summary>Parameter definition for Global Eq 3 Hi Mid Gain.</summary>
    public static KatanaParameterDefinition GlobalEq3HiMidGain { get; } =
        new("global-eq3-himid-gain", "Global EQ 3 High-Mid",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.HiMidGain),
            0, 40, description: "Adjusts the high-mid frequency tone for bank 3 (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 3 High Gain.</summary>
    public static KatanaParameterDefinition GlobalEq3HighGain { get; } =
        new("global-eq3-high-gain", "Global EQ 3 High",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.HighGain),
            0, 40, description: "Adjusts the high frequency tone for bank 3 (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 3 High Cut.</summary>
    public static KatanaParameterDefinition GlobalEq3HighCut { get; } =
        new("global-eq3-high-cut", "Global EQ 3 High Cut",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.HighCut),
            maximum: 14, description: "Sets the high cut frequency for Global EQ bank 3 (630 Hz-FLAT).");

    /// <summary>Parameter definition for Global Eq 3 Level.</summary>
    public static KatanaParameterDefinition GlobalEq3Level { get; } =
        new("global-eq3-level", "Global EQ 3 Level",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Level),
            0, 40, description: "Overall volume level of Global EQ bank 3 (-20 to +20 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq 3 1 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq3Geq31Hz { get; } =
        new("global-eq3-geq-31hz", "Global EQ 3 31 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Geq31Hz),
            maximum: 48, description: "GE-10 graphic EQ 3 band at 31 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq6 2 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq3Geq62Hz { get; } =
        new("global-eq3-geq-62hz", "Global EQ 3 62 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Geq62Hz),
            maximum: 48, description: "GE-10 graphic EQ 3 band at 62 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq 1 25 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq3Geq125Hz { get; } =
        new("global-eq3-geq-125hz", "Global EQ 3 125 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Geq125Hz),
            maximum: 48, description: "GE-10 graphic EQ 3 band at 125 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq 250 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq3Geq250Hz { get; } =
        new("global-eq3-geq-250hz", "Global EQ 3 250 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Geq250Hz),
            maximum: 48, description: "GE-10 graphic EQ 3 band at 250 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq500 Hz.</summary>
    public static KatanaParameterDefinition GlobalEq3Geq500Hz { get; } =
        new("global-eq3-geq-500hz", "Global EQ 3 500 Hz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Geq500Hz),
            maximum: 48, description: "GE-10 graphic EQ 3 band at 500 Hz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq 1k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq3Geq1kHz { get; } =
        new("global-eq3-geq-1khz", "Global EQ 3 1 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Geq1kHz),
            maximum: 48, description: "GE-10 graphic EQ 3 band at 1 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq 2k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq3Geq2kHz { get; } =
        new("global-eq3-geq-2khz", "Global EQ 3 2 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Geq2kHz),
            maximum: 48, description: "GE-10 graphic EQ 3 band at 2 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq4k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq3Geq4kHz { get; } =
        new("global-eq3-geq-4khz", "Global EQ 3 4 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Geq4kHz),
            maximum: 48, description: "GE-10 graphic EQ 3 band at 4 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq8k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq3Geq8kHz { get; } =
        new("global-eq3-geq-8khz", "Global EQ 3 8 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Geq8kHz),
            maximum: 48, description: "GE-10 graphic EQ 3 band at 8 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq 16k Hz.</summary>
    public static KatanaParameterDefinition GlobalEq3Geq16kHz { get; } =
        new("global-eq3-geq-16khz", "Global EQ 3 16 kHz",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.Geq16kHz),
            maximum: 48, description: "GE-10 graphic EQ 3 band at 16 kHz (-24 to +24 dB).");

    /// <summary>Parameter definition for Global Eq 3 Geq Level.</summary>
    public static KatanaParameterDefinition GlobalEq3GeqLevel { get; } =
        new("global-eq3-geq-level", "Global EQ 3 GEQ Level",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.System,
                KatanaAddressMap.SystemBlocks.GlobalEq3, KatanaAddressMap.GlobalEqParams.GeqLevel),
            maximum: 48, description: "Overall output level of GE-10 graphic EQ bank 3 (-24 to +24 dB).");
}
