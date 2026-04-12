namespace Kataka.Domain.Midi;

public static partial class KatanaMkIIParameterCatalog
{
    public static KatanaParameterDefinition AmpGain { get; } =
        new("amp-gain", "Gain", [0x60, 0x00, 0x06, 0x51]);

    public static KatanaParameterDefinition AmpVolume { get; } =
        new("amp-volume", "Volume", [0x60, 0x00, 0x06, 0x52]);

    public static KatanaParameterDefinition Bass { get; } =
        new("amp-bass", "Bass", [0x60, 0x00, 0x06, 0x53]);

    public static KatanaParameterDefinition Middle { get; } =
        new("amp-middle", "Middle", [0x60, 0x00, 0x06, 0x54]);

    public static KatanaParameterDefinition Treble { get; } =
        new("amp-treble", "Treble", [0x60, 0x00, 0x06, 0x55]);

    public static KatanaParameterDefinition Presence { get; } =
        new("amp-presence", "Presence", [0x60, 0x00, 0x06, 0x56]);

    public static IReadOnlyList<KatanaParameterDefinition> AmpEditorControls { get; } =
    [
        AmpGain,
        AmpVolume,
        Bass,
        Middle,
        Treble,
        Presence,
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

    // Amp type on the front panel (KNOB_POS_TYPE, 0-4: ACOUSTIC/CLEAN/CRUNCH/LEAD/BROWN).
    public static KatanaParameterDefinition AmpType { get; } =
        new("amp-type", "Amp Type", [0x60, 0x00, 0x06, 0x50], maximum: 4);

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

    public static IReadOnlyList<byte> DelayTimeAddress { get; } = [0x60, 0x00, 0x05, 0x02];

    /// <summary>Signal chain pattern (PRM_CHAIN_PTN). Values 0–6 map to CHAIN 1 / CHAIN 2-1 / CHAIN 3-1 / CHAIN 4-1 / CHAIN 2-2 / CHAIN 3-2 / CHAIN 4-2.</summary>
    public static KatanaParameterDefinition ChainPattern { get; } =
        new("chain-pattern", "Chain Pattern", [0x60, 0x00, 0x06, 0x20], maximum: 6);

    /// <summary>Trigger address for saving current temp state to a patch slot (data = [0x00, slot_0based]).</summary>
    public static IReadOnlyList<byte> PatchWriteAddress { get; } = [0x7F, 0x00, 0x01, 0x04];

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
        new("booster", "Booster", BoosterSwitch, variationParameter: BoosterVariation, typeParameter: BoosterType, levelParameter: BoostLevel,
            detailParameters: [BoosterDrive, BoosterTone, BoosterBottom, BoosterSoloSw, BoosterSoloLevel, BoosterEffectLevel, BoosterDirectMix]),
        new("mod", "Mod", ModSwitch, variationParameter: ModVariation, typeParameter: ModType, levelParameter: ModLevel),
        new("fx", "FX", FxSwitch, variationParameter: FxVariation, typeParameter: FxType, levelParameter: FxLevel),
        new("delay", "Delay", DelaySwitch, variationParameter: DelayVariation, typeParameter: DelayType, levelParameter: DelayLevel,
            detailParameters: [DelayFeedback, DelayHighCut, DelayEffectLevel, DelayDirectMix, DelayTapTime, DelayModRate, DelayModDepth, DelayRange, DelayFilter, DelayFeedbackPhase, DelayDelayPhase, DelayModSw]),
        new("delay2", "Delay 2", Delay2Switch, typeParameter: Delay2Type,
            detailParameters: [Delay2Feedback, Delay2HighCut, Delay2EffectLevel, Delay2DirectMix, Delay2TapTime, Delay2ModRate, Delay2ModDepth, Delay2Range, Delay2Filter, Delay2FeedbackPhase, Delay2DelayPhase, Delay2ModSw]),
        new("reverb", "Reverb", ReverbSwitch, variationParameter: ReverbVariation, typeParameter: ReverbType, levelParameter: ReverbLevel,
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
