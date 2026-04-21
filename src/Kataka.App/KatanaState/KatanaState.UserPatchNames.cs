namespace Kataka.App.KatanaState;

public partial class KatanaState
{
    private readonly Dictionary<int, string> _userPatchNames = new();

    /// <summary>
    ///     Maps slot index (0-based) to the stored patch name for that slot.
    ///     Updated by <see cref="SetUserPatchName" />; fires <see cref="UserPatchNamesChanged" /> on each update.
    /// </summary>
    public IReadOnlyDictionary<int, string> UserPatchNames => _userPatchNames;

    /// <summary>Fires whenever any entry in <see cref="UserPatchNames" /> is added or updated.</summary>
    public event Action? UserPatchNamesChanged;

    /// <summary>Sets the stored name for <paramref name="slotIndex" /> and fires <see cref="UserPatchNamesChanged" />.</summary>
    public void SetUserPatchName(int slotIndex, string name)
    {
        _userPatchNames[slotIndex] = name;
        UserPatchNamesChanged?.Invoke();
    }
}
