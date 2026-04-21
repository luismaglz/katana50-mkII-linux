using Kataka.Domain.Midi;

namespace Kataka.App.Services;

public static class Utilities
{
    public static string AddressToKey(IReadOnlyList<byte> address) =>
        $"{address[0]:X2}-{address[1]:X2}-{address[2]:X2}-{address[3]:X2}";

    public static byte[] AddressOffset(IReadOnlyList<byte> baseAddr, int offset)
    {
        // Roland SysEx addresses use 4 bytes where each byte is 7-bit (0x00–0x7F).
        // Decode to a 28-bit integer, add offset, then re-encode.
        var b28 = ((baseAddr[0] & 0x7F) << 21)
                | ((baseAddr[1] & 0x7F) << 14)
                | ((baseAddr[2] & 0x7F) << 7)
                |  (baseAddr[3] & 0x7F);
        b28 += offset;
        return
        [
            (byte)((b28 >> 21) & 0x7F),
            (byte)((b28 >> 14) & 0x7F),
            (byte)((b28 >> 7)  & 0x7F),
            (byte) (b28        & 0x7F)
        ];
    }

    public static int DecodeDelayTime(IReadOnlyList<byte> data)
    {
        if (data.Count != 2) throw new ArgumentException("Delay time data must contain exactly 2 bytes.", nameof(data));
        return ((data[0] & 0x0F) << 7) | (data[1] & 0x7F);
    }

    public static string ToPanelChannelDisplay(KatanaPanelChannel channel) => channel switch
    {
        KatanaPanelChannel.ChA1 => "CH A1",
        KatanaPanelChannel.ChA2 => "CH A2",
        KatanaPanelChannel.ChB1 => "CH B1",
        KatanaPanelChannel.ChB2 => "CH B2",
        _ => "Panel"
    };

    public static KatanaPanelChannel ParsePanelChannelDisplay(string display) => display switch
    {
        "CH A1" => KatanaPanelChannel.ChA1,
        "CH A2" => KatanaPanelChannel.ChA2,
        "CH B1" => KatanaPanelChannel.ChB1,
        "CH B2" => KatanaPanelChannel.ChB2,
        _ => KatanaPanelChannel.Panel
    };
}
