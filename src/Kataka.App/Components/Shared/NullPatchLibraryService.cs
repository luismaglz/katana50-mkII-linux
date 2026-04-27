using Kataka.App.Services;

namespace Kataka.App.ViewModels.Design;

internal sealed class NullPatchLibraryService : IPatchLibraryService
{
    public string? LoadedFileName => null;
    public IReadOnlyList<string> PatchNames => [];
    public Task LoadFromPathAsync(string path) => Task.CompletedTask;
    public Task LoadFromJsonAsync(string json) => Task.CompletedTask;
    public Task ApplyPatchAsync(int index) => Task.CompletedTask;
}
