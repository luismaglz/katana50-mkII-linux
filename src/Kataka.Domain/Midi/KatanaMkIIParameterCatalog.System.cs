namespace Kataka.Domain.Midi;

public static partial class KatanaMkIIParameterCatalog
{
    /// <summary>
    ///     Current patch/channel selection.
    ///     The amp sends a 2-byte block at 0x00010000: [0x00, channel_code].
    ///     The actual channel byte lives at offset +1 (0x00010001).
    /// </summary>
    public static KatanaParameterDefinition CurrentChannel { get; } =
        new("current-channel", "Channel", [0x00, 0x01, 0x00, 0x01], maximum: 8,
            description: "Currently selected patch/channel number (raw SysEx code).");

    /// <summary>SysEx byte values for each selectable channel.</summary>
    public static byte ChannelPanel { get; } = 0;

    public static byte ChannelChA1 { get; } = 1;
    public static byte ChannelChA2 { get; } = 2;
    public static byte ChannelChB1 { get; } = 5;
    public static byte ChannelChB2 { get; } = 6;
}
