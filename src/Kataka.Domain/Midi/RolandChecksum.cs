namespace Kataka.Domain.Midi;

/// <summary>RolandChecksum - auto-generated summary.</summary>
public static class RolandChecksum
{

    /// <summary>Auto-generated: static byte Calculate(params byte[] bytes)</summary>
    public static byte Calculate(params byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        var sum = bytes.Sum(value => value);
        var remainder = sum % 0x80;
        return remainder == 0 ? (byte)0x00 : (byte)(0x80 - remainder);
    }
}
