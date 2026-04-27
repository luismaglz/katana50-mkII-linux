using System.Collections.ObjectModel;

using Avalonia.Platform.Storage;

using CommunityToolkit.Mvvm.Input;

using Kataka.App.Services;

using Microsoft.Extensions.Logging;

using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class PatchLibraryViewModel : ViewModelBase
{
    private readonly IPatchLibraryService _service;
    private readonly ILogger<PatchLibraryViewModel> _logger;
    private bool _programmaticSelection;

    public PatchLibraryViewModel(IPatchLibraryService service, ILogger<PatchLibraryViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    public IStorageProvider? StorageProvider { get; set; }

    public ObservableCollection<string> PatchNames { get; } = [];
    [Reactive] public int SelectedIndex { get; set; } = -1;
    [Reactive] public bool HasPatches { get; private set; }
    [Reactive] public string LoadedFileName { get; private set; } = string.Empty;

    [RelayCommand]
    private async Task LoadFileAsync()
    {
        if (StorageProvider is null) return;

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Patch Collection",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Boss Tone Studio Patch") { Patterns = ["*.tsl"] },
                new FilePickerFileType("All Files") { Patterns = ["*"] }
            ]
        });

        if (files.Count == 0) return;

        var file = files[0];
        try
        {
            string json;
            await using (var stream = await file.OpenReadAsync())
            using (var reader = new StreamReader(stream))
                json = await reader.ReadToEndAsync();

            await _service.LoadFromJsonAsync(json);

            PatchNames.Clear();
            foreach (var name in _service.PatchNames)
                PatchNames.Add(name);

            LoadedFileName = _service.LoadedFileName ?? file.Name;
            HasPatches = PatchNames.Count > 0;

            if (PatchNames.Count > 0)
            {
                _programmaticSelection = true;
                SelectedIndex = 0;
                _programmaticSelection = false;
                await _service.ApplyPatchAsync(0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load patch file {Name}", file.Name);
        }
    }

    [RelayCommand]
    private async Task SelectPatchAsync(int index)
    {
        if (_programmaticSelection || index < 0) return;
        try
        {
            await _service.ApplyPatchAsync(index);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply patch at index {Index}", index);
        }
    }
}
