namespace Kataka.Domain.Midi;

/// <summary>
/// Type name lookup tables for Katana MK2 effect type parameters.
/// Based on the Katana MK2 FloorBoard source (stompbox_*.cpp) and BTS manual.
/// Keys are wire byte values; values are human-readable display names.
/// Byte values not present in a table fall back to "Type N" formatting.
/// </summary>
public static class KatanaTypeNameTables
{
    // Source: stompbox_od.cpp stack field ordering + BTS manual booster section.
    // Max 22, skip { 7 }. Default = 10 (TURBO OD).
    public static IReadOnlyDictionary<byte, string> BoosterTypes { get; } =
        new Dictionary<byte, string>
        {
            [0]  = "CLEAN BOOST",
            [1]  = "TREBLE BOOST",
            [2]  = "MID BOOST",
            [3]  = "CRUNCH OD",
            [4]  = "BLUES DRIVE",
            [5]  = "NATURAL OD",
            [6]  = "OD-1",
            // 7 skipped
            [8]  = "OD-2",
            [9]  = "T-SCREAM",
            [10] = "TURBO OD",
            [11] = "BLUESMAN OD",
            [12] = "HARMONIOUS OD",
            [13] = "LEAD DS",
            [14] = "METAL DS",
            [15] = "DIST ST",
            [16] = "MID DIST",
            [17] = "\'60S FUZZ",
            [18] = "OCTAVE FUZZ",
            [19] = "MUFF FUZZ",
            [20] = "OCT FUZZ",
            [21] = "BASS DS",
            [22] = "GUITAR SYNTH",
        };

    // Source: stompbox_fx1.cpp // comments in stack field order.
    // Max 39, skips { 5, 8, 11, 13, 17, 24, 30, 32, 33, 34 }. Default = 29 (ROTARY).
    // Note: exact mapping of types 16, 19-22, 27 was not determinable from source
    // alone; these fall through to "Type N" fallback until hardware-verified.
    public static IReadOnlyDictionary<byte, string> ModFxTypes { get; } =
        new Dictionary<byte, string>
        {
            [0]  = "T-Wah",
            [1]  = "Auto Wah",
            [2]  = "Sub Wah",
            [3]  = "Compressor",
            [4]  = "Limiter",
            // 5 skipped
            [6]  = "Graphic EQ",
            [7]  = "Parametric EQ",
            // 8 skipped
            [9]  = "Guitar Sim",
            [10] = "Slow Gear",
            // 11 skipped
            [12] = "Wave Synth",
            // 13 skipped
            [14] = "Octaver",
            [15] = "Pitch Shifter",
            // 16 = unknown — hardware-verify
            // 17 skipped
            [18] = "Harmonizer",
            // 19-22 = unknown — hardware-verify
            [23] = "Acoustic Guitar Processor",
            // 24 skipped
            [25] = "Phaser",
            [26] = "Flanger",
            // 27 = unknown — hardware-verify
            [28] = "Tremolo",
            [29] = "Rotary",
            // 30 skipped
            [31] = "Uni-V",
            // 32, 33, 34 skipped
            [35] = "Slicer",
            [36] = "Vibrato",
            [37] = "Ring Modulator",
            [38] = "Humanizer",
            [39] = "2CE",
        };

    // Source: stompbox_dd.cpp // comments in stack field order.
    // Max 10, no skips.
    public static IReadOnlyDictionary<byte, string> DelayTypes { get; } =
        new Dictionary<byte, string>
        {
            [0]  = "SINGLE",
            [1]  = "PAN",
            [2]  = "STEREO",
            [3]  = "DUAL SERIES",
            [4]  = "DUAL PARALLEL",
            [5]  = "DUAL L/R",
            [6]  = "REVERSE",
            [7]  = "ANALOG",
            [8]  = "TAPE",
            [9]  = "MODULATE",
            [10] = "SDE-3000",
        };

    // Source: BTS manual reverb section + stompbox_rv.cpp structure.
    // Max 6, no skips. Default = 4 (PLATE).
    public static IReadOnlyDictionary<byte, string> ReverbTypes { get; } =
        new Dictionary<byte, string>
        {
            [0] = "AMBIENCE",
            [1] = "ROOM",
            [2] = "HALL 1",
            [3] = "HALL 2",
            [4] = "PLATE",
            [5] = "SPRING",
            [6] = "MODULATE",
        };

    // Source: BTS manual pedal wah section (AllParams.cpp: Strings::PedalWahTypes).
    // Max 5, no skips. Used for Pedal FX Wah subtype and Mod/FX Sub Wah sub-control.
    // Note: ordering is best-effort; verify on hardware.
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

    // Source: AllParams.cpp Strings::WahFxTypes for PRM_PEDAL_FX_TYPE (max 2, no skips).
    public static IReadOnlyDictionary<byte, string> PedalFxTypes { get; } =
        new Dictionary<byte, string>
        {
            [0] = "WAH",
            [1] = "PEDAL BEND",
            [2] = "EVH WAH95",
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
