namespace Kataka.Domain.Midi;

/// <summary>Auto-generated: sealed class KatanaParameterDefinition</summary>
public sealed class KatanaParameterDefinition
{
    /// <summary>Auto-generated: KatanaParameterDefinition(</summary>
    public KatanaParameterDefinition(
        string key,
        string displayName,
        IReadOnlyList<byte> address,
        int minimum = 0,
        int maximum = 100,
        IReadOnlyList<byte>? skippedValues = null,
        string? description = null,
        int byteSize = 1)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(address);

        if (address.Count != 4)
            throw new ArgumentException("Katana parameter addresses must contain exactly 4 bytes.", nameof(address));

        if (minimum > maximum)
            throw new ArgumentOutOfRangeException(nameof(minimum), "Minimum must be less than or equal to maximum.");

        if (byteSize < 1 || byteSize > 2)
            throw new ArgumentOutOfRangeException(nameof(byteSize), "ByteSize must be 1 or 2.");

        Key = key;
        DisplayName = displayName;
        Address = address.ToArray();
        Minimum = minimum;
        Maximum = maximum;
        SkippedValues = skippedValues?.ToArray() ?? [];
        Description = description;
        ByteSize = byteSize;
    }

    /// <summary>Auto-generated: string Key { get; }</summary>
    public string Key { get; }

    /// <summary>Auto-generated: string DisplayName { get; }</summary>
    public string DisplayName { get; }

    /// <summary>Auto-generated: IReadOnlyList<byte> Address { get; }</summary>
    public IReadOnlyList<byte> Address { get; }

    /// <summary>Auto-generated: string AddressString => string.Join("-", Address.Select(b => b.ToString("X2")));</summary>
    public string AddressString => string.Join("-", Address.Select(b => b.ToString("X2")));

    /// <summary>Auto-generated: byte Minimum { get; }</summary>
    public int Minimum { get; }

    /// <summary>Auto-generated: byte Maximum { get; }</summary>
    public int Maximum { get; }

    /// <summary>
    ///     Number of SysEx bytes used to encode this parameter's value (INTEGER2x7 = 2, default = 1).
    ///     For ByteSize==2: encoded as two 7-bit bytes, value = (byte0 &lt;&lt; 7) | byte1.
    /// </summary>
    public int ByteSize { get; }

    /// <summary>Auto-generated: IReadOnlyList<byte> SkippedValues { get; }</summary>
    public IReadOnlyList<byte> SkippedValues { get; }

    /// <summary>Auto-generated: string? Description { get; }</summary>
    public string? Description { get; }
}
