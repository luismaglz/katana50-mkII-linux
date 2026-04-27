namespace Kataka.Domain.Midi;

/// <summary>
///     Type name lookup tables for Katana MK2 effect type parameters.
///     Source: BOSS TONE STUDIO for KATANA MkII app (layout.div select-box option lists).
///     Keys are wire byte values; values are human-readable display names.
///     Byte values not present in a table fall back to "Type N" formatting.
/// </summary>
public static class KatanaTypeNameTables
{
    // Source: BTS booster-type-select-box (item.json list_order + layout.div option labels).
    // Wire values are NOT sequential; BTS reorders display items independently of wire encoding.
    // Wire 7 is skipped (not present in BTS list_order). Max wire value from address_map.js: 25.
    /// <summary>Auto-generated: static IReadOnlyDictionary<byte, string> BoosterTypes { get; } =</summary>
    public static IReadOnlyDictionary<byte, string> BoosterTypes { get; } =
        new Dictionary<byte, string>
        {
            [0] = "MID BOOST",
            [1] = "CLEAN BOOST",
            [2] = "TREBLE BOOST",
            [3] = "CRUNCH OD",
            [4] = "NATURAL OD",
            [5] = "WARM OD",
            [6] = "FAT DS",
            // [7] skipped — not a valid wire value
            [8] = "METAL DS",
            [9] = "OCT FUZZ",
            [10] = "BLUES DRIVE",
            [11] = "OVERDRIVE",
            [12] = "T-SCREAM",
            [13] = "TURBO OD",
            [14] = "DISTORTION",
            [15] = "RAT",
            [16] = "GUV DS",
            [17] = "DST+",
            [18] = "METAL ZONE",
            [19] = "'60S FUZZ",
            [20] = "MUFF FUZZ",
            [21] = "HM-2",
            [22] = "METAL CORE",
            [23] = "CENTA OD"
        };

    // Source: BTS modfx-mod-type-select-box / modfx-fx-type-select-box (item.json list_order + layout.div labels).
    // Wire values are NOT sequential; BTS reorders display items independently of wire encoding.
    // Skipped wires: 5, 8, 11, 13, 17, 24, 30, 32, 33, 34. Max wire value: 40 (from address_map.js).
    /// <summary>Auto-generated: static IReadOnlyDictionary<byte, string> ModFxTypes { get; } =</summary>
    public static IReadOnlyDictionary<byte, string> ModFxTypes { get; } =
        new Dictionary<byte, string>
        {
            [0] = "T.WAH",
            [1] = "AUTO WAH",
            [2] = "PEDAL WAH",
            [3] = "COMP",
            [4] = "LIMITER",
            // [5] skipped
            [6] = "GRAPHIC EQ",
            [7] = "PARAMETRIC EQ",
            // [8] skipped
            [9] = "GUITAR SIM",
            [10] = "SLOW GEAR",
            // [11] skipped
            [12] = "WAVE SYNTH",
            // [13] skipped
            [14] = "OCTAVE",
            [15] = "PITCH SHIFTER",
            [16] = "HARMONIST",
            // [17] skipped
            [18] = "AC.PROCESSOR",
            [19] = "PHASER",
            [20] = "FLANGER",
            [21] = "TREMOLO",
            [22] = "ROTARY",
            [23] = "UNI-V",
            // [24] skipped
            [25] = "SLICER",
            [26] = "VIBRATO",
            [27] = "RING MOD",
            [28] = "HUMANIZER",
            [29] = "CHORUS",
            // [30] skipped
            [31] = "AC.GUITAR SIM",
            // [32], [33], [34] skipped
            [35] = "PHASER 90E",
            [36] = "FLANGER 117E",
            [37] = "WAH 95E",
            [38] = "DC-30",
            [39] = "HEAVY OCTAVE",
            [40] = "PEDAL BEND"
        };

    // Source: BTS delay-delay1-type-select-box (item.json list_order + layout.div labels).
    // Wire values 3, 4, 5 are skipped. Max wire value: 10 (from address_map.js).
    /// <summary>Auto-generated: static IReadOnlyDictionary<byte, string> DelayTypes { get; } =</summary>
    public static IReadOnlyDictionary<byte, string> DelayTypes { get; } =
        new Dictionary<byte, string>
        {
            [0] = "DIGITAL",
            [1] = "PAN",
            [2] = "STEREO",
            // [3], [4], [5] skipped
            [6] = "REVERSE",
            [7] = "ANALOG",
            [8] = "TAPE ECHO",
            [9] = "MODULATE",
            [10] = "SDE-3000"
        };

    // Source: BTS reverb-type-select-box (item.json list_order + layout.div labels).
    // Wire values 0 and 2 are skipped. Only 5 valid types. Max wire value: 6 (from address_map.js).
    /// <summary>Auto-generated: static IReadOnlyDictionary<byte, string> ReverbTypes { get; } =</summary>
    public static IReadOnlyDictionary<byte, string> ReverbTypes { get; } =
        new Dictionary<byte, string>
        {
            // [0] skipped
            [1] = "ROOM",
            // [2] skipped
            [3] = "HALL",
            [4] = "PLATE",
            [5] = "SPRING",
            [6] = "MODULATE"
        };

    // Source: BTS pedalwah-type-select-box. Sequential, no skips. Max 5.
    /// <summary>Auto-generated: static IReadOnlyDictionary<byte, string> PedalWahTypes { get; } =</summary>
    public static IReadOnlyDictionary<byte, string> PedalWahTypes { get; } =
        new Dictionary<byte, string>
        {
            [0] = "CRY WAH",
            [1] = "VO WAH",
            [2] = "FAT WAH",
            [3] = "LIGHT WAH",
            [4] = "7STRING WAH",
            [5] = "RESO WAH"
        };
}
