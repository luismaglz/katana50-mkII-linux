namespace Kataka.Domain.Midi;

public static partial class KatanaMkIIParameterCatalog
{
    // ── Panel-mode knob positions (PRM_KNOB_POS_*, Status block 0x0650) ──────────
    // These reflect the physical front-panel knob positions. In PANEL mode the amp
    // drives its sound directly from these values. In CHANNEL mode the stored preamp
    // params (PreampGain / PreampBass etc., see below) govern the sound, but the amp
    // still pushes KNOB_POS updates when knobs are physically moved.
    public static KatanaParameterDefinition AmpGain { get; } =
        new("amp-gain", "Gain", [0x60, 0x00, 0x06, 0x51]);

    public static KatanaParameterDefinition AmpVolume { get; } =
        new("amp-volume", "Volume", [0x60, 0x00, 0x06, 0x52]);

    public static KatanaParameterDefinition AmpBass { get; } =
        new("amp-bass", "Bass", [0x60, 0x00, 0x06, 0x53]);

    public static KatanaParameterDefinition AmpMiddle { get; } =
        new("amp-middle", "Middle", [0x60, 0x00, 0x06, 0x54]);

    public static KatanaParameterDefinition AmpTreble { get; } =
        new("amp-treble", "Treble", [0x60, 0x00, 0x06, 0x55]);

    public static KatanaParameterDefinition AmpPresence { get; } =
        new("amp-presence", "Presence", [0x60, 0x00, 0x06, 0x56]);

    public static IReadOnlyList<KatanaParameterDefinition> AmpEditorControls { get; } =
    [
        AmpGain,
        AmpVolume,
        AmpBass,
        AmpMiddle,
        AmpTreble,
        AmpPresence,
    ];

    public static KatanaParameterDefinition BoosterSwitch { get; } =
        new("panel-booster-switch", "Booster", [0x60, 0x00, 0x00, 0x10], maximum: 1);

    public static KatanaParameterDefinition BoosterType { get; } =
        new("panel-booster-type", "Booster Type", [0x60, 0x00, 0x00, 0x11], maximum: 22,
            skippedValues: [0x07]);

    public static KatanaParameterDefinition ModSwitch { get; } =
        new("panel-mod-switch", "Mod", [0x60, 0x00, 0x01, 0x00], maximum: 1);

    public static KatanaParameterDefinition ModType { get; } =
        new("panel-mod-type", "Mod Type", [0x60, 0x00, 0x01, 0x01], maximum: 39,
            skippedValues: [0x05, 0x08, 0x0B, 0x0D, 0x11, 0x18, 0x1E, 0x20, 0x21, 0x22]);

    public static KatanaParameterDefinition FxSwitch { get; } =
        new("panel-fx-switch", "FX", [0x60, 0x00, 0x03, 0x00], maximum: 1);

    public static KatanaParameterDefinition FxType { get; } =
        new("panel-fx-type", "FX Type", [0x60, 0x00, 0x03, 0x01], maximum: 39,
            skippedValues: [0x05, 0x08, 0x0B, 0x0D, 0x11, 0x18, 0x1E, 0x20, 0x21, 0x22]);

    public static KatanaParameterDefinition DelaySwitch { get; } =
        new("panel-delay-switch", "Delay", [0x60, 0x00, 0x05, 0x00], maximum: 1);

    public static KatanaParameterDefinition DelayType { get; } =
        new("panel-delay-type", "Delay Type", [0x60, 0x00, 0x05, 0x01], maximum: 10);

    public static KatanaParameterDefinition Delay2Switch { get; } =
        new("panel-delay2-switch", "Delay 2", [0x60, 0x00, 0x05, 0x20], maximum: 1);

    public static KatanaParameterDefinition Delay2Type { get; } =
        new("panel-delay2-type", "Delay 2 Type", [0x60, 0x00, 0x05, 0x21], maximum: 10);

    public static KatanaParameterDefinition ReverbSwitch { get; } =
        new("panel-reverb-switch", "Reverb", [0x60, 0x00, 0x05, 0x40], maximum: 1);

    public static KatanaParameterDefinition ReverbType { get; } =
        new("panel-reverb-type", "Reverb Type", [0x60, 0x00, 0x05, 0x41], maximum: 6);

    public static KatanaParameterDefinition BoosterVariation { get; } =
        new("panel-booster-variation", "Booster Variation", [0x60, 0x00, 0x06, 0x39], maximum: 2);

    public static KatanaParameterDefinition ModVariation { get; } =
        new("panel-mod-variation", "Mod Variation", [0x60, 0x00, 0x06, 0x3A], maximum: 2);

    public static KatanaParameterDefinition FxVariation { get; } =
        new("panel-fx-variation", "FX Variation", [0x60, 0x00, 0x06, 0x3B], maximum: 2);

    public static KatanaParameterDefinition DelayVariation { get; } =
        new("panel-delay-variation", "Delay Variation", [0x60, 0x00, 0x06, 0x3C], maximum: 2);

    public static KatanaParameterDefinition ReverbVariation { get; } =
        new("panel-reverb-variation", "Reverb Variation", [0x60, 0x00, 0x06, 0x3D], maximum: 2);

    // KNOB_POS level controls (0x60000657-65b) — front-panel knob positions for each effect.
    // Raw byte 0x00 means "knob not yet moved" (amp source value −1); treated here as 0.
    public static KatanaParameterDefinition BoostLevel { get; } =
        new("panel-boost-level", "Boost Level", [0x60, 0x00, 0x06, 0x57]);

    public static KatanaParameterDefinition ModLevel { get; } =
        new("panel-mod-level", "Mod Level", [0x60, 0x00, 0x06, 0x58]);

    public static KatanaParameterDefinition FxLevel { get; } =
        new("panel-fx-level", "FX Level", [0x60, 0x00, 0x06, 0x59]);

    public static KatanaParameterDefinition DelayLevel { get; } =
        new("panel-delay-level", "Delay Level", [0x60, 0x00, 0x06, 0x5A]);

    public static KatanaParameterDefinition ReverbLevel { get; } =
        new("panel-reverb-level", "Reverb Level", [0x60, 0x00, 0x06, 0x5B]);

    // Panel-mode amp type knob position (PRM_KNOB_POS_TYPE, 0-4: ACOUSTIC/CLEAN/CRUNCH/LEAD/BROWN).
    // For channel-mode amp character selection (0-32) use PreampType.
    public static KatanaParameterDefinition AmpType { get; } =
        new("amp-type", "Amp Type", [0x60, 0x00, 0x06, 0x50], maximum: 4);

    // Amp variation (TYPE 1 / TYPE 2 voicing toggle). PRM_LED_STATE_VARI.
    // PATCH_STATUS block base 0x650, offset 0x0C → 0x65C → 60 00 06 5C.
    public static KatanaParameterDefinition AmpVariation { get; } =
        new("amp-variation", "Variation", [0x60, 0x00, 0x06, 0x5C], maximum: 1);

    // Cabinet resonance (0-2: LOW / MIDDLE / HIGH).
    public static KatanaParameterDefinition CabinetResonance { get; } =
        new("amp-cabinet-resonance", "Cabinet", [0x60, 0x00, 0x06, 0x43], maximum: 2);

    public static KatanaParameterDefinition PedalFxSwitch { get; } =
        new("pedal-fx-switch", "Pedal FX", [0x60, 0x00, 0x05, 0x50], maximum: 1);

    public static KatanaParameterDefinition PedalFxType { get; } =
        new("pedal-fx-type", "Pedal FX Type", [0x60, 0x00, 0x05, 0x51], maximum: 2);

    public static KatanaParameterDefinition PedalFxPosition { get; } =
        new("pedal-fx-position", "Pedal FX Position", [0x60, 0x00, 0x06, 0x23], maximum: 1);

    public static KatanaParameterDefinition PedalFxWahType { get; } =
        new("pedal-fx-wah-type", "Wah Type", [0x60, 0x00, 0x05, 0x52], maximum: 5);

    public static KatanaParameterDefinition PedalFxWahPedalPosition { get; } =
        new("pedal-fx-wah-position", "Wah Pedal Position", [0x60, 0x00, 0x05, 0x53]);

    public static KatanaParameterDefinition PedalFxWahPedalMinimum { get; } =
        new("pedal-fx-wah-min", "Wah Pedal Minimum", [0x60, 0x00, 0x05, 0x54]);

    public static KatanaParameterDefinition PedalFxWahPedalMaximum { get; } =
        new("pedal-fx-wah-max", "Wah Pedal Maximum", [0x60, 0x00, 0x05, 0x55]);

    public static KatanaParameterDefinition PedalFxWahEffectLevel { get; } =
        new("pedal-fx-wah-effect-level", "Wah Effect Level", [0x60, 0x00, 0x05, 0x56]);

    public static KatanaParameterDefinition PedalFxWahDirectMix { get; } =
        new("pedal-fx-wah-direct-mix", "Wah Direct Mix", [0x60, 0x00, 0x05, 0x57]);

    public static KatanaParameterDefinition PedalFxBendPitch { get; } =
        new("pedal-fx-bend-pitch", "Pitch", [0x60, 0x00, 0x05, 0x58], minimum: 0, maximum: 48);

    public static KatanaParameterDefinition PedalFxBendPedalPosition { get; } =
        new("pedal-fx-bend-position", "Pedal Pos", [0x60, 0x00, 0x05, 0x59]);

    public static KatanaParameterDefinition PedalFxBendEffectLevel { get; } =
        new("pedal-fx-bend-effect-level", "Effect Level", [0x60, 0x00, 0x05, 0x5A]);

    public static KatanaParameterDefinition PedalFxBendDirectMix { get; } =
        new("pedal-fx-bend-direct-mix", "Direct Mix", [0x60, 0x00, 0x05, 0x5B]);

    public static KatanaParameterDefinition PedalFxEvh95Position { get; } =
        new("pedal-fx-evh95-position", "Pedal Pos", [0x60, 0x00, 0x05, 0x5C]);

    public static KatanaParameterDefinition PedalFxEvh95Minimum { get; } =
        new("pedal-fx-evh95-min", "Pedal Min", [0x60, 0x00, 0x05, 0x5D]);

    public static KatanaParameterDefinition PedalFxEvh95Maximum { get; } =
        new("pedal-fx-evh95-max", "Pedal Max", [0x60, 0x00, 0x05, 0x5E]);

    public static KatanaParameterDefinition PedalFxEvh95EffectLevel { get; } =
        new("pedal-fx-evh95-effect-level", "Effect Level", [0x60, 0x00, 0x05, 0x5F]);

    public static KatanaParameterDefinition PedalFxEvh95DirectMix { get; } =
        new("pedal-fx-evh95-direct-mix", "Direct Mix", [0x60, 0x00, 0x05, 0x60]);

    public static KatanaParameterDefinition FootVolume { get; } =
        new("pedal-fx-foot-volume", "Foot Volume", [0x60, 0x00, 0x05, 0x61]);

    public static KatanaParameterDefinition PatchLevel { get; } =
        new("panel-patch-level", "Patch Level", [0x60, 0x00, 0x06, 0x4C], maximum: 200);

    // ── Solo EQ (Ver200+, Mk2V2 block, patch offset 0xF10) ──────────────────────
    public static KatanaParameterDefinition SoloEqPosition { get; } =
        new("solo-eq-position", "Solo EQ Position", [0x60, 0x00, 0x0F, 0x10], maximum: 1);

    public static KatanaParameterDefinition SoloEqSw { get; } =
        new("solo-eq-sw", "Solo EQ", [0x60, 0x00, 0x0F, 0x11], maximum: 1);

    public static KatanaParameterDefinition SoloEqLowCut { get; } =
        new("solo-eq-low-cut", "Solo EQ Low Cut", [0x60, 0x00, 0x0F, 0x12], maximum: 17);

    public static KatanaParameterDefinition SoloEqLowGain { get; } =
        new("solo-eq-low-gain", "Solo EQ Low", [0x60, 0x00, 0x0F, 0x13], minimum: 0, maximum: 48);

    public static KatanaParameterDefinition SoloEqMidFreq { get; } =
        new("solo-eq-mid-freq", "Solo EQ Mid Freq", [0x60, 0x00, 0x0F, 0x14], maximum: 27);

    public static KatanaParameterDefinition SoloEqMidQ { get; } =
        new("solo-eq-mid-q", "Solo EQ Mid Q", [0x60, 0x00, 0x0F, 0x15], maximum: 5);

    public static KatanaParameterDefinition SoloEqMidGain { get; } =
        new("solo-eq-mid-gain", "Solo EQ Mid", [0x60, 0x00, 0x0F, 0x16], minimum: 0, maximum: 48);

    public static KatanaParameterDefinition SoloEqHighGain { get; } =
        new("solo-eq-high-gain", "Solo EQ High", [0x60, 0x00, 0x0F, 0x17], minimum: 0, maximum: 48);

    public static KatanaParameterDefinition SoloEqHighCut { get; } =
        new("solo-eq-high-cut", "Solo EQ High Cut", [0x60, 0x00, 0x0F, 0x18], maximum: 14);

    public static KatanaParameterDefinition SoloEqLevel { get; } =
        new("solo-eq-level", "Solo EQ Level", [0x60, 0x00, 0x0F, 0x19], minimum: 0, maximum: 48);

    // ── Solo Delay (Ver210+, same Mk2V2 block) ───────────────────────────────────
    public static KatanaParameterDefinition SoloDelaySw { get; } =
        new("solo-delay-sw", "Solo Delay", [0x60, 0x00, 0x0F, 0x1A], maximum: 1);

    public static KatanaParameterDefinition SoloDelayCarryover { get; } =
        new("solo-delay-carryover", "Solo Dly Carry", [0x60, 0x00, 0x0F, 0x1B], maximum: 1);

    // Solo Delay Time is INTEGER2x7 (2 bytes); address of first byte.
    public static IReadOnlyList<byte> SoloDelayTimeAddress { get; } = [0x60, 0x00, 0x0F, 0x1C];

    public static KatanaParameterDefinition SoloDelayFeedback { get; } =
        new("solo-delay-feedback", "Solo Dly Feedback", [0x60, 0x00, 0x0F, 0x1E], maximum: 100);

    public static KatanaParameterDefinition SoloDelayEffectLevel { get; } =
        new("solo-delay-effect-level", "Solo Dly Level", [0x60, 0x00, 0x0F, 0x1F], maximum: 120);

    public static KatanaParameterDefinition SoloDelayDirectLevel { get; } =
        new("solo-delay-direct-level", "Solo Dly Direct", [0x60, 0x00, 0x0F, 0x20], maximum: 100);

    public static KatanaParameterDefinition SoloDelayFilter { get; } =
        new("solo-delay-filter", "Solo Dly Filter", [0x60, 0x00, 0x0F, 0x21], maximum: 2);

    public static KatanaParameterDefinition SoloDelayHighCut { get; } =
        new("solo-delay-high-cut", "Solo Dly Hi Cut", [0x60, 0x00, 0x0F, 0x22], maximum: 14);

    public static KatanaParameterDefinition SoloDelayModSw { get; } =
        new("solo-delay-mod-sw", "Solo Dly Mod", [0x60, 0x00, 0x0F, 0x23], maximum: 1);

    public static KatanaParameterDefinition SoloDelayModRate { get; } =
        new("solo-delay-mod-rate", "Solo Dly Mod Rate", [0x60, 0x00, 0x0F, 0x24], maximum: 100);

    public static KatanaParameterDefinition SoloDelayModDepth { get; } =
        new("solo-delay-mod-depth", "Solo Dly Mod Depth", [0x60, 0x00, 0x0F, 0x25], maximum: 100);

    // ── Contour (Ver200+, patch offsets 0xF30/0xF38/0xF40) ──────────────────────
    // Three contour blocks share the same parameter structure (prm_prop_contour).
    // Contour(1) = 0xF30, Contour(2) = 0xF38, Contour(3) = 0xF40.
    public static KatanaParameterDefinition Contour1Type { get; } =
        new("contour1-type", "Contour 1 Type", [0x60, 0x00, 0x0F, 0x30], maximum: 3);

    public static KatanaParameterDefinition Contour1FreqShift { get; } =
        new("contour1-freq-shift", "Contour 1 Freq", [0x60, 0x00, 0x0F, 0x31], minimum: 0, maximum: 100);

    public static KatanaParameterDefinition Contour2Type { get; } =
        new("contour2-type", "Contour 2 Type", [0x60, 0x00, 0x0F, 0x38], maximum: 3);

    public static KatanaParameterDefinition Contour2FreqShift { get; } =
        new("contour2-freq-shift", "Contour 2 Freq", [0x60, 0x00, 0x0F, 0x39], minimum: 0, maximum: 100);

    public static KatanaParameterDefinition Contour3Type { get; } =
        new("contour3-type", "Contour 3 Type", [0x60, 0x00, 0x0F, 0x40], maximum: 3);

    public static KatanaParameterDefinition Contour3FreqShift { get; } =
        new("contour3-freq-shift", "Contour 3 Freq", [0x60, 0x00, 0x0F, 0x41], minimum: 0, maximum: 100);

    // ── Patch EQ — Patch_0 embedded EQ block (BTS prm_prop_patch_0 addrs 0x30–0x47) ──
    // Absolute addresses: Patch_0 base (0x10) + relative (0x30–0x47) = 0x40–0x57.
    // Two EQ blocks exist per patch: Eq(1) at 0x40 and Eq(2) at 0x60.
    public static KatanaParameterDefinition PatchEq1Sw { get; } =
        new("patch-eq1-sw", "Patch EQ SW", [0x60, 0x00, 0x00, 0x40], maximum: 1);
    public static KatanaParameterDefinition PatchEq1Type { get; } =
        new("patch-eq1-type", "Patch EQ Type", [0x60, 0x00, 0x00, 0x41], maximum: 1);
    public static KatanaParameterDefinition PatchEq1LowCut { get; } =
        new("patch-eq1-low-cut", "Patch EQ Low Cut", [0x60, 0x00, 0x00, 0x42], maximum: 17);
    public static KatanaParameterDefinition PatchEq1LowGain { get; } =
        new("patch-eq1-low-gain", "Patch EQ Low Gain", [0x60, 0x00, 0x00, 0x43], maximum: 40);
    public static KatanaParameterDefinition PatchEq1LowMidFreq { get; } =
        new("patch-eq1-lomid-freq", "Patch EQ LM Freq", [0x60, 0x00, 0x00, 0x44], maximum: 27);
    public static KatanaParameterDefinition PatchEq1LowMidQ { get; } =
        new("patch-eq1-lomid-q", "Patch EQ LM Q", [0x60, 0x00, 0x00, 0x45], maximum: 5);
    public static KatanaParameterDefinition PatchEq1LowMidGain { get; } =
        new("patch-eq1-lomid-gain", "Patch EQ LM Gain", [0x60, 0x00, 0x00, 0x46], maximum: 40);
    public static KatanaParameterDefinition PatchEq1HighMidFreq { get; } =
        new("patch-eq1-himid-freq", "Patch EQ HM Freq", [0x60, 0x00, 0x00, 0x47], maximum: 27);
    public static KatanaParameterDefinition PatchEq1HighMidQ { get; } =
        new("patch-eq1-himid-q", "Patch EQ HM Q", [0x60, 0x00, 0x00, 0x48], maximum: 5);
    public static KatanaParameterDefinition PatchEq1HighMidGain { get; } =
        new("patch-eq1-himid-gain", "Patch EQ HM Gain", [0x60, 0x00, 0x00, 0x49], maximum: 40);
    public static KatanaParameterDefinition PatchEq1HighGain { get; } =
        new("patch-eq1-high-gain", "Patch EQ High Gain", [0x60, 0x00, 0x00, 0x4A], maximum: 40);
    public static KatanaParameterDefinition PatchEq1HighCut { get; } =
        new("patch-eq1-high-cut", "Patch EQ High Cut", [0x60, 0x00, 0x00, 0x4B], maximum: 14);
    public static KatanaParameterDefinition PatchEq1Level { get; } =
        new("patch-eq1-level", "Patch EQ Level", [0x60, 0x00, 0x00, 0x4C], maximum: 40);
    public static KatanaParameterDefinition PatchEq1Geq31Hz { get; } =
        new("patch-eq1-geq-31hz", "31Hz", [0x60, 0x00, 0x00, 0x4D], maximum: 48);
    public static KatanaParameterDefinition PatchEq1Geq62Hz { get; } =
        new("patch-eq1-geq-62hz", "62Hz", [0x60, 0x00, 0x00, 0x4E], maximum: 48);
    public static KatanaParameterDefinition PatchEq1Geq125Hz { get; } =
        new("patch-eq1-geq-125hz", "125Hz", [0x60, 0x00, 0x00, 0x4F], maximum: 48);
    public static KatanaParameterDefinition PatchEq1Geq250Hz { get; } =
        new("patch-eq1-geq-250hz", "250Hz", [0x60, 0x00, 0x00, 0x50], maximum: 48);
    public static KatanaParameterDefinition PatchEq1Geq500Hz { get; } =
        new("patch-eq1-geq-500hz", "500Hz", [0x60, 0x00, 0x00, 0x51], maximum: 48);
    public static KatanaParameterDefinition PatchEq1Geq1kHz { get; } =
        new("patch-eq1-geq-1khz", "1kHz", [0x60, 0x00, 0x00, 0x52], maximum: 48);
    public static KatanaParameterDefinition PatchEq1Geq2kHz { get; } =
        new("patch-eq1-geq-2khz", "2kHz", [0x60, 0x00, 0x00, 0x53], maximum: 48);
    public static KatanaParameterDefinition PatchEq1Geq4kHz { get; } =
        new("patch-eq1-geq-4khz", "4kHz", [0x60, 0x00, 0x00, 0x54], maximum: 48);
    public static KatanaParameterDefinition PatchEq1Geq8kHz { get; } =
        new("patch-eq1-geq-8khz", "8kHz", [0x60, 0x00, 0x00, 0x55], maximum: 48);
    public static KatanaParameterDefinition PatchEq1Geq16kHz { get; } =
        new("patch-eq1-geq-16khz", "16kHz", [0x60, 0x00, 0x00, 0x56], maximum: 48);
    public static KatanaParameterDefinition PatchEq1GeqLevel { get; } =
        new("patch-eq1-geq-level", "GEQ Level", [0x60, 0x00, 0x00, 0x57], maximum: 48);

    // ── Patch Eq(2) — second EQ block (BTS prm_prop_patch_eq2 at 0x0060) ──────────
    public static KatanaParameterDefinition PatchEq2Sw { get; } =
        new("patch-eq2-sw", "Patch EQ2 SW", [0x60, 0x00, 0x00, 0x60], maximum: 1);
    public static KatanaParameterDefinition PatchEq2Type { get; } =
        new("patch-eq2-type", "Patch EQ2 Type", [0x60, 0x00, 0x00, 0x61], maximum: 1);
    public static KatanaParameterDefinition PatchEq2LowCut { get; } =
        new("patch-eq2-low-cut", "Patch EQ2 Low Cut", [0x60, 0x00, 0x00, 0x62], maximum: 17);
    public static KatanaParameterDefinition PatchEq2LowGain { get; } =
        new("patch-eq2-low-gain", "Patch EQ2 Low Gain", [0x60, 0x00, 0x00, 0x63], maximum: 40);
    public static KatanaParameterDefinition PatchEq2LowMidFreq { get; } =
        new("patch-eq2-lomid-freq", "Patch EQ2 LM Freq", [0x60, 0x00, 0x00, 0x64], maximum: 27);
    public static KatanaParameterDefinition PatchEq2LowMidQ { get; } =
        new("patch-eq2-lomid-q", "Patch EQ2 LM Q", [0x60, 0x00, 0x00, 0x65], maximum: 5);
    public static KatanaParameterDefinition PatchEq2LowMidGain { get; } =
        new("patch-eq2-lomid-gain", "Patch EQ2 LM Gain", [0x60, 0x00, 0x00, 0x66], maximum: 40);
    public static KatanaParameterDefinition PatchEq2HighMidFreq { get; } =
        new("patch-eq2-himid-freq", "Patch EQ2 HM Freq", [0x60, 0x00, 0x00, 0x67], maximum: 27);
    public static KatanaParameterDefinition PatchEq2HighMidQ { get; } =
        new("patch-eq2-himid-q", "Patch EQ2 HM Q", [0x60, 0x00, 0x00, 0x68], maximum: 5);
    public static KatanaParameterDefinition PatchEq2HighMidGain { get; } =
        new("patch-eq2-himid-gain", "Patch EQ2 HM Gain", [0x60, 0x00, 0x00, 0x69], maximum: 40);
    public static KatanaParameterDefinition PatchEq2HighGain { get; } =
        new("patch-eq2-high-gain", "Patch EQ2 High Gain", [0x60, 0x00, 0x00, 0x6A], maximum: 40);
    public static KatanaParameterDefinition PatchEq2HighCut { get; } =
        new("patch-eq2-high-cut", "Patch EQ2 High Cut", [0x60, 0x00, 0x00, 0x6B], maximum: 14);
    public static KatanaParameterDefinition PatchEq2Level { get; } =
        new("patch-eq2-level", "Patch EQ2 Level", [0x60, 0x00, 0x00, 0x6C], maximum: 40);
    public static KatanaParameterDefinition PatchEq2Geq31Hz { get; } =
        new("patch-eq2-geq-31hz", "31Hz", [0x60, 0x00, 0x00, 0x6D], maximum: 48);
    public static KatanaParameterDefinition PatchEq2Geq62Hz { get; } =
        new("patch-eq2-geq-62hz", "62Hz", [0x60, 0x00, 0x00, 0x6E], maximum: 48);
    public static KatanaParameterDefinition PatchEq2Geq125Hz { get; } =
        new("patch-eq2-geq-125hz", "125Hz", [0x60, 0x00, 0x00, 0x6F], maximum: 48);
    public static KatanaParameterDefinition PatchEq2Geq250Hz { get; } =
        new("patch-eq2-geq-250hz", "250Hz", [0x60, 0x00, 0x00, 0x70], maximum: 48);
    public static KatanaParameterDefinition PatchEq2Geq500Hz { get; } =
        new("patch-eq2-geq-500hz", "500Hz", [0x60, 0x00, 0x00, 0x71], maximum: 48);
    public static KatanaParameterDefinition PatchEq2Geq1kHz { get; } =
        new("patch-eq2-geq-1khz", "1kHz", [0x60, 0x00, 0x00, 0x72], maximum: 48);
    public static KatanaParameterDefinition PatchEq2Geq2kHz { get; } =
        new("patch-eq2-geq-2khz", "2kHz", [0x60, 0x00, 0x00, 0x73], maximum: 48);
    public static KatanaParameterDefinition PatchEq2Geq4kHz { get; } =
        new("patch-eq2-geq-4khz", "4kHz", [0x60, 0x00, 0x00, 0x74], maximum: 48);
    public static KatanaParameterDefinition PatchEq2Geq8kHz { get; } =
        new("patch-eq2-geq-8khz", "8kHz", [0x60, 0x00, 0x00, 0x75], maximum: 48);
    public static KatanaParameterDefinition PatchEq2Geq16kHz { get; } =
        new("patch-eq2-geq-16khz", "16kHz", [0x60, 0x00, 0x00, 0x76], maximum: 48);
    public static KatanaParameterDefinition PatchEq2GeqLevel { get; } =
        new("patch-eq2-geq-level", "GEQ Level", [0x60, 0x00, 0x00, 0x77], maximum: 48);

    public static IReadOnlyList<byte> DelayTimeAddress { get; } = [0x60, 0x00, 0x05, 0x02];

    /// <summary>Signal chain pattern (PRM_CHAIN_PTN). Values 0–6 map to CHAIN 1 / CHAIN 2-1 / CHAIN 3-1 / CHAIN 4-1 / CHAIN 2-2 / CHAIN 3-2 / CHAIN 4-2.</summary>
    public static KatanaParameterDefinition ChainPattern { get; } =
        new("chain-pattern", "Chain Pattern", [0x60, 0x00, 0x06, 0x20], maximum: 6);

    /// <summary>Trigger address for saving current temp state to a patch slot (data = [0x00, slot_0based]).</summary>
    public static IReadOnlyList<byte> PatchWriteAddress { get; } = [0x7F, 0x00, 0x01, 0x04];

    // ── Channel-mode (stored) preamp parameters ─────────────────────────────────
    // In PANEL mode the amp uses knob-position values from the Status block (0x0651+).
    // In CHANNEL mode the amp reads these stored preamp values from Patch_0 (base 0x10).
    // BTS block: prm_prop_patch_0, Temporary patch offset 0x0010 + field offset.

    /// <summary>Amp character type (PRM_PREAMP_A_TYPE). 0-32 covers all BTS amp characters.</summary>
    public static KatanaParameterDefinition PreampType { get; } =
        new("preamp-type", "Preamp Type", [0x60, 0x00, 0x00, 0x21], maximum: 32);

    /// <summary>Gain in channel mode (PRM_PREAMP_A_GAIN). 0-120.</summary>
    public static KatanaParameterDefinition PreampGain { get; } =
        new("preamp-gain", "Preamp Gain", [0x60, 0x00, 0x00, 0x22], maximum: 120);

    public static KatanaParameterDefinition PreampBass { get; } =
        new("preamp-bass", "Preamp Bass", [0x60, 0x00, 0x00, 0x24]);

    public static KatanaParameterDefinition PreampMiddle { get; } =
        new("preamp-middle", "Preamp Middle", [0x60, 0x00, 0x00, 0x25]);

    public static KatanaParameterDefinition PreampTreble { get; } =
        new("preamp-treble", "Preamp Treble", [0x60, 0x00, 0x00, 0x26]);

    public static KatanaParameterDefinition PreampPresence { get; } =
        new("preamp-presence", "Preamp Presence", [0x60, 0x00, 0x00, 0x27]);

    /// <summary>Preamp output level (channel mode). Not the same as the master Volume knob.</summary>
    public static KatanaParameterDefinition PreampLevel { get; } =
        new("preamp-level", "Preamp Level", [0x60, 0x00, 0x00, 0x28]);

    public static KatanaParameterDefinition PreampBright { get; } =
        new("preamp-bright", "Preamp Bright", [0x60, 0x00, 0x00, 0x29], maximum: 1);

    public static KatanaParameterDefinition PreampSoloSw { get; } =
        new("preamp-solo-sw", "Preamp Solo", [0x60, 0x00, 0x00, 0x2B], maximum: 1);

    public static KatanaParameterDefinition PreampSoloLevel { get; } =
        new("preamp-solo-level", "Preamp Solo Lvl", [0x60, 0x00, 0x00, 0x2C]);

    // ── Booster DSP params (same for all booster types) ─────────────────────────
    public static KatanaParameterDefinition BoosterDrive { get; } =
        new("booster-drive", "Drive", [0x60, 0x00, 0x00, 0x12], maximum: 120);

    public static KatanaParameterDefinition BoosterTone { get; } =
        new("booster-tone", "Tone", [0x60, 0x00, 0x00, 0x13]);

    public static KatanaParameterDefinition BoosterBottom { get; } =
        new("booster-bottom", "Bottom", [0x60, 0x00, 0x00, 0x14]);

    public static KatanaParameterDefinition BoosterSoloSw { get; } =
        new("booster-solo-sw", "Solo", [0x60, 0x00, 0x00, 0x15], maximum: 1);

    public static KatanaParameterDefinition BoosterSoloLevel { get; } =
        new("booster-solo-level", "Solo Lvl", [0x60, 0x00, 0x00, 0x16]);

    public static KatanaParameterDefinition BoosterEffectLevel { get; } =
        new("booster-effect-level", "E.Level", [0x60, 0x00, 0x00, 0x17]);

    public static KatanaParameterDefinition BoosterDirectMix { get; } =
        new("booster-direct-mix", "D.Mix", [0x60, 0x00, 0x00, 0x18]);

    // ── Delay DSP params ─────────────────────────────────────────────────────────
    public static KatanaParameterDefinition DelayFeedback { get; } =
        new("delay-feedback", "Feedback", [0x60, 0x00, 0x05, 0x04], maximum: 100);

    public static KatanaParameterDefinition DelayHighCut { get; } =
        new("delay-high-cut", "High Cut", [0x60, 0x00, 0x05, 0x05], maximum: 14);

    public static KatanaParameterDefinition DelayEffectLevel { get; } =
        new("delay-effect-level", "E.Level", [0x60, 0x00, 0x05, 0x06], maximum: 120);

    public static KatanaParameterDefinition DelayDirectMix { get; } =
        new("delay-direct-mix", "D.Mix", [0x60, 0x00, 0x05, 0x07], maximum: 100);

    public static KatanaParameterDefinition DelayTapTime { get; } =
        new("delay-tap-time", "Tap Time", [0x60, 0x00, 0x05, 0x08], maximum: 100);

    public static KatanaParameterDefinition DelayModRate { get; } =
        new("delay-mod-rate", "Mod Rate", [0x60, 0x00, 0x05, 0x13], maximum: 100);

    public static KatanaParameterDefinition DelayModDepth { get; } =
        new("delay-mod-depth", "Mod Depth", [0x60, 0x00, 0x05, 0x14], maximum: 100);

    public static KatanaParameterDefinition DelayRange { get; } =
        new("delay-range", "Range", [0x60, 0x00, 0x05, 0x15], maximum: 1);

    public static KatanaParameterDefinition DelayFilter { get; } =
        new("delay-filter", "Filter", [0x60, 0x00, 0x05, 0x16], maximum: 1);

    public static KatanaParameterDefinition DelayFeedbackPhase { get; } =
        new("delay-feedback-phase", "FB Phase", [0x60, 0x00, 0x05, 0x17], maximum: 1);

    public static KatanaParameterDefinition DelayDelayPhase { get; } =
        new("delay-delay-phase", "Dly Phase", [0x60, 0x00, 0x05, 0x18], maximum: 1);

    public static KatanaParameterDefinition DelayModSw { get; } =
        new("delay-mod-sw", "Mod SW", [0x60, 0x00, 0x05, 0x19], maximum: 1);

    // ── Delay 2 DSP params ───────────────────────────────────────────────────────
    public static KatanaParameterDefinition Delay2Feedback { get; } =
        new("delay2-feedback", "Feedback", [0x60, 0x00, 0x05, 0x24], maximum: 100);

    public static KatanaParameterDefinition Delay2HighCut { get; } =
        new("delay2-high-cut", "High Cut", [0x60, 0x00, 0x05, 0x25], maximum: 14);

    public static KatanaParameterDefinition Delay2EffectLevel { get; } =
        new("delay2-effect-level", "E.Level", [0x60, 0x00, 0x05, 0x26], maximum: 120);

    public static KatanaParameterDefinition Delay2DirectMix { get; } =
        new("delay2-direct-mix", "D.Mix", [0x60, 0x00, 0x05, 0x27], maximum: 100);

    public static KatanaParameterDefinition Delay2TapTime { get; } =
        new("delay2-tap-time", "Tap Time", [0x60, 0x00, 0x05, 0x28], maximum: 100);

    public static KatanaParameterDefinition Delay2ModRate { get; } =
        new("delay2-mod-rate", "Mod Rate", [0x60, 0x00, 0x05, 0x33], maximum: 100);

    public static KatanaParameterDefinition Delay2ModDepth { get; } =
        new("delay2-mod-depth", "Mod Depth", [0x60, 0x00, 0x05, 0x34], maximum: 100);

    public static KatanaParameterDefinition Delay2Range { get; } =
        new("delay2-range", "Range", [0x60, 0x00, 0x05, 0x35], maximum: 1);

    public static KatanaParameterDefinition Delay2Filter { get; } =
        new("delay2-filter", "Filter", [0x60, 0x00, 0x05, 0x36], maximum: 1);

    public static KatanaParameterDefinition Delay2FeedbackPhase { get; } =
        new("delay2-feedback-phase", "FB Phase", [0x60, 0x00, 0x05, 0x37], maximum: 1);

    public static KatanaParameterDefinition Delay2DelayPhase { get; } =
        new("delay2-delay-phase", "Dly Phase", [0x60, 0x00, 0x05, 0x38], maximum: 1);

    public static KatanaParameterDefinition Delay2ModSw { get; } =
        new("delay2-mod-sw", "Mod SW", [0x60, 0x00, 0x05, 0x39], maximum: 1);

    // ── Reverb DSP params (shared for all reverb types) ─────────────────────────
    public static KatanaParameterDefinition ReverbTime { get; } =
        new("reverb-time", "Time", [0x60, 0x00, 0x05, 0x42]);

    public static KatanaParameterDefinition ReverbPreDelay { get; } =
        new("reverb-pre-delay", "Pre Delay", [0x60, 0x00, 0x05, 0x43]);

    public static KatanaParameterDefinition ReverbLowCut { get; } =
        new("reverb-low-cut", "Low Cut", [0x60, 0x00, 0x05, 0x44], maximum: 17);

    public static KatanaParameterDefinition ReverbHighCut { get; } =
        new("reverb-high-cut", "High Cut", [0x60, 0x00, 0x05, 0x45], maximum: 17);

    public static KatanaParameterDefinition ReverbDensity { get; } =
        new("reverb-density", "Density", [0x60, 0x00, 0x05, 0x46], maximum: 10);

    public static KatanaParameterDefinition ReverbColor { get; } =
        new("reverb-color", "Color", [0x60, 0x00, 0x05, 0x47], maximum: 100);

    public static KatanaParameterDefinition ReverbEffectLevel { get; } =
        new("reverb-effect-level", "E.Level", [0x60, 0x00, 0x05, 0x48], maximum: 100);

    public static KatanaParameterDefinition ReverbDirectMix { get; } =
        new("reverb-direct-mix", "D.Mix", [0x60, 0x00, 0x05, 0x49], maximum: 100);

    public static IReadOnlyList<KatanaPanelEffectDefinition> PanelEffects { get; } =
    [
        new("booster", "Booster", BoosterSwitch, variationParameter: BoosterVariation, typeParameter: BoosterType,
            detailParameters: [BoosterDrive, BoosterTone, BoosterBottom, BoosterSoloSw, BoosterSoloLevel, BoosterEffectLevel, BoosterDirectMix]),
        new("mod", "Mod", ModSwitch, variationParameter: ModVariation, typeParameter: ModType),
        new("fx", "FX", FxSwitch, variationParameter: FxVariation, typeParameter: FxType),
        new("delay", "Delay", DelaySwitch, variationParameter: DelayVariation, typeParameter: DelayType,
            detailParameters: [DelayFeedback, DelayHighCut, DelayEffectLevel, DelayDirectMix, DelayTapTime, DelayModRate, DelayModDepth, DelayRange, DelayFilter, DelayFeedbackPhase, DelayDelayPhase, DelayModSw]),
        new("delay2", "Delay 2", Delay2Switch, typeParameter: Delay2Type,
            detailParameters: [Delay2Feedback, Delay2HighCut, Delay2EffectLevel, Delay2DirectMix, Delay2TapTime, Delay2ModRate, Delay2ModDepth, Delay2Range, Delay2Filter, Delay2FeedbackPhase, Delay2DelayPhase, Delay2ModSw]),
        new("reverb", "Reverb", ReverbSwitch, variationParameter: ReverbVariation, typeParameter: ReverbType,
            detailParameters: [ReverbTime, ReverbPreDelay, ReverbLowCut, ReverbHighCut, ReverbDensity, ReverbColor, ReverbEffectLevel, ReverbDirectMix]),
    ];

    public static KatanaPedalFxDefinition PedalFx { get; } =
        new(
            PedalFxSwitch,
            PedalFxType,
            PedalFxPosition,
            PedalFxWahType,
            PedalFxWahPedalPosition,
            PedalFxWahPedalMinimum,
            PedalFxWahPedalMaximum,
            PedalFxWahEffectLevel,
            PedalFxWahDirectMix,
            PedalFxBendPitch,
            PedalFxBendPedalPosition,
            PedalFxBendEffectLevel,
            PedalFxBendDirectMix,
            PedalFxEvh95Position,
            PedalFxEvh95Minimum,
            PedalFxEvh95Maximum,
            PedalFxEvh95EffectLevel,
            PedalFxEvh95DirectMix,
            FootVolume);

    public static IReadOnlyList<KatanaParameterDefinition> PedalFxReadParameters { get; } =
    [
        PedalFxSwitch,
        PedalFxType,
        PedalFxPosition,
        PedalFxWahType,
        PedalFxWahPedalPosition,
        PedalFxWahPedalMinimum,
        PedalFxWahPedalMaximum,
        PedalFxWahEffectLevel,
        PedalFxWahDirectMix,
        PedalFxBendPitch,
        PedalFxBendPedalPosition,
        PedalFxBendEffectLevel,
        PedalFxBendDirectMix,
        PedalFxEvh95Position,
        PedalFxEvh95Minimum,
        PedalFxEvh95Maximum,
        PedalFxEvh95EffectLevel,
        PedalFxEvh95DirectMix,
        FootVolume,
    ];
}
