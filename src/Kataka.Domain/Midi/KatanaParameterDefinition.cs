namespace Kataka.Domain.Midi;

/// <summary>Auto-generated: sealed class KatanaParameterDefinition</summary>
public sealed class KatanaParameterDefinition
{
    /// <summary>Auto-generated: KatanaParameterDefinition(</summary>
    public KatanaParameterDefinition(
        string key,
        string displayName,
        IReadOnlyList<byte> address,
        byte minimum = 0,
        byte maximum = 100,
        IReadOnlyList<byte>? skippedValues = null,
        string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(address);

        if (address.Count != 4)
            throw new ArgumentException("Katana parameter addresses must contain exactly 4 bytes.", nameof(address));

        if (minimum > maximum)
            throw new ArgumentOutOfRangeException(nameof(minimum), "Minimum must be less than or equal to maximum.");

        Key = key;
        DisplayName = displayName;
        Address = address.ToArray();
        Minimum = minimum;
        Maximum = maximum;
        SkippedValues = skippedValues?.ToArray() ?? [];
        Description = description;
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
    public byte Minimum { get; }

    /// <summary>Auto-generated: byte Maximum { get; }</summary>
    public byte Maximum { get; }

    /// <summary>Auto-generated: IReadOnlyList<byte> SkippedValues { get; }</summary>
    public IReadOnlyList<byte> SkippedValues { get; }

    /// <summary>Auto-generated: string? Description { get; }</summary>
    public string? Description { get; }
}
