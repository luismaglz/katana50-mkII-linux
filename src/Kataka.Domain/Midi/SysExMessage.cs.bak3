using System.Collections.ObjectModel;

namespace Kataka.Domain.Midi;

public sealed class SysExMessage
{
    public SysExMessage(IEnumerable<byte> bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        var buffer = bytes.ToArray();
        if (buffer.Length < 2)
        {
            throw new ArgumentException("A SysEx message must contain at least a start and end byte.", nameof(bytes));
        }

        if (buffer[0] != 0xF0)
        {
            throw new ArgumentException("A SysEx message must start with 0xF0.", nameof(bytes));
        }

        if (buffer[^1] != 0xF7)
        {
            throw new ArgumentException("A SysEx message must end with 0xF7.", nameof(bytes));
        }

        Bytes = Array.AsReadOnly(buffer);
    }

    public ReadOnlyCollection<byte> Bytes { get; }

    public string ToHexString(string separator = " ") =>
        string.Join(separator, Bytes.Select(value => value.ToString("X2")));
}
