using System.Text;

using Kataka.App.Services;

namespace Kataka.App.KatanaState;

/// <summary>
///     Buffers the 16 ASCII bytes that form the current patch name (addresses 60-00-00-00 through 60-00-00-0F).
///     Fires <see cref="NameChanged" /> once the last byte is received.
///     The class owns the address order; <see cref="CurrentName" /> is always assembled in <see cref="AddressKeys" /> order.
/// </summary>
public class PatchNameState : IMultiAddressState
{
    private static readonly byte[] BaseAddress = [0x60, 0x00, 0x00, 0x00];
    private const int NameLength = 16;

    private readonly byte[] _bytes = new byte[NameLength];

    public PatchNameState()
    {
        AddressKeys = Enumerable.Range(0, NameLength)
            .Select(i => Utilities.AddressToKey(Utilities.AddressOffset(BaseAddress, i)))
            .ToList();
    }

    public IReadOnlyList<string> AddressKeys { get; }

    public string CurrentName { get; private set; } = string.Empty;

    public event Action? NameChanged;

    public void SetByte(string addressKey, byte value)
    {
        var idx = ((List<string>)AddressKeys).IndexOf(addressKey);
        if (idx < 0) return;

        _bytes[idx] = value;

        // Fire once the last byte arrives. Delivery is always sequential
        // (push notification loop and startup SetStates both iterate in address order).
        if (addressKey != AddressKeys[^1]) return;

        CurrentName = Encoding.ASCII.GetString(_bytes).TrimEnd();
        NameChanged?.Invoke();
    }
}
