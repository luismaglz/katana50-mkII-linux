using System.Linq;

namespace Kataka.Domain.Midi;

public sealed class KatanaParameterDefinition
{
    public KatanaParameterDefinition(
        string key,
        string displayName,
        IReadOnlyList<byte> address,
        byte minimum = 0,
        byte maximum = 100,
        IReadOnlyList<byte>? skippedValues = null)
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
    }

    public string Key { get; }

    public string DisplayName { get; }

    public IReadOnlyList<byte> Address { get; }

    public string AddressString => string.Join("-", Address.Select(b => b.ToString("X2")));

    public byte Minimum { get; }

    public byte Maximum { get; }

    public IReadOnlyList<byte> SkippedValues { get; }
}
