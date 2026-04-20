namespace Kataka.App.KatanaState;

/// <summary>
///     A state object that owns a set of addresses and buffers their bytes.
///     Used for multi-byte values (e.g. patch name) where a single logical value
///     spans several sequential amp addresses.
///     <see cref="RegisterAll" /> detects this interface and registers all
///     <see cref="AddressKeys" /> into the multi-address state dictionary.
/// </summary>
public interface IMultiAddressState
{
    /// <summary>All address keys this state owns, in logical order (e.g. char 0 first).</summary>
    IReadOnlyList<string> AddressKeys { get; }

    /// <summary>Store one byte and fire any change event when the complete value is received.</summary>
    void SetByte(string addressKey, byte value);
}
