namespace Kataka.Application.Katana;

/// <summary>
///     Describes a single named block within a Katana patch in the "Temporary" area.
/// </summary>
/// <param name="TslKey">The TSL paramSet block name (without "UserPatch%" prefix), e.g. "Patch_0".</param>
/// <param name="Offset">Byte offset from the Temporary patch base (0x60000000).</param>
/// <param name="Size">Block size in bytes.</param>
public sealed record KatanaPatchBlock(string TslKey, int Offset, int Size)
{
    /// <summary>
    ///     Computes the 4-byte Roland MIDI address for this block within the Temporary patch area.
    ///     Temporary patch base = 0x60000000.
    /// </summary>
    public byte[] Address
    {
        get
        {
            var full = 0x60000000 + Offset;
            return
            [
                (byte)((full >> 24) & 0x7F),
                (byte)((full >> 16) & 0x7F),
                (byte)((full >> 8) & 0x7F),
                (byte)(full & 0x7F)
            ];
        }
    }
}

/// <summary>
///     All known patch blocks in a Katana MkII patch, ordered by MIDI address offset.
///     Derived from the Boss Tone Studio address_map.js (Patch array, Temporary area = 0x60000000).
/// </summary>
public static class KatanaPatchBlocks
{
    public static readonly IReadOnlyList<KatanaPatchBlock> All =
    [
        new("PatchName", 0x00000000, 16),
        new("Patch_0", 0x00000010, 72),
        new("Eq(2)", 0x00000060, 24),
        new("Fx(1)", 0x00000100, 225),
        new("Fx(2)", 0x00000300, 225),
        new("Delay(1)", 0x00000500, 26),
        new("Delay(2)", 0x00000520, 26),
        new("Patch_1", 0x00000540, 91),
        new("Patch_2", 0x00000620, 36),
        new("Status", 0x00000650, 18),
        new("KnobAsgn", 0x00000700, 34),
        new("ExpPedalAsgn", 0x00000800, 34),
        new("ExpPedalAsgnMinMax", 0x00000830, 78),
        new("GafcExp1Asgn", 0x00000900, 34),
        new("GafcExp1AsgnMinMax", 0x00000930, 78),
        new("GafcExp2Asgn", 0x00000A00, 34),
        new("GafcExp2AsgnMinMax", 0x00000A30, 78),
        new("GafcExp3Asgn", 0x00000B00, 34),
        new("GafcExp3AsgnMinMax", 0x00000B30, 78),
        new("GafcExExp1Asgn", 0x00000C00, 34),
        new("GafcExExp1AsgnMinMax", 0x00000C30, 78),
        new("GafcExExp2Asgn", 0x00000D00, 34),
        new("GafcExExp2AsgnMinMax", 0x00000D30, 78),
        new("GafcExExp3Asgn", 0x00000E00, 34),
        new("GafcExExp3AsgnMinMax", 0x00000E30, 78),
        new("CtrlAsgn", 0x00000F00, 8),
        new("FsAsgn", 0x00000F08, 2),
        new("Patch_Mk2V2", 0x00000F10, 22),
        new("Contour(1)", 0x00000F30, 2),
        new("Contour(2)", 0x00000F38, 2),
        new("Contour(3)", 0x00000F40, 2)
    ];

    public static readonly IReadOnlyDictionary<string, KatanaPatchBlock> ByKey =
        All.ToDictionary(b => b.TslKey, StringComparer.Ordinal);
}
