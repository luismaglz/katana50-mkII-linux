namespace Kataka.Domain.Midi;

/// <summary>Auto-generated: static partial class KatanaMkIIParameterCatalog</summary>
public static partial class KatanaMkIIParameterCatalog
{
    /// <summary> Panel-mode knob positions (PRM_KNOB_POS_*, Status block 0x0650) ────────── </summary>
    // These reflect the physical front-panel knob positions. In PANEL mode the amp
    // drives its sound directly from these values. In CHANNEL mode the stored preamp
    // params (PreampGain / PreampBass etc., see below) govern the sound, but the amp
    // still pushes KNOB_POS updates when knobs are physically moved.
    /// <summary>Parameter definition for Amp Gain.</summary>
    public static KatanaParameterDefinition AmpGain { get; } =
        new("amp-gain", "Gain", [0x60, 0x00, 0x06, 0x51],
            description: "Adjusts the amount of gain/distortion.");

    /// <summary>Parameter definition for Amp Volume.</summary>
    public static KatanaParameterDefinition AmpVolume { get; } =
        new("amp-volume", "Volume", [0x60, 0x00, 0x06, 0x52],
            description: "Adjusts the output volume of the amplifier.");

    /// <summary>Parameter definition for Amp Bass.</summary>
    public static KatanaParameterDefinition AmpBass { get; } =
        new("amp-bass", "Bass", [0x60, 0x00, 0x06, 0x53],
            description: "Adjusts the low frequency tone.");

    /// <summary>Parameter definition for Amp Middle.</summary>
    public static KatanaParameterDefinition AmpMiddle { get; } =
        new("amp-middle", "Middle", [0x60, 0x00, 0x06, 0x54],
            description: "Adjusts the midrange tone.");

    /// <summary>Parameter definition for Amp Treble.</summary>
    public static KatanaParameterDefinition AmpTreble { get; } =
        new("amp-treble", "Treble", [0x60, 0x00, 0x06, 0x55],
            description: "Adjusts the high frequency tone.");

    /// <summary>Parameter definition for Amp Presence.</summary>
    public static KatanaParameterDefinition AmpPresence { get; } =
        new("amp-presence", "Presence", [0x60, 0x00, 0x06, 0x56],
            description: "Adjusts the balance in the extended upper frequency range.");

    /// <summary>Auto-generated: static IReadOnlyList<KatanaParameterDefinition> AmpEditorControls { get; } =</summary>
    public static IReadOnlyList<KatanaParameterDefinition> AmpEditorControls { get; } =
    [
        AmpGain,
        AmpVolume,
        AmpBass,
        AmpMiddle,
        AmpTreble,
        AmpPresence,
    ];

    /// <summary>Parameter definition for Booster Switch.</summary>
    public static KatanaParameterDefinition BoosterSwitch { get; } =
        new("panel-booster-switch", "Booster", [0x60, 0x00, 0x00, 0x10], maximum: 1,
            description: "Turns the Booster effect on/off.");

    /// <summary>Parameter definition for Booster Type.</summary>
    public static KatanaParameterDefinition BoosterType { get; } =
        new("panel-booster-type", "Booster Type", [0x60, 0x00, 0x00, 0x11], maximum: 22,
            skippedValues: [0x07],
            description: "Selects the booster/distortion type (Clean Boost, Blues Drive, Metal Zone, etc.).");

    /// <summary>Parameter definition for Mod Switch.</summary>
    public static KatanaParameterDefinition ModSwitch { get; } =
        new("panel-mod-switch", "Mod", [0x60, 0x00, 0x01, 0x00], maximum: 1,
            description: "Turns the Mod effect on/off.");

    /// <summary>Parameter definition for Mod Type.</summary>
    public static KatanaParameterDefinition ModType { get; } =
        new("panel-mod-type", "Mod Type", [0x60, 0x00, 0x01, 0x01], maximum: 39,
            skippedValues: [0x05, 0x08, 0x0B, 0x0D, 0x11, 0x18, 0x1E, 0x20, 0x21, 0x22],
            description: "Selects the modulation effect type (Chorus, Flanger, Phaser, Tremolo, etc.).");

    /// <summary>Parameter definition for Fx Switch.</summary>
    public static KatanaParameterDefinition FxSwitch { get; } =
        new("panel-fx-switch", "FX", [0x60, 0x00, 0x03, 0x00], maximum: 1,
            description: "Turns the FX effect on/off.");

    /// <summary>Parameter definition for Fx Type.</summary>
    public static KatanaParameterDefinition FxType { get; } =
        new("panel-fx-type", "FX Type", [0x60, 0x00, 0x03, 0x01], maximum: 39,
            skippedValues: [0x05, 0x08, 0x0B, 0x0D, 0x11, 0x18, 0x1E, 0x20, 0x21, 0x22],
            description: "Selects the FX effect type (same options as Mod).");

    /// <summary>Parameter definition for Delay Switch.</summary>
    public static KatanaParameterDefinition DelaySwitch { get; } =
        new("panel-delay-switch", "Delay", [0x60, 0x00, 0x05, 0x00], maximum: 1,
            description: "Turns the Delay effect on/off.");

    /// <summary>Parameter definition for Delay Type.</summary>
    public static KatanaParameterDefinition DelayType { get; } =
        new("panel-delay-type", "Delay Type", [0x60, 0x00, 0x05, 0x01], maximum: 10,
            description:
            "Selects the delay type (Digital, Pan, Stereo, Analog, Tape Echo, Reverse, Modulate, SDE-3000).");

    /// <summary>Parameter definition for Delay 2 Switch.</summary>
    public static KatanaParameterDefinition Delay2Switch { get; } =
        new("panel-delay2-switch", "Delay 2", [0x60, 0x00, 0x05, 0x20], maximum: 1,
            description: "Turns the second Delay effect on/off.");

    /// <summary>Parameter definition for Delay 2 Type.</summary>
    public static KatanaParameterDefinition Delay2Type { get; } =
        new("panel-delay2-type", "Delay 2 Type", [0x60, 0x00, 0x05, 0x21], maximum: 10,
            description: "Selects the type for the second delay (same options as Delay).");

    /// <summary>Parameter definition for Reverb Switch.</summary>
    public static KatanaParameterDefinition ReverbSwitch { get; } =
        new("panel-reverb-switch", "Reverb", [0x60, 0x00, 0x05, 0x40], maximum: 1,
            description: "Turns the Reverb effect on/off.");

    /// <summary>Parameter definition for Reverb Type.</summary>
    public static KatanaParameterDefinition ReverbType { get; } =
        new("panel-reverb-type", "Reverb Type", [0x60, 0x00, 0x05, 0x41], maximum: 6,
            description: "Selects the reverb type (Plate, Room, Hall 1, Spring, Modulate).");

    /// <summary>Parameter definition for Booster Variation.</summary>
    public static KatanaParameterDefinition BoosterVariation { get; } =
        new("panel-booster-variation", "Booster Variation", [0x60, 0x00, 0x06, 0x39], maximum: 2,
            description: "Selects the Booster effect variation (0–2).");

    /// <summary>Parameter definition for Mod Variation.</summary>
    public static KatanaParameterDefinition ModVariation { get; } =
        new("panel-mod-variation", "Mod Variation", [0x60, 0x00, 0x06, 0x3A], maximum: 2,
            description: "Selects the Mod effect variation (0–2).");

    /// <summary>Parameter definition for Fx Variation.</summary>
    public static KatanaParameterDefinition FxVariation { get; } =
        new("panel-fx-variation", "FX Variation", [0x60, 0x00, 0x06, 0x3B], maximum: 2,
            description: "Selects the FX effect variation (0–2).");

    /// <summary>Parameter definition for Delay Variation.</summary>
    public static KatanaParameterDefinition DelayVariation { get; } =
        new("panel-delay-variation", "Delay Variation", [0x60, 0x00, 0x06, 0x3C], maximum: 2,
            description: "Selects the Delay effect variation (0–2).");

    /// <summary>Parameter definition for Reverb Variation.</summary>
    public static KatanaParameterDefinition ReverbVariation { get; } =
        new("panel-reverb-variation", "Reverb Variation", [0x60, 0x00, 0x06, 0x3D], maximum: 2,
            description: "Selects the Reverb effect variation (0–2).");

    // KNOB_POS level controls (0x60000657-65b) — front-panel knob positions for each effect.
    // Raw byte 0x00 means "knob not yet moved" (amp source value −1); treated here as 0.
    /// <summary>Parameter definition for Boost Level.</summary>
    public static KatanaParameterDefinition BoostLevel { get; } =
        new("panel-boost-level", "Boost Level", [0x60, 0x00, 0x06, 0x57],
            description: "Front-panel knob position for the Booster level.");

    /// <summary>Parameter definition for Mod Level.</summary>
    public static KatanaParameterDefinition ModLevel { get; } =
        new("panel-mod-level", "Mod Level", [0x60, 0x00, 0x06, 0x58],
            description: "Front-panel knob position for the Mod level.");

    /// <summary>Parameter definition for Fx Level.</summary>
    public static KatanaParameterDefinition FxLevel { get; } =
        new("panel-fx-level", "FX Level", [0x60, 0x00, 0x06, 0x59],
            description: "Front-panel knob position for the FX level.");

    /// <summary>Parameter definition for Delay Level.</summary>
    public static KatanaParameterDefinition DelayLevel { get; } =
        new("panel-delay-level", "Delay Level", [0x60, 0x00, 0x06, 0x5A],
            description: "Front-panel knob position for the Delay level.");

    /// <summary>Parameter definition for Reverb Level.</summary>
    public static KatanaParameterDefinition ReverbLevel { get; } =
        new("panel-reverb-level", "Reverb Level", [0x60, 0x00, 0x06, 0x5B],
            description: "Front-panel knob position for the Reverb level.");

    // Panel-mode amp type knob position (PRM_KNOB_POS_TYPE, 0-4: ACOUSTIC/CLEAN/CRUNCH/LEAD/BROWN).
    // For channel-mode amp character selection (0-32) use PreampType.
    /// <summary>Parameter definition for Amp Type.</summary>
    public static KatanaParameterDefinition AmpType { get; } =
        new("amp-type", "Amp Type", [0x60, 0x00, 0x06, 0x50], maximum: 4,
            description: "Selects the amp character in panel mode: ACOUSTIC, CLEAN, CRUNCH, LEAD, BROWN.");

    // Amp variation (TYPE 1 / TYPE 2 voicing toggle). PRM_LED_STATE_VARI.
    // PATCH_STATUS block base 0x650, offset 0x0C → 0x65C → 60 00 06 5C.
    /// <summary>Parameter definition for Amp Variation.</summary>
    public static KatanaParameterDefinition AmpVariation { get; } =
        new("amp-variation", "Variation", [0x60, 0x00, 0x06, 0x5C], maximum: 1,
            description: "Toggles between TYPE 1 and TYPE 2 voicing for the selected amp character.");

    // PATCH_STATUS LED states (offsets 0x0D–0x11). Values 0-3 reflect the front-panel LED
    // visual state pushed by the amp when effects or channels change.
    /// <summary>Parameter definition for Led State Booster.</summary>
    public static KatanaParameterDefinition LedStateBooster { get; } =
        new("led-state-booster", "Booster LED", [0x60, 0x00, 0x06, 0x5D], maximum: 3);
    /// <summary>Parameter definition for Led State Mod.</summary>
    public static KatanaParameterDefinition LedStateMod { get; } =
        new("led-state-mod", "Mod LED", [0x60, 0x00, 0x06, 0x5E], maximum: 3);
    /// <summary>Parameter definition for Led State Fx.</summary>
    public static KatanaParameterDefinition LedStateFx { get; } =
        new("led-state-fx", "FX LED", [0x60, 0x00, 0x06, 0x5F], maximum: 3);
    /// <summary>Parameter definition for Led State Delay.</summary>
    public static KatanaParameterDefinition LedStateDelay { get; } =
        new("led-state-delay", "Delay LED", [0x60, 0x00, 0x06, 0x60], maximum: 3);
    /// <summary>Parameter definition for Led State Reverb.</summary>
    public static KatanaParameterDefinition LedStateReverb { get; } =
        new("led-state-reverb", "Reverb LED", [0x60, 0x00, 0x06, 0x61], maximum: 3);

    // Cabinet resonance (0-2: LOW / MIDDLE / HIGH).
    /// <summary>Parameter definition for Cabinet Resonance.</summary>
    public static KatanaParameterDefinition CabinetResonance { get; } =
        new("amp-cabinet-resonance", "Cabinet", [0x60, 0x00, 0x06, 0x43], maximum: 2,
            description: "Selects the cabinet resonance character: LOW, MIDDLE, or HIGH.");

    /// <summary>Parameter definition for Pedal Fx Switch.</summary>
    public static KatanaParameterDefinition PedalFxSwitch { get; } =
        new("pedal-fx-switch", "Pedal FX", [0x60, 0x00, 0x05, 0x50], maximum: 1,
            description: "Turns the Pedal FX (expression pedal effect) on/off.");

    /// <summary>Parameter definition for Pedal Fx Type.</summary>
    public static KatanaParameterDefinition PedalFxType { get; } =
        new("pedal-fx-type", "Pedal FX Type", [0x60, 0x00, 0x05, 0x51], maximum: 2,
            description: "Selects the pedal FX type: WAH, BEND, or EVH95.");

    /// <summary>Parameter definition for Pedal Fx Position.</summary>
    public static KatanaParameterDefinition PedalFxPosition { get; } =
        new("pedal-fx-position", "Pedal FX Position", [0x60, 0x00, 0x06, 0x23], maximum: 1,
            description: "Sets the position of the Pedal FX in the signal chain.");

    /// <summary>Parameter definition for Pedal Fx Wah Type.</summary>
    public static KatanaParameterDefinition PedalFxWahType { get; } =
        new("pedal-fx-wah-type", "Wah Type", [0x60, 0x00, 0x05, 0x52], maximum: 5,
            description: "Selects the wah pedal type: CRY WAH, VO WAH, FAT WAH, LIGHT WAH, 7STRING WAH, RESO WAH.");

    /// <summary>Parameter definition for Pedal Fx Wah Pedal Position.</summary>
    public static KatanaParameterDefinition PedalFxWahPedalPosition { get; } =
        new("pedal-fx-wah-position", "Wah Pedal Position", [0x60, 0x00, 0x05, 0x53],
            description: "Current position of the wah pedal (used after assigning to an EXP pedal).");

    /// <summary>Parameter definition for Pedal Fx Wah Pedal Minimum.</summary>
    public static KatanaParameterDefinition PedalFxWahPedalMinimum { get; } =
        new("pedal-fx-wah-min", "Wah Pedal Minimum", [0x60, 0x00, 0x05, 0x54],
            description: "Selects the tone produced when the heel of the EXP pedal is depressed.");

    /// <summary>Parameter definition for Pedal Fx Wah Pedal Maximum.</summary>
    public static KatanaParameterDefinition PedalFxWahPedalMaximum { get; } =
        new("pedal-fx-wah-max", "Wah Pedal Maximum", [0x60, 0x00, 0x05, 0x55],
            description: "Selects the tone produced when the toe of the EXP pedal is depressed.");

    /// <summary>Parameter definition for Pedal Fx Wah Effect Level.</summary>
    public static KatanaParameterDefinition PedalFxWahEffectLevel { get; } =
        new("pedal-fx-wah-effect-level", "Wah Effect Level", [0x60, 0x00, 0x05, 0x56],
            description: "Adjusts the volume of the wah effect sound.");

    /// <summary>Parameter definition for Pedal Fx Wah Direct Mix.</summary>
    public static KatanaParameterDefinition PedalFxWahDirectMix { get; } =
        new("pedal-fx-wah-direct-mix", "Wah Direct Mix", [0x60, 0x00, 0x05, 0x57],
            description: "Adjusts the volume of the direct (dry) sound.");

    /// <summary>Parameter definition for Pedal Fx Bend Pitch.</summary>
    public static KatanaParameterDefinition PedalFxBendPitch { get; } =
        new("pedal-fx-bend-pitch", "Pitch", [0x60, 0x00, 0x05, 0x58], 0, 48,
            description: "Sets the pitch at the point where the pedal is all the way down (in semitones, −24 to +24).");

    /// <summary>Parameter definition for Pedal Fx Bend Pedal Position.</summary>
    public static KatanaParameterDefinition PedalFxBendPedalPosition { get; } =
        new("pedal-fx-bend-position", "Pedal Pos", [0x60, 0x00, 0x05, 0x59],
            description: "Current position of the pedal bend pedal.");

    /// <summary>Parameter definition for Pedal Fx Bend Effect Level.</summary>
    public static KatanaParameterDefinition PedalFxBendEffectLevel { get; } =
        new("pedal-fx-bend-effect-level", "Effect Level", [0x60, 0x00, 0x05, 0x5A],
            description: "Adjusts the volume of the pedal bend effect sound.");

    /// <summary>Parameter definition for Pedal Fx Bend Direct Mix.</summary>
    public static KatanaParameterDefinition PedalFxBendDirectMix { get; } =
        new("pedal-fx-bend-direct-mix", "Direct Mix", [0x60, 0x00, 0x05, 0x5B],
            description: "Adjusts the volume of the direct sound.");

    /// <summary>Parameter definition for Pedal Fx Evh95 Position.</summary>
    public static KatanaParameterDefinition PedalFxEvh95Position { get; } =
        new("pedal-fx-evh95-position", "Pedal Pos", [0x60, 0x00, 0x05, 0x5C],
            description: "Current position of the EVH95 wah pedal.");

    /// <summary>Parameter definition for Pedal Fx Evh95 Minimum.</summary>
    public static KatanaParameterDefinition PedalFxEvh95Minimum { get; } =
        new("pedal-fx-evh95-min", "Pedal Min", [0x60, 0x00, 0x05, 0x5D],
            description: "Tone produced when the heel of the EXP pedal is depressed.");

    /// <summary>Parameter definition for Pedal Fx Evh95 Maximum.</summary>
    public static KatanaParameterDefinition PedalFxEvh95Maximum { get; } =
        new("pedal-fx-evh95-max", "Pedal Max", [0x60, 0x00, 0x05, 0x5E],
            description: "Tone produced when the toe of the EXP pedal is depressed.");

    /// <summary>Parameter definition for Pedal Fx Evh95 Effect Level.</summary>
    public static KatanaParameterDefinition PedalFxEvh95EffectLevel { get; } =
        new("pedal-fx-evh95-effect-level", "Effect Level", [0x60, 0x00, 0x05, 0x5F],
            description: "Adjusts the volume of the EVH95 effect sound.");

    /// <summary>Parameter definition for Pedal Fx Evh95 Direct Mix.</summary>
    public static KatanaParameterDefinition PedalFxEvh95DirectMix { get; } =
        new("pedal-fx-evh95-direct-mix", "Direct Mix", [0x60, 0x00, 0x05, 0x60],
            description: "Adjusts the volume of the direct sound.");

    /// <summary>Parameter definition for Foot Volume.</summary>
    public static KatanaParameterDefinition FootVolume { get; } =
        new("pedal-fx-foot-volume", "Foot Volume", [0x60, 0x00, 0x05, 0x61],
            description: "Controls overall volume via the expression pedal.");

    /// <summary>Parameter definition for Patch Level.</summary>
    public static KatanaParameterDefinition PatchLevel { get; } =
        new("panel-patch-level", "Patch Level", [0x60, 0x00, 0x06, 0x4C], maximum: 200,
            description: "Adjusts the overall output level of the patch.");

    /// <summary> Solo EQ (Ver200+, Mk2V2 block, patch offset 0xF10) ────────────────────── </summary>
    public static KatanaParameterDefinition SoloEqPosition { get; } =
        new("solo-eq-position", "Solo EQ Position", [0x60, 0x00, 0x0F, 0x10], maximum: 1,
            description: "Positions the Solo EQ before (AMP IN) or after (AMP OUT) the preamp in the effect chain.");

    /// <summary>Parameter definition for Solo Eq Sw.</summary>
    public static KatanaParameterDefinition SoloEqSw { get; } =
        new("solo-eq-sw", "Solo EQ", [0x60, 0x00, 0x0F, 0x11], maximum: 1,
            description: "Turns the Solo EQ on/off when the solo function is active.");

    /// <summary>Parameter definition for Solo Eq Low Cut.</summary>
    public static KatanaParameterDefinition SoloEqLowCut { get; } =
        new("solo-eq-low-cut", "Solo EQ Low Cut", [0x60, 0x00, 0x0F, 0x12], maximum: 17,
            description: "Sets the frequency at which the low cut filter begins to take effect (FLAT or 20–800 Hz).");

    /// <summary>Parameter definition for Solo Eq Low Gain.</summary>
    public static KatanaParameterDefinition SoloEqLowGain { get; } =
        new("solo-eq-low-gain", "Solo EQ Low", [0x60, 0x00, 0x0F, 0x13], 0, 48,
            description: "Adjusts the low frequency range tone (−12 to +12 dB).");

    /// <summary>Parameter definition for Solo Eq Mid Freq.</summary>
    public static KatanaParameterDefinition SoloEqMidFreq { get; } =
        new("solo-eq-mid-freq", "Solo EQ Mid Freq", [0x60, 0x00, 0x0F, 0x14], maximum: 27,
            description: "Specifies the center frequency adjusted by MID GAIN (20 Hz–10.0 kHz).");

    /// <summary>Parameter definition for Solo Eq Mid Q.</summary>
    public static KatanaParameterDefinition SoloEqMidQ { get; } =
        new("solo-eq-mid-q", "Solo EQ Mid Q", [0x60, 0x00, 0x0F, 0x15], maximum: 5,
            description: "Adjusts the bandwidth of the mid-frequency band. Higher values narrow the affected area.");

    /// <summary>Parameter definition for Solo Eq Mid Gain.</summary>
    public static KatanaParameterDefinition SoloEqMidGain { get; } =
        new("solo-eq-mid-gain", "Solo EQ Mid", [0x60, 0x00, 0x0F, 0x16], 0, 48,
            description: "Adjusts the middle frequency range tone (−12 to +12 dB).");

    /// <summary>Parameter definition for Solo Eq High Gain.</summary>
    public static KatanaParameterDefinition SoloEqHighGain { get; } =
        new("solo-eq-high-gain", "Solo EQ High", [0x60, 0x00, 0x0F, 0x17], 0, 48,
            description: "Adjusts the high frequency range tone (−12 to +12 dB).");

    /// <summary>Parameter definition for Solo Eq High Cut.</summary>
    public static KatanaParameterDefinition SoloEqHighCut { get; } =
        new("solo-eq-high-cut", "Solo EQ High Cut", [0x60, 0x00, 0x0F, 0x18], maximum: 14,
            description: "Sets the frequency at which the high cut filter begins to take effect (630 Hz–FLAT).");

    /// <summary>Parameter definition for Solo Eq Level.</summary>
    public static KatanaParameterDefinition SoloEqLevel { get; } =
        new("solo-eq-level", "Solo EQ Level", [0x60, 0x00, 0x0F, 0x19], 0, 48,
            description: "Adjusts the overall volume level of the Solo EQ (−12 to +12 dB).");

    /// <summary> Solo Delay (Ver210+, same Mk2V2 block) ─────────────────────────────────── </summary>
    public static KatanaParameterDefinition SoloDelaySw { get; } =
        new("solo-delay-sw", "Solo Delay", [0x60, 0x00, 0x0F, 0x1A], maximum: 1,
            description: "Turns the Solo Delay on/off when the solo function is active.");

    /// <summary>Parameter definition for Solo Delay Carryover.</summary>
    public static KatanaParameterDefinition SoloDelayCarryover { get; } =
        new("solo-delay-carryover", "Solo Dly Carry", [0x60, 0x00, 0x0F, 0x1B], maximum: 1,
            description: "When ON, the delay sound continues after switching sounds or turning off solo.");

    // Solo Delay Time is INTEGER2x7 (2 bytes); address of first byte.
    /// <summary>Auto-generated: static IReadOnlyList<byte> SoloDelayTimeAddress { get; } = [0x60, 0x00, 0x0F,...</summary>
    public static IReadOnlyList<byte> SoloDelayTimeAddress { get; } = [0x60, 0x00, 0x0F, 0x1C];

    /// <summary>Parameter definition for Solo Delay Feedback.</summary>
    public static KatanaParameterDefinition SoloDelayFeedback { get; } =
        new("solo-delay-feedback", "Solo Dly Feedback", [0x60, 0x00, 0x0F, 0x1E], maximum: 100,
            description:
            "Adjusts the volume returned to the input; higher values increase the number of delay repeats.");

    /// <summary>Parameter definition for Solo Delay Effect Level.</summary>
    public static KatanaParameterDefinition SoloDelayEffectLevel { get; } =
        new("solo-delay-effect-level", "Solo Dly Level", [0x60, 0x00, 0x0F, 0x1F], maximum: 120,
            description: "Adjusts the volume of the Solo Delay effect sound.");

    /// <summary>Parameter definition for Solo Delay Direct Level.</summary>
    public static KatanaParameterDefinition SoloDelayDirectLevel { get; } =
        new("solo-delay-direct-level", "Solo Dly Direct", [0x60, 0x00, 0x0F, 0x20], maximum: 100,
            description: "Adjusts the volume of the direct (dry) sound.");

    /// <summary>Parameter definition for Solo Delay Filter.</summary>
    public static KatanaParameterDefinition SoloDelayFilter { get; } =
        new("solo-delay-filter", "Solo Dly Filter", [0x60, 0x00, 0x0F, 0x21], maximum: 2,
            description: "Switches filter settings: OFF, ANALOG (natural analog-like tone), or TAPE ECHO.");

    /// <summary>Parameter definition for Solo Delay High Cut.</summary>
    public static KatanaParameterDefinition SoloDelayHighCut { get; } =
        new("solo-delay-high-cut", "Solo Dly Hi Cut", [0x60, 0x00, 0x0F, 0x22], maximum: 14,
            description:
            "Sets the frequency at which the high cut filter begins to take effect (630 Hz–12.5 kHz, FLAT).");

    /// <summary>Parameter definition for Solo Delay Mod Sw.</summary>
    public static KatanaParameterDefinition SoloDelayModSw { get; } =
        new("solo-delay-mod-sw", "Solo Dly Mod", [0x60, 0x00, 0x0F, 0x23], maximum: 1,
            description: "Turns the delay modulation on/off.");

    /// <summary>Parameter definition for Solo Delay Mod Rate.</summary>
    public static KatanaParameterDefinition SoloDelayModRate { get; } =
        new("solo-delay-mod-rate", "Solo Dly Mod Rate", [0x60, 0x00, 0x0F, 0x24], maximum: 100,
            description: "Adjusts the modulation rate of the Solo Delay sound.");

    /// <summary>Parameter definition for Solo Delay Mod Depth.</summary>
    public static KatanaParameterDefinition SoloDelayModDepth { get; } =
        new("solo-delay-mod-depth", "Solo Dly Mod Depth", [0x60, 0x00, 0x0F, 0x25], maximum: 100,
            description: "Adjusts the modulation depth of the Solo Delay sound.");

    /// <summary> Contour (Ver200+, patch offsets 0xF30/0xF38/0xF40) ────────────────────── </summary>
    // Three contour blocks share the same parameter structure (prm_prop_contour).
    // Contour(1) = 0xF30, Contour(2) = 0xF38, Contour(3) = 0xF40.
    /// <summary>Parameter definition for Contour 1 Type.</summary>
    public static KatanaParameterDefinition Contour1Type { get; } =
        new("contour1-type", "Contour 1 Type", [0x60, 0x00, 0x0F, 0x30], maximum: 3,
            description: "Selects the contour type (tone definition characteristics). Can also be set to custom.");

    /// <summary>Parameter definition for Contour 1 Freq Shift.</summary>
    public static KatanaParameterDefinition Contour1FreqShift { get; } =
        new("contour1-freq-shift", "Contour 1 Freq", [0x60, 0x00, 0x0F, 0x31], 0, 100,
            description: "Adjusts the frequency shift amount for Contour 1.");

    /// <summary>Parameter definition for Contour 2 Type.</summary>
    public static KatanaParameterDefinition Contour2Type { get; } =
        new("contour2-type", "Contour 2 Type", [0x60, 0x00, 0x0F, 0x38], maximum: 3,
            description: "Selects the contour type for Contour 2.");

    /// <summary>Parameter definition for Contour 2 Freq Shift.</summary>
    public static KatanaParameterDefinition Contour2FreqShift { get; } =
        new("contour2-freq-shift", "Contour 2 Freq", [0x60, 0x00, 0x0F, 0x39], 0, 100,
            description: "Adjusts the frequency shift amount for Contour 2.");

    /// <summary>Parameter definition for Contour 3 Type.</summary>
    public static KatanaParameterDefinition Contour3Type { get; } =
        new("contour3-type", "Contour 3 Type", [0x60, 0x00, 0x0F, 0x40], maximum: 3,
            description: "Selects the contour type for Contour 3.");

    /// <summary>Parameter definition for Contour 3 Freq Shift.</summary>
    public static KatanaParameterDefinition Contour3FreqShift { get; } =
        new("contour3-freq-shift", "Contour 3 Freq", [0x60, 0x00, 0x0F, 0x41], 0, 100,
            description: "Adjusts the frequency shift amount for Contour 3.");

    /// <summary> Patch EQ — Patch_0 embedded EQ block (BTS prm_prop_patch_0 addrs 0x30–0x47) ── </summary>
    // Absolute addresses: Patch_0 base (0x10) + relative (0x30–0x47) = 0x40–0x57.
    // Two EQ blocks exist per patch: Eq(1) at 0x40 and Eq(2) at 0x60.
    /// <summary>Parameter definition for Patch Eq 1 Sw.</summary>
    public static KatanaParameterDefinition PatchEq1Sw { get; } =
        new("patch-eq1-sw", "EQ On/Off", [0x60, 0x00, 0x00, 0x40], maximum: 1,
            description: "Turns the Patch EQ on/off.");

    /// <summary>Parameter definition for Patch Eq 1 Type.</summary>
    public static KatanaParameterDefinition PatchEq1Type { get; } =
        new("patch-eq1-type", "EQ Type", [0x60, 0x00, 0x00, 0x41], maximum: 1,
            description: "Selects the EQ type: PARAMETRIC EQ or GE-10 (graphic equalizer).");

    /// <summary>Parameter definition for Patch Eq 1 Low Cut.</summary>
    public static KatanaParameterDefinition PatchEq1LowCut { get; } =
        new("patch-eq1-low-cut", "Low Cut", [0x60, 0x00, 0x00, 0x42], maximum: 17,
            description: "Sets the frequency at which the low cut filter begins to take effect (FLAT or 20–800 Hz).");

    /// <summary>Parameter definition for Patch Eq 1 Low Gain.</summary>
    public static KatanaParameterDefinition PatchEq1LowGain { get; } =
        new("patch-eq1-low-gain", "Low Gain", [0x60, 0x00, 0x00, 0x43], maximum: 40,
            description: "Adjusts the low frequency range tone (−20 to +20 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Low Mid Freq.</summary>
    public static KatanaParameterDefinition PatchEq1LowMidFreq { get; } =
        new("patch-eq1-lomid-freq", "Low-Mid Freq", [0x60, 0x00, 0x00, 0x44], maximum: 27,
            description: "Specifies the center frequency adjusted by the Low-Mid Gain (20 Hz–10.0 kHz).");

    /// <summary>Parameter definition for Patch Eq 1 Low Mid Q.</summary>
    public static KatanaParameterDefinition PatchEq1LowMidQ { get; } =
        new("patch-eq1-lomid-q", "Low-Mid Q", [0x60, 0x00, 0x00, 0x45], maximum: 5,
            description: "Adjusts the bandwidth of the low-mid band. Higher values narrow the affected area.");

    /// <summary>Parameter definition for Patch Eq 1 Low Mid Gain.</summary>
    public static KatanaParameterDefinition PatchEq1LowMidGain { get; } =
        new("patch-eq1-lomid-gain", "Low-Mid Gain", [0x60, 0x00, 0x00, 0x46], maximum: 40,
            description: "Adjusts the low-middle frequency range tone (−20 to +20 dB).");

    /// <summary>Parameter definition for Patch Eq 1 High Mid Freq.</summary>
    public static KatanaParameterDefinition PatchEq1HighMidFreq { get; } =
        new("patch-eq1-himid-freq", "High-Mid Freq", [0x60, 0x00, 0x00, 0x47], maximum: 27,
            description: "Specifies the center frequency adjusted by the High-Mid Gain (20 Hz–10.0 kHz).");

    /// <summary>Parameter definition for Patch Eq 1 High Mid Q.</summary>
    public static KatanaParameterDefinition PatchEq1HighMidQ { get; } =
        new("patch-eq1-himid-q", "High-Mid Q", [0x60, 0x00, 0x00, 0x48], maximum: 5,
            description: "Adjusts the bandwidth of the high-mid band. Higher values narrow the affected area.");

    /// <summary>Parameter definition for Patch Eq 1 High Mid Gain.</summary>
    public static KatanaParameterDefinition PatchEq1HighMidGain { get; } =
        new("patch-eq1-himid-gain", "High-Mid Gain", [0x60, 0x00, 0x00, 0x49], maximum: 40,
            description: "Adjusts the high-middle frequency range tone (−20 to +20 dB).");

    /// <summary>Parameter definition for Patch Eq 1 High Gain.</summary>
    public static KatanaParameterDefinition PatchEq1HighGain { get; } =
        new("patch-eq1-high-gain", "High Gain", [0x60, 0x00, 0x00, 0x4A], maximum: 40,
            description: "Adjusts the high frequency range tone (−20 to +20 dB).");

    /// <summary>Parameter definition for Patch Eq 1 High Cut.</summary>
    public static KatanaParameterDefinition PatchEq1HighCut { get; } =
        new("patch-eq1-high-cut", "High Cut", [0x60, 0x00, 0x00, 0x4B], maximum: 14,
            description:
            "Sets the frequency at which the high cut filter begins to take effect (630 Hz–12.5 kHz, FLAT).");

    /// <summary>Parameter definition for Patch Eq 1 Level.</summary>
    public static KatanaParameterDefinition PatchEq1Level { get; } =
        new("patch-eq1-level", "EQ Level", [0x60, 0x00, 0x00, 0x4C], maximum: 40,
            description: "Adjusts the overall volume level of the EQ (−20 to +20 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq 3 1 Hz.</summary>
    public static KatanaParameterDefinition PatchEq1Geq31Hz { get; } =
        new("patch-eq1-geq-31hz", "31 Hz", [0x60, 0x00, 0x00, 0x4D], maximum: 48,
            description: "GE-10 graphic EQ band at 31 Hz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq6 2 Hz.</summary>
    public static KatanaParameterDefinition PatchEq1Geq62Hz { get; } =
        new("patch-eq1-geq-62hz", "62 Hz", [0x60, 0x00, 0x00, 0x4E], maximum: 48,
            description: "GE-10 graphic EQ band at 62 Hz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq 1 25 Hz.</summary>
    public static KatanaParameterDefinition PatchEq1Geq125Hz { get; } =
        new("patch-eq1-geq-125hz", "125 Hz", [0x60, 0x00, 0x00, 0x4F], maximum: 48,
            description: "GE-10 graphic EQ band at 125 Hz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq 250 Hz.</summary>
    public static KatanaParameterDefinition PatchEq1Geq250Hz { get; } =
        new("patch-eq1-geq-250hz", "250 Hz", [0x60, 0x00, 0x00, 0x50], maximum: 48,
            description: "GE-10 graphic EQ band at 250 Hz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq500 Hz.</summary>
    public static KatanaParameterDefinition PatchEq1Geq500Hz { get; } =
        new("patch-eq1-geq-500hz", "500 Hz", [0x60, 0x00, 0x00, 0x51], maximum: 48,
            description: "GE-10 graphic EQ band at 500 Hz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq 1k Hz.</summary>
    public static KatanaParameterDefinition PatchEq1Geq1kHz { get; } =
        new("patch-eq1-geq-1khz", "1 kHz", [0x60, 0x00, 0x00, 0x52], maximum: 48,
            description: "GE-10 graphic EQ band at 1 kHz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq 2k Hz.</summary>
    public static KatanaParameterDefinition PatchEq1Geq2kHz { get; } =
        new("patch-eq1-geq-2khz", "2 kHz", [0x60, 0x00, 0x00, 0x53], maximum: 48,
            description: "GE-10 graphic EQ band at 2 kHz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq4k Hz.</summary>
    public static KatanaParameterDefinition PatchEq1Geq4kHz { get; } =
        new("patch-eq1-geq-4khz", "4 kHz", [0x60, 0x00, 0x00, 0x54], maximum: 48,
            description: "GE-10 graphic EQ band at 4 kHz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq8k Hz.</summary>
    public static KatanaParameterDefinition PatchEq1Geq8kHz { get; } =
        new("patch-eq1-geq-8khz", "8 kHz", [0x60, 0x00, 0x00, 0x55], maximum: 48,
            description: "GE-10 graphic EQ band at 8 kHz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq 16k Hz.</summary>
    public static KatanaParameterDefinition PatchEq1Geq16kHz { get; } =
        new("patch-eq1-geq-16khz", "16 kHz", [0x60, 0x00, 0x00, 0x56], maximum: 48,
            description: "GE-10 graphic EQ band at 16 kHz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 1 Geq Level.</summary>
    public static KatanaParameterDefinition PatchEq1GeqLevel { get; } =
        new("patch-eq1-geq-level", "GEQ Level", [0x60, 0x00, 0x00, 0x57], maximum: 48,
            description: "Overall output level of the GE-10 graphic EQ (−12 to +12 dB).");

    /// <summary> Patch Eq(2) — second EQ block (BTS prm_prop_patch_eq2 at 0x0060) ────────── </summary>
    public static KatanaParameterDefinition PatchEq2Sw { get; } =
        new("patch-eq2-sw", "EQ 2 On/Off", [0x60, 0x00, 0x00, 0x60], maximum: 1,
            description: "Turns the second Patch EQ on/off.");

    /// <summary>Parameter definition for Patch Eq 2 Type.</summary>
    public static KatanaParameterDefinition PatchEq2Type { get; } =
        new("patch-eq2-type", "EQ 2 Type", [0x60, 0x00, 0x00, 0x61], maximum: 1,
            description: "Selects the EQ 2 type: PARAMETRIC EQ or GE-10 (graphic equalizer).");

    /// <summary>Parameter definition for Patch Eq 2 Low Cut.</summary>
    public static KatanaParameterDefinition PatchEq2LowCut { get; } =
        new("patch-eq2-low-cut", "EQ 2 Low Cut", [0x60, 0x00, 0x00, 0x62], maximum: 17,
            description: "Sets the frequency at which the low cut filter begins to take effect (FLAT or 20–800 Hz).");

    /// <summary>Parameter definition for Patch Eq 2 Low Gain.</summary>
    public static KatanaParameterDefinition PatchEq2LowGain { get; } =
        new("patch-eq2-low-gain", "EQ 2 Low Gain", [0x60, 0x00, 0x00, 0x63], maximum: 40,
            description: "Adjusts the low frequency range tone (−20 to +20 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Low Mid Freq.</summary>
    public static KatanaParameterDefinition PatchEq2LowMidFreq { get; } =
        new("patch-eq2-lomid-freq", "EQ 2 Low-Mid Freq", [0x60, 0x00, 0x00, 0x64], maximum: 27,
            description: "Specifies the center frequency adjusted by the Low-Mid Gain (20 Hz–10.0 kHz).");

    /// <summary>Parameter definition for Patch Eq 2 Low Mid Q.</summary>
    public static KatanaParameterDefinition PatchEq2LowMidQ { get; } =
        new("patch-eq2-lomid-q", "EQ 2 Low-Mid Q", [0x60, 0x00, 0x00, 0x65], maximum: 5,
            description: "Adjusts the bandwidth of the low-mid band. Higher values narrow the affected area.");

    /// <summary>Parameter definition for Patch Eq 2 Low Mid Gain.</summary>
    public static KatanaParameterDefinition PatchEq2LowMidGain { get; } =
        new("patch-eq2-lomid-gain", "EQ 2 Low-Mid Gain", [0x60, 0x00, 0x00, 0x66], maximum: 40,
            description: "Adjusts the low-middle frequency range tone (−20 to +20 dB).");

    /// <summary>Parameter definition for Patch Eq 2 High Mid Freq.</summary>
    public static KatanaParameterDefinition PatchEq2HighMidFreq { get; } =
        new("patch-eq2-himid-freq", "EQ 2 High-Mid Freq", [0x60, 0x00, 0x00, 0x67], maximum: 27,
            description: "Specifies the center frequency adjusted by the High-Mid Gain (20 Hz–10.0 kHz).");

    /// <summary>Parameter definition for Patch Eq 2 High Mid Q.</summary>
    public static KatanaParameterDefinition PatchEq2HighMidQ { get; } =
        new("patch-eq2-himid-q", "EQ 2 High-Mid Q", [0x60, 0x00, 0x00, 0x68], maximum: 5,
            description: "Adjusts the bandwidth of the high-mid band. Higher values narrow the affected area.");

    /// <summary>Parameter definition for Patch Eq 2 High Mid Gain.</summary>
    public static KatanaParameterDefinition PatchEq2HighMidGain { get; } =
        new("patch-eq2-himid-gain", "EQ 2 High-Mid Gain", [0x60, 0x00, 0x00, 0x69], maximum: 40,
            description: "Adjusts the high-middle frequency range tone (−20 to +20 dB).");

    /// <summary>Parameter definition for Patch Eq 2 High Gain.</summary>
    public static KatanaParameterDefinition PatchEq2HighGain { get; } =
        new("patch-eq2-high-gain", "EQ 2 High Gain", [0x60, 0x00, 0x00, 0x6A], maximum: 40,
            description: "Adjusts the high frequency range tone (−20 to +20 dB).");

    /// <summary>Parameter definition for Patch Eq 2 High Cut.</summary>
    public static KatanaParameterDefinition PatchEq2HighCut { get; } =
        new("patch-eq2-high-cut", "EQ 2 High Cut", [0x60, 0x00, 0x00, 0x6B], maximum: 14,
            description:
            "Sets the frequency at which the high cut filter begins to take effect (630 Hz–12.5 kHz, FLAT).");

    /// <summary>Parameter definition for Patch Eq 2 Level.</summary>
    public static KatanaParameterDefinition PatchEq2Level { get; } =
        new("patch-eq2-level", "EQ 2 Level", [0x60, 0x00, 0x00, 0x6C], maximum: 40,
            description: "Adjusts the overall volume level of EQ 2 (−20 to +20 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq 3 1 Hz.</summary>
    public static KatanaParameterDefinition PatchEq2Geq31Hz { get; } =
        new("patch-eq2-geq-31hz", "EQ 2 31 Hz", [0x60, 0x00, 0x00, 0x6D], maximum: 48,
            description: "GE-10 graphic EQ 2 band at 31 Hz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq6 2 Hz.</summary>
    public static KatanaParameterDefinition PatchEq2Geq62Hz { get; } =
        new("patch-eq2-geq-62hz", "EQ 2 62 Hz", [0x60, 0x00, 0x00, 0x6E], maximum: 48,
            description: "GE-10 graphic EQ 2 band at 62 Hz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq 1 25 Hz.</summary>
    public static KatanaParameterDefinition PatchEq2Geq125Hz { get; } =
        new("patch-eq2-geq-125hz", "EQ 2 125 Hz", [0x60, 0x00, 0x00, 0x6F], maximum: 48,
            description: "GE-10 graphic EQ 2 band at 125 Hz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq 250 Hz.</summary>
    public static KatanaParameterDefinition PatchEq2Geq250Hz { get; } =
        new("patch-eq2-geq-250hz", "EQ 2 250 Hz", [0x60, 0x00, 0x00, 0x70], maximum: 48,
            description: "GE-10 graphic EQ 2 band at 250 Hz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq500 Hz.</summary>
    public static KatanaParameterDefinition PatchEq2Geq500Hz { get; } =
        new("patch-eq2-geq-500hz", "EQ 2 500 Hz", [0x60, 0x00, 0x00, 0x71], maximum: 48,
            description: "GE-10 graphic EQ 2 band at 500 Hz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq 1k Hz.</summary>
    public static KatanaParameterDefinition PatchEq2Geq1kHz { get; } =
        new("patch-eq2-geq-1khz", "EQ 2 1 kHz", [0x60, 0x00, 0x00, 0x72], maximum: 48,
            description: "GE-10 graphic EQ 2 band at 1 kHz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq 2k Hz.</summary>
    public static KatanaParameterDefinition PatchEq2Geq2kHz { get; } =
        new("patch-eq2-geq-2khz", "EQ 2 2 kHz", [0x60, 0x00, 0x00, 0x73], maximum: 48,
            description: "GE-10 graphic EQ 2 band at 2 kHz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq4k Hz.</summary>
    public static KatanaParameterDefinition PatchEq2Geq4kHz { get; } =
        new("patch-eq2-geq-4khz", "EQ 2 4 kHz", [0x60, 0x00, 0x00, 0x74], maximum: 48,
            description: "GE-10 graphic EQ 2 band at 4 kHz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq8k Hz.</summary>
    public static KatanaParameterDefinition PatchEq2Geq8kHz { get; } =
        new("patch-eq2-geq-8khz", "EQ 2 8 kHz", [0x60, 0x00, 0x00, 0x75], maximum: 48,
            description: "GE-10 graphic EQ 2 band at 8 kHz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq 16k Hz.</summary>
    public static KatanaParameterDefinition PatchEq2Geq16kHz { get; } =
        new("patch-eq2-geq-16khz", "EQ 2 16 kHz", [0x60, 0x00, 0x00, 0x76], maximum: 48,
            description: "GE-10 graphic EQ 2 band at 16 kHz (−12 to +12 dB).");

    /// <summary>Parameter definition for Patch Eq 2 Geq Level.</summary>
    public static KatanaParameterDefinition PatchEq2GeqLevel { get; } =
        new("patch-eq2-geq-level", "EQ 2 GEQ Level", [0x60, 0x00, 0x00, 0x77], maximum: 48,
            description: "Overall output level of the GE-10 graphic EQ 2 (−12 to +12 dB).");

    /// <summary>Auto-generated: static IReadOnlyList<byte> DelayTimeAddress { get; } = [0x60, 0x00, 0x05, 0x02];</summary>
    public static IReadOnlyList<byte> DelayTimeAddress { get; } = [0x60, 0x00, 0x05, 0x02];

    /// <summary>
    ///     Signal chain pattern (PRM_CHAIN_PTN). Values 0–6 map to CHAIN 1 / CHAIN 2-1 / CHAIN 3-1 / CHAIN 4-1 / CHAIN
    ///     2-2 / CHAIN 3-2 / CHAIN 4-2.
    /// </summary>
    public static KatanaParameterDefinition ChainPattern { get; } =
        new("chain-pattern", "Chain Pattern", [0x60, 0x00, 0x06, 0x20], maximum: 6,
            description: "Selects the signal chain routing pattern: CHAIN 1, 2-1, 3-1, 4-1, 2-2, 3-2, or 4-2.");

    /// <summary>Trigger address for saving current temp state to a patch slot (data = [0x00, slot_0based]).</summary>
    public static IReadOnlyList<byte> PatchWriteAddress { get; } = [0x7F, 0x00, 0x01, 0x04];

    /// <summary>
    /// Address pushed by the amp when the active panel/channel changes (DT1 SysEx).
    /// Value encoding: 0=Panel, 1=ChA1, 2=ChA2, 5=ChB1, 6=ChB2.
    /// </summary>
    public static IReadOnlyList<byte> CurrentChannelAddress { get; } = [0x00, 0x01, 0x00, 0x00];

    /// <summary> Channel-mode (stored) preamp parameters ───────────────────────────────── </summary>
    // In PANEL mode the amp uses knob-position values from the Status block (0x0651+).
    // In CHANNEL mode the amp reads these stored preamp values from Patch_0 (base 0x10).
    // BTS block: prm_prop_patch_0, Temporary patch offset 0x0010 + field offset.

    /// <summary>Amp character type (PRM_PREAMP_A_TYPE). 0-32 covers all BTS amp characters.</summary>
    public static KatanaParameterDefinition PreampType { get; } =
        new("preamp-type", "Preamp Type", [0x60, 0x00, 0x00, 0x21], maximum: 32,
            description: "Selects the amp character in channel mode (0–32 covers all BTS amp characters).");

    /// <summary>Gain in channel mode (PRM_PREAMP_A_GAIN). 0-120.</summary>
    public static KatanaParameterDefinition PreampGain { get; } =
        new("preamp-gain", "Preamp Gain", [0x60, 0x00, 0x00, 0x22], maximum: 120,
            description: "Adjusts the gain/distortion amount in channel mode.");

    /// <summary>Parameter definition for Preamp Bass.</summary>
    public static KatanaParameterDefinition PreampBass { get; } =
        new("preamp-bass", "Preamp Bass", [0x60, 0x00, 0x00, 0x24],
            description: "Adjusts the low frequency tone in channel mode.");

    /// <summary>Parameter definition for Preamp Middle.</summary>
    public static KatanaParameterDefinition PreampMiddle { get; } =
        new("preamp-middle", "Preamp Middle", [0x60, 0x00, 0x00, 0x25],
            description: "Adjusts the midrange tone in channel mode.");

    /// <summary>Parameter definition for Preamp Treble.</summary>
    public static KatanaParameterDefinition PreampTreble { get; } =
        new("preamp-treble", "Preamp Treble", [0x60, 0x00, 0x00, 0x26],
            description: "Adjusts the high frequency tone in channel mode.");

    /// <summary>Parameter definition for Preamp Presence.</summary>
    public static KatanaParameterDefinition PreampPresence { get; } =
        new("preamp-presence", "Preamp Presence", [0x60, 0x00, 0x00, 0x27],
            description: "Adjusts the balance in the extended upper frequency range in channel mode.");

    /// <summary>Preamp output level (channel mode). Not the same as the master Volume knob.</summary>
    public static KatanaParameterDefinition PreampLevel { get; } =
        new("preamp-level", "Preamp Level", [0x60, 0x00, 0x00, 0x28],
            description: "Adjusts the preamp output level in channel mode (distinct from the master Volume knob).");

    /// <summary>Parameter definition for Preamp Bright.</summary>
    public static KatanaParameterDefinition PreampBright { get; } =
        new("preamp-bright", "Preamp Bright", [0x60, 0x00, 0x00, 0x29], maximum: 1,
            description: "Turns the bright switch on/off, adding brightness to the tone.");

    /// <summary>Parameter definition for Preamp Solo Sw.</summary>
    public static KatanaParameterDefinition PreampSoloSw { get; } =
        new("preamp-solo-sw", "Preamp Solo", [0x60, 0x00, 0x00, 0x2B], maximum: 1,
            description: "Switches to a tone suitable for solos (solo function on/off).");

    /// <summary>Parameter definition for Preamp Solo Level.</summary>
    public static KatanaParameterDefinition PreampSoloLevel { get; } =
        new("preamp-solo-level", "Preamp Solo Lvl", [0x60, 0x00, 0x00, 0x2C],
            description: "Adjusts the volume level when the Solo switch is ON.");

    /// <summary> Booster DSP params (same for all booster types) ───────────────────────── </summary>
    public static KatanaParameterDefinition BoosterDrive { get; } =
        new("booster-drive", "Drive", [0x60, 0x00, 0x00, 0x12], maximum: 120,
            description: "Adjusts the depth of distortion.");

    /// <summary>Parameter definition for Booster Tone.</summary>
    public static KatanaParameterDefinition BoosterTone { get; } =
        new("booster-tone", "Tone", [0x60, 0x00, 0x00, 0x13],
            description: "Adjusts the tone.");

    /// <summary>Parameter definition for Booster Bottom.</summary>
    public static KatanaParameterDefinition BoosterBottom { get; } =
        new("booster-bottom", "Bottom", [0x60, 0x00, 0x00, 0x14],
            description:
            "Adjusts the tone for the low frequency range. Turning left reduces the low end; turning right boosts it.");

    /// <summary>Parameter definition for Booster Solo Sw.</summary>
    public static KatanaParameterDefinition BoosterSoloSw { get; } =
        new("booster-solo-sw", "Solo", [0x60, 0x00, 0x00, 0x15], maximum: 1,
            description: "Switches to a tone that is suitable for solos.");

    /// <summary>Parameter definition for Booster Solo Level.</summary>
    public static KatanaParameterDefinition BoosterSoloLevel { get; } =
        new("booster-solo-level", "Solo Lvl", [0x60, 0x00, 0x00, 0x16],
            description: "Adjusts the volume level when the Solo switch is ON.");

    /// <summary>Parameter definition for Booster Effect Level.</summary>
    public static KatanaParameterDefinition BoosterEffectLevel { get; } =
        new("booster-effect-level", "E.Level", [0x60, 0x00, 0x00, 0x17],
            description: "Adjusts the volume of the effect sound.");

    /// <summary>Parameter definition for Booster Direct Mix.</summary>
    public static KatanaParameterDefinition BoosterDirectMix { get; } =
        new("booster-direct-mix", "D.Mix", [0x60, 0x00, 0x00, 0x18],
            description: "Adjusts the volume of the direct (dry) sound.");

    /// <summary> Delay DSP params ───────────────────────────────────────────────────────── </summary>
    public static KatanaParameterDefinition DelayFeedback { get; } =
        new("delay-feedback", "Feedback", [0x60, 0x00, 0x05, 0x04], maximum: 100,
            description:
            "Adjusts the volume returned to the input. Higher values increase the number of delay repeats.");

    /// <summary>Parameter definition for Delay High Cut.</summary>
    public static KatanaParameterDefinition DelayHighCut { get; } =
        new("delay-high-cut", "High Cut", [0x60, 0x00, 0x05, 0x05], maximum: 14,
            description:
            "Sets the frequency at which the high cut filter begins to take effect (630 Hz–12.5 kHz, FLAT).");

    /// <summary>Parameter definition for Delay Effect Level.</summary>
    public static KatanaParameterDefinition DelayEffectLevel { get; } =
        new("delay-effect-level", "E.Level", [0x60, 0x00, 0x05, 0x06], maximum: 120,
            description: "Adjusts the volume of the delay effect sound.");

    /// <summary>Parameter definition for Delay Direct Mix.</summary>
    public static KatanaParameterDefinition DelayDirectMix { get; } =
        new("delay-direct-mix", "D.Mix", [0x60, 0x00, 0x05, 0x07], maximum: 100,
            description: "Adjusts the volume of the direct (dry) sound.");

    /// <summary>Parameter definition for Delay Tap Time.</summary>
    public static KatanaParameterDefinition DelayTapTime { get; } =
        new("delay-tap-time", "Tap Time", [0x60, 0x00, 0x05, 0x08], maximum: 100,
            description:
            "Adjusts the R-channel delay time relative to L (100% = same as L). Only available for PAN type.");

    /// <summary>Parameter definition for Delay Mod Rate.</summary>
    public static KatanaParameterDefinition DelayModRate { get; } =
        new("delay-mod-rate", "Mod Rate", [0x60, 0x00, 0x05, 0x13], maximum: 100,
            description:
            "Adjusts the modulation rate of the delay sound. Only available for MODULATE and SDE-3000 types.");

    /// <summary>Parameter definition for Delay Mod Depth.</summary>
    public static KatanaParameterDefinition DelayModDepth { get; } =
        new("delay-mod-depth", "Mod Depth", [0x60, 0x00, 0x05, 0x14], maximum: 100,
            description:
            "Adjusts the modulation depth of the delay sound. Only available for MODULATE and SDE-3000 types.");

    /// <summary>Parameter definition for Delay Range.</summary>
    public static KatanaParameterDefinition DelayRange { get; } =
        new("delay-range", "Range", [0x60, 0x00, 0x05, 0x15], maximum: 1,
            description:
            "Switches the frequency response of the SDE-3000 delay (8kHz or 17kHz). Only for SDE-3000 type.");

    /// <summary>Parameter definition for Delay Filter.</summary>
    public static KatanaParameterDefinition DelayFilter { get; } =
        new("delay-filter", "Filter", [0x60, 0x00, 0x05, 0x16], maximum: 1,
            description:
            "Turns the filter on/off. When ON, provides a natural-sounding echo effect. Only for SDE-3000 type.");

    /// <summary>Parameter definition for Delay Feedback Phase.</summary>
    public static KatanaParameterDefinition DelayFeedbackPhase { get; } =
        new("delay-feedback-phase", "FB Phase", [0x60, 0x00, 0x05, 0x17], maximum: 1,
            description:
            "Specifies the phase of the delay feedback. Selecting INV inverts the phase. Only for SDE-3000 type.");

    /// <summary>Parameter definition for Delay Delay Phase.</summary>
    public static KatanaParameterDefinition DelayDelayPhase { get; } =
        new("delay-delay-phase", "Dly Phase", [0x60, 0x00, 0x05, 0x18], maximum: 1,
            description:
            "Specifies the phase of the delay sound. Selecting INV inverts the phase. Only for SDE-3000 type.");

    /// <summary>Parameter definition for Delay Mod Sw.</summary>
    public static KatanaParameterDefinition DelayModSw { get; } =
        new("delay-mod-sw", "Mod SW", [0x60, 0x00, 0x05, 0x19], maximum: 1,
            description: "Turns the delay modulation on/off. Only available for SDE-3000 type.");

    /// <summary> Delay 2 DSP params ─────────────────────────────────────────────────────── </summary>
    public static KatanaParameterDefinition Delay2Feedback { get; } =
        new("delay2-feedback", "Feedback", [0x60, 0x00, 0x05, 0x24], maximum: 100,
            description:
            "Adjusts the volume returned to the input. Higher values increase the number of delay 2 repeats.");

    /// <summary>Parameter definition for Delay 2 High Cut.</summary>
    public static KatanaParameterDefinition Delay2HighCut { get; } =
        new("delay2-high-cut", "High Cut", [0x60, 0x00, 0x05, 0x25], maximum: 14,
            description: "Sets the frequency at which the high cut filter begins to take effect for Delay 2.");

    /// <summary>Parameter definition for Delay 2 Effect Level.</summary>
    public static KatanaParameterDefinition Delay2EffectLevel { get; } =
        new("delay2-effect-level", "E.Level", [0x60, 0x00, 0x05, 0x26], maximum: 120,
            description: "Adjusts the volume of the Delay 2 effect sound.");

    /// <summary>Parameter definition for Delay 2 Direct Mix.</summary>
    public static KatanaParameterDefinition Delay2DirectMix { get; } =
        new("delay2-direct-mix", "D.Mix", [0x60, 0x00, 0x05, 0x27], maximum: 100,
            description: "Adjusts the volume of the direct (dry) sound for Delay 2.");

    /// <summary>Parameter definition for Delay 2 Tap Time.</summary>
    public static KatanaParameterDefinition Delay2TapTime { get; } =
        new("delay2-tap-time", "Tap Time", [0x60, 0x00, 0x05, 0x28], maximum: 100,
            description: "Adjusts the R-channel delay time relative to L for Delay 2. Only available for PAN type.");

    /// <summary>Parameter definition for Delay 2 Mod Rate.</summary>
    public static KatanaParameterDefinition Delay2ModRate { get; } =
        new("delay2-mod-rate", "Mod Rate", [0x60, 0x00, 0x05, 0x33], maximum: 100,
            description: "Adjusts the modulation rate of the Delay 2 sound.");

    /// <summary>Parameter definition for Delay 2 Mod Depth.</summary>
    public static KatanaParameterDefinition Delay2ModDepth { get; } =
        new("delay2-mod-depth", "Mod Depth", [0x60, 0x00, 0x05, 0x34], maximum: 100,
            description: "Adjusts the modulation depth of the Delay 2 sound.");

    /// <summary>Parameter definition for Delay 2 Range.</summary>
    public static KatanaParameterDefinition Delay2Range { get; } =
        new("delay2-range", "Range", [0x60, 0x00, 0x05, 0x35], maximum: 1,
            description: "Switches the frequency response range for Delay 2.");

    /// <summary>Parameter definition for Delay 2 Filter.</summary>
    public static KatanaParameterDefinition Delay2Filter { get; } =
        new("delay2-filter", "Filter", [0x60, 0x00, 0x05, 0x36], maximum: 1,
            description: "Turns the filter on/off for Delay 2.");

    /// <summary>Parameter definition for Delay 2 Feedback Phase.</summary>
    public static KatanaParameterDefinition Delay2FeedbackPhase { get; } =
        new("delay2-feedback-phase", "FB Phase", [0x60, 0x00, 0x05, 0x37], maximum: 1,
            description: "Specifies the phase of the Delay 2 feedback. Selecting INV inverts the phase.");

    /// <summary>Parameter definition for Delay 2 Delay Phase.</summary>
    public static KatanaParameterDefinition Delay2DelayPhase { get; } =
        new("delay2-delay-phase", "Dly Phase", [0x60, 0x00, 0x05, 0x38], maximum: 1,
            description: "Specifies the phase of the Delay 2 sound. Selecting INV inverts the phase.");

    /// <summary>Parameter definition for Delay 2 Mod Sw.</summary>
    public static KatanaParameterDefinition Delay2ModSw { get; } =
        new("delay2-mod-sw", "Mod SW", [0x60, 0x00, 0x05, 0x39], maximum: 1,
            description: "Turns the Delay 2 modulation on/off.");

    /// <summary> Reverb DSP params (shared for all reverb types) ───────────────────────── </summary>
    public static KatanaParameterDefinition ReverbTime { get; } =
        new("reverb-time", "Time", [0x60, 0x00, 0x05, 0x42],
            description: "Adjusts the length (time) of reverberation.");

    /// <summary>Parameter definition for Reverb Pre Delay.</summary>
    public static KatanaParameterDefinition ReverbPreDelay { get; } =
        new("reverb-pre-delay", "Pre Delay", [0x60, 0x00, 0x05, 0x43],
            description: "Adjusts the time until the reverb sound appears (0–500 ms).");

    /// <summary>Parameter definition for Reverb Low Cut.</summary>
    public static KatanaParameterDefinition ReverbLowCut { get; } =
        new("reverb-low-cut", "Low Cut", [0x60, 0x00, 0x05, 0x44], maximum: 17,
            description: "Sets the frequency at which the low cut filter begins to take effect (FLAT or 20–800 Hz).");

    /// <summary>Parameter definition for Reverb High Cut.</summary>
    public static KatanaParameterDefinition ReverbHighCut { get; } =
        new("reverb-high-cut", "High Cut", [0x60, 0x00, 0x05, 0x45], maximum: 17,
            description:
            "Sets the frequency at which the high cut filter begins to take effect (630 Hz–12.5 kHz, FLAT).");

    /// <summary>Parameter definition for Reverb Density.</summary>
    public static KatanaParameterDefinition ReverbDensity { get; } =
        new("reverb-density", "Density", [0x60, 0x00, 0x05, 0x46], maximum: 10,
            description: "Adjusts the density of the reverb sound.");

    /// <summary>Parameter definition for Reverb Color.</summary>
    public static KatanaParameterDefinition ReverbColor { get; } =
        new("reverb-color", "Color", [0x60, 0x00, 0x05, 0x47], maximum: 100,
            description: "Adjusts the unique tone of the spring reverb. Only available when type is SPRING.");

    /// <summary>Parameter definition for Reverb Effect Level.</summary>
    public static KatanaParameterDefinition ReverbEffectLevel { get; } =
        new("reverb-effect-level", "E.Level", [0x60, 0x00, 0x05, 0x48], maximum: 100,
            description: "Adjusts the volume of the reverb effect sound.");

    /// <summary>Parameter definition for Reverb Direct Mix.</summary>
    public static KatanaParameterDefinition ReverbDirectMix { get; } =
        new("reverb-direct-mix", "D.Mix", [0x60, 0x00, 0x05, 0x49], maximum: 100,
            description: "Adjusts the volume of the direct (dry) sound.");

    /// <summary>Auto-generated: static IReadOnlyList<KatanaPanelEffectDefinition> PanelEffects { get; } =</summary>
    public static IReadOnlyList<KatanaPanelEffectDefinition> PanelEffects { get; } =
    [
        new("booster", "Booster", BoosterSwitch, BoosterVariation, BoosterType,
            detailParameters:
            [
                BoosterDrive, BoosterTone, BoosterBottom, BoosterSoloSw, BoosterSoloLevel, BoosterEffectLevel,
                BoosterDirectMix
            ]),
        new("mod", "Mod", ModSwitch, ModVariation, ModType),
        new("fx", "FX", FxSwitch, FxVariation, FxType),
        new("delay", "Delay", DelaySwitch, DelayVariation, DelayType,
            detailParameters:
            [
                DelayFeedback, DelayHighCut, DelayEffectLevel, DelayDirectMix, DelayTapTime, DelayModRate,
                DelayModDepth, DelayRange, DelayFilter, DelayFeedbackPhase, DelayDelayPhase, DelayModSw
            ]),
        new("delay2", "Delay 2", Delay2Switch, typeParameter: Delay2Type,
            detailParameters:
            [
                Delay2Feedback, Delay2HighCut, Delay2EffectLevel, Delay2DirectMix, Delay2TapTime, Delay2ModRate,
                Delay2ModDepth, Delay2Range, Delay2Filter, Delay2FeedbackPhase, Delay2DelayPhase, Delay2ModSw
            ]),
        new("reverb", "Reverb", ReverbSwitch, ReverbVariation, ReverbType,
            detailParameters:
            [
                ReverbTime, ReverbPreDelay, ReverbLowCut, ReverbHighCut, ReverbDensity, ReverbColor, ReverbEffectLevel,
                ReverbDirectMix
            ])
    ];

    /// <summary>Auto-generated: static KatanaPedalFxDefinition PedalFx { get; } =</summary>
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

    /// <summary>Auto-generated: static IReadOnlyList<KatanaParameterDefinition> PedalFxReadParameters { get; } =</summary>
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
        FootVolume
    ];
}
