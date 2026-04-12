namespace Kataka.Domain.Midi;

/// <summary>
/// Type name lookup tables for Katana MK2 effect type parameters.
/// Source: BOSS TONE STUDIO for KATANA MkII app (layout.div select-box option lists).
/// Keys are wire byte values; values are human-readable display names.
/// Byte values not present in a table fall back to "Type N" formatting.
/// </summary>
public static class KatanaTypeNameTables
{
    // Source: BTS booster-type-select-box. Sequential, no skips. Max 22.
    public static IReadOnlyDictionary<byte, string> BoosterTypes { get; } =
        new Dictionary<byte, string>
        {
            [0]  = "CLEAN BOOST",
            [1]  = "TREBLE BOOST",
            [2]  = "MID BOOST",
            [3]  = "CRUNCH OD",
            [4]  = "BLUES DRIVE",
            [5]  = "OVERDRIVE",
            [6]  = "NATURAL OD",
            [7]  = "WARM OD",
            [8]  = "TURBO OD",
            [9]  = "T-SCREAM",
            [10] = "DISTORTION",
            [11] = "FAT DS",
            [12] = "DST+",
            [13] = "GUV DS",
            [14] = "RAT",
            [15] = "METAL ZONE",
            [16] = "METAL DS",
            [17] = "'60S FUZZ",
            [18] = "MUFF FUZZ",
            [19] = "OCT FUZZ",
            [20] = "HM-2",
            [21] = "METAL CORE",
            [22] = "CENTA OD",
        };

    // Source: BTS modfx-fx-type-select-box (shared for Mod and FX). Sequential, no skips. Max 30.
    public static IReadOnlyDictionary<byte, string> ModFxTypes { get; } =
        new Dictionary<byte, string>
        {
            [0]  = "CHORUS",
            [1]  = "FLANGER",
            [2]  = "PHASER",
            [3]  = "UNI-V",
            [4]  = "TREMOLO",
            [5]  = "VIBRATO",
            [6]  = "ROTARY",
            [7]  = "RING MOD",
            [8]  = "SLOW GEAR",
            [9]  = "SLICER",
            [10] = "COMP",
            [11] = "LIMITER",
            [12] = "T.WAH",
            [13] = "AUTO WAH",
            [14] = "PEDAL WAH",
            [15] = "GRAPHIC EQ",
            [16] = "PARAMETRIC EQ",
            [17] = "GUITAR SIM",
            [18] = "AC.GUITAR SIM",
            [19] = "AC.PROCESSOR",
            [20] = "WAVE SYNTH",
            [21] = "OCTAVE",
            [22] = "HEAVY OCTAVE",
            [23] = "PITCH SHIFTER",
            [24] = "HARMONIST",
            [25] = "HUMANIZER",
            [26] = "PHASER 90E",
            [27] = "FLANGER 117E",
            [28] = "WAH 95E",
            [29] = "DC-30",
            [30] = "PEDAL BEND",
        };

    // Source: BTS delay-delay1-type-select-box. Sequential, no skips. Max 7.
    public static IReadOnlyDictionary<byte, string> DelayTypes { get; } =
        new Dictionary<byte, string>
        {
            [0] = "DIGITAL",
            [1] = "PAN",
            [2] = "STEREO",
            [3] = "ANALOG",
            [4] = "TAPE ECHO",
            [5] = "REVERSE",
            [6] = "MODULATE",
            [7] = "SDE-3000",
        };

    // Source: BTS reverb-type-select-box. Sequential, no skips. Max 4.
    public static IReadOnlyDictionary<byte, string> ReverbTypes { get; } =
        new Dictionary<byte, string>
        {
            [0] = "PLATE",
            [1] = "ROOM",
            [2] = "HALL",
            [3] = "SPRING",
            [4] = "MODULATE",
        };

    // Source: BTS pedalwah-type-select-box. Sequential, no skips. Max 5.
    public static IReadOnlyDictionary<byte, string> PedalWahTypes { get; } =
        new Dictionary<byte, string>
        {
            [0] = "CRY WAH",
            [1] = "VO WAH",
            [2] = "FAT WAH",
            [3] = "LIGHT WAH",
            [4] = "7STRING WAH",
            [5] = "RESO WAH",
        };

    // Source: BTS pedalfx-type-select-box first three entries (pedal expression types). Max 2.
    public static IReadOnlyDictionary<byte, string> PedalFxTypes { get; } =
        new Dictionary<byte, string>
        {
            [0] = "PEDAL WAH",
            [1] = "PEDAL BEND",
            [2] = "WAH 95E",
        };

    /// <summary>Returns the type name lookup table for a parameter key, or null if none.</summary>
    public static IReadOnlyDictionary<byte, string>? GetTableForKey(string parameterKey) =>
        parameterKey switch
        {
            "panel-booster-type" => BoosterTypes,
            "panel-mod-type"     => ModFxTypes,
            "panel-fx-type"      => ModFxTypes,
            "panel-delay-type"   => DelayTypes,
            "panel-delay2-type"  => DelayTypes,
            "panel-reverb-type"  => ReverbTypes,
            _                    => null,
        };
}
