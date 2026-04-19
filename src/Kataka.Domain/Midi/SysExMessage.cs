using System.Collections.ObjectModel;

namespace Kataka.Domain.Midi;

/// <summary>Auto-generated: sealed class SysExMessage</summary>
public sealed class SysExMessage
{
    /// <summary>Auto-generated: SysExMessage(IEnumerable<byte> bytes)</summary>
    public SysExMessage(IEnumerable<byte> bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        var buffer = bytes.ToArray();
        if (buffer.Length < 2)
            throw new ArgumentException("A SysEx message must contain at least a start and end byte.", nameof(bytes));

        if (buffer[0] != 0xF0) throw new ArgumentException("A SysEx message must start with 0xF0.", nameof(bytes));

        if (buffer[^1] != 0xF7) throw new ArgumentException("A SysEx message must end with 0xF7.", nameof(bytes));

        Bytes = Array.AsReadOnly(buffer);
    }

    /// <summary>Auto-generated: ReadOnlyCollection<byte> Bytes { get; }</summary>
    public ReadOnlyCollection<byte> Bytes { get; }

    /// <summary>Auto-generated: string ToHexString(string separator = " ") =></summary>
    public string ToHexString(string separator = " ") =>
        string.Join(separator, Bytes.Select(value => value.ToString("X2")));
}
