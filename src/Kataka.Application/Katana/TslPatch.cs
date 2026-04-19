namespace Kataka.Application.Katana;

/// <summary>
///     Represents a Boss Tone Studio .tsl patch file loaded into memory.
/// </summary>
public sealed class TslPatch
{
    public string Name { get; set; } = "New Patch";
    public string Memo { get; set; } = string.Empty;
    public string FormatRev { get; init; } = "0002";
    public string Device { get; init; } = "KATANA MkII";

    /// <summary>
    ///     Block data keyed by TSL block name (e.g. "Patch_0", "Fx(1)", "Status").
    ///     Values are the raw byte arrays for each block.
    /// </summary>
    public Dictionary<string, byte[]> Blocks { get; } = new();
}
