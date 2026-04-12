namespace Kataka.Domain.Midi;

public static partial class KatanaMkIIParameterCatalog
{
    // Address helper: slotBase is 0x01 for Mod, 0x03 for FX.
    // sectionRel is the offset within prm_prop_patch_fx (BTS absolute addr minus 0x0100).
    private static byte[] ModFxAddr(byte slotBase, int sectionRel) =>
        [0x60, 0x00, (byte)(slotBase + (sectionRel >> 8)), (byte)(sectionRel & 0xFF)];

    private static KatanaParameterDefinition ModFxParam(
        string key, string display, byte slotBase, int sectionRel,
        byte minimum = 0, byte maximum = 100) =>
        new(key, display, ModFxAddr(slotBase, sectionRel), minimum, maximum);

    // ── CHORUS (2×2 Chorus, type index 0) ────────────────────────────────────────
    // BTS section: PRM_FX1_2x2CHORUS, addrs 0x0137–0x0140

    public static IReadOnlyList<KatanaParameterDefinition> ModChorusParams { get; } = BuildChorusParams(0x01);
    public static IReadOnlyList<KatanaParameterDefinition> FxChorusParams  { get; } = BuildChorusParams(0x03);

    private static IReadOnlyList<KatanaParameterDefinition> BuildChorusParams(byte slot)
    {
        string s = slot == 0x01 ? "mod" : "fx";
        return
        [
            ModFxParam($"modfx-{s}-chorus-xover-freq",     "XOVER FREQ",    slot, 0x0037, maximum: 16),
            ModFxParam($"modfx-{s}-chorus-low-rate",       "LOW RATE",      slot, 0x0038),
            ModFxParam($"modfx-{s}-chorus-low-depth",      "LOW DEPTH",     slot, 0x0039),
            ModFxParam($"modfx-{s}-chorus-low-pre-delay",  "LOW PRE DLY",   slot, 0x003A, maximum: 80),
            ModFxParam($"modfx-{s}-chorus-low-level",      "LOW LEVEL",     slot, 0x003B),
            ModFxParam($"modfx-{s}-chorus-high-rate",      "HIGH RATE",     slot, 0x003C),
            ModFxParam($"modfx-{s}-chorus-high-depth",     "HIGH DEPTH",    slot, 0x003D),
            ModFxParam($"modfx-{s}-chorus-high-pre-delay", "HIGH PRE DLY",  slot, 0x003E, maximum: 80),
            ModFxParam($"modfx-{s}-chorus-high-level",     "HIGH LEVEL",    slot, 0x003F),
            ModFxParam($"modfx-{s}-chorus-direct-mix",     "DIRECT MIX",    slot, 0x0040),
        ];
    }

    // ── FLANGER (type index 1) ────────────────────────────────────────────────────
    // BTS section: PRM_FX1_FLANGER, addrs 0x010B–0x0112 (skip 0x010F which is empty)

    public static IReadOnlyList<KatanaParameterDefinition> ModFlangerParams { get; } = BuildFlangerParams(0x01);
    public static IReadOnlyList<KatanaParameterDefinition> FxFlangerParams  { get; } = BuildFlangerParams(0x03);

    private static IReadOnlyList<KatanaParameterDefinition> BuildFlangerParams(byte slot)
    {
        string s = slot == 0x01 ? "mod" : "fx";
        return
        [
            ModFxParam($"modfx-{s}-flanger-rate",         "RATE",          slot, 0x000B),
            ModFxParam($"modfx-{s}-flanger-depth",        "DEPTH",         slot, 0x000C),
            ModFxParam($"modfx-{s}-flanger-manual",       "MANUAL",        slot, 0x000D),
            ModFxParam($"modfx-{s}-flanger-resonance",    "RESONANCE",     slot, 0x000E),
            ModFxParam($"modfx-{s}-flanger-low-cut",      "LOW CUT",       slot, 0x0010, maximum: 10),
            ModFxParam($"modfx-{s}-flanger-effect-level", "EFFECT LEVEL",  slot, 0x0011),
            ModFxParam($"modfx-{s}-flanger-direct-mix",   "DIRECT MIX",    slot, 0x0012),
        ];
    }

    // ── PHASER (type index 2) ─────────────────────────────────────────────────────
    // BTS section: PRM_FX1_PHASER, addrs 0x0103–0x010A

    public static IReadOnlyList<KatanaParameterDefinition> ModPhaserParams { get; } = BuildPhaserParams(0x01);
    public static IReadOnlyList<KatanaParameterDefinition> FxPhaserParams  { get; } = BuildPhaserParams(0x03);

    private static IReadOnlyList<KatanaParameterDefinition> BuildPhaserParams(byte slot)
    {
        string s = slot == 0x01 ? "mod" : "fx";
        return
        [
            ModFxParam($"modfx-{s}-phaser-type",         "TYPE",          slot, 0x0003, maximum: 3),
            ModFxParam($"modfx-{s}-phaser-rate",         "RATE",          slot, 0x0004),
            ModFxParam($"modfx-{s}-phaser-depth",        "DEPTH",         slot, 0x0005),
            ModFxParam($"modfx-{s}-phaser-manual",       "MANUAL",        slot, 0x0006),
            ModFxParam($"modfx-{s}-phaser-resonance",    "RESONANCE",     slot, 0x0007),
            ModFxParam($"modfx-{s}-phaser-step-rate",    "STEP RATE",     slot, 0x0008),
            ModFxParam($"modfx-{s}-phaser-effect-level", "EFFECT LEVEL",  slot, 0x0009),
            ModFxParam($"modfx-{s}-phaser-direct-mix",   "DIRECT MIX",    slot, 0x000A),
        ];
    }
}
