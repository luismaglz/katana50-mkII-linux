namespace Kataka.App.Services;

public sealed class TslPatchCollection
{
    public string FileName { get; init; } = string.Empty;
    public IReadOnlyList<TslPatch> Patches { get; init; } = [];
}
