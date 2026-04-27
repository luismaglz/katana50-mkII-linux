namespace Kataka.App.Services;

public interface IPatchLibraryService
{
    string? LoadedFileName { get; }
    IReadOnlyList<string> PatchNames { get; }
    Task LoadFromPathAsync(string path);
    Task LoadFromJsonAsync(string json);
    Task ApplyPatchAsync(int index);
}
