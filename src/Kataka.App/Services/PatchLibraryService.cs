using System.IO.Abstractions;

using Kataka.App.KatanaState;

namespace Kataka.App.Services;

public sealed class PatchLibraryService : IPatchLibraryService
{
    private readonly IKatanaState _katanaState;
    private readonly IFileSystem _fileSystem;
    private readonly List<(string Name, IReadOnlyDictionary<string, byte> Values)> _patches = [];

    public PatchLibraryService(IKatanaState katanaState, IFileSystem fileSystem)
    {
        _katanaState = katanaState;
        _fileSystem = fileSystem;
    }

    public string? LoadedFileName { get; private set; }

    public IReadOnlyList<string> PatchNames => _patches.Select(p => p.Name).ToList();

    public async Task LoadFromPathAsync(string path)
    {
        var json = await _fileSystem.File.ReadAllTextAsync(path);
        await LoadFromJsonAsync(json);
        LoadedFileName = _fileSystem.Path.GetFileNameWithoutExtension(path);
    }

    public Task LoadFromJsonAsync(string json)
    {
        var collection = TslPatchSerializer.DeserializeAll(json);
        LoadedFileName = collection.FileName;
        _patches.Clear();
        foreach (var patch in collection.Patches)
            _patches.Add((patch.Name, ExpandPatch(patch)));
        return Task.CompletedTask;
    }

    public Task ApplyPatchAsync(int index)
    {
        if (index >= 0 && index < _patches.Count)
            _katanaState.ApplyPatchValues(_patches[index].Values);
        return Task.CompletedTask;
    }

    internal static IReadOnlyDictionary<string, byte> ExpandPatch(TslPatch patch)
    {
        var result = new Dictionary<string, byte>(StringComparer.Ordinal);
        foreach (var block in KatanaPatchBlocks.All)
        {
            if (!patch.Blocks.TryGetValue(block.TslKey, out var data)) continue;
            for (var i = 0; i < data.Length; i++)
                result[Utilities.AddressToKey(Utilities.AddressOffset(block.Address, i))] = data[i];
        }
        return result;
    }

    internal static TslPatch CollapsePatch(IKatanaState state, string name = "")
    {
        var patch = new TslPatch { Name = name };
        var allStates = state.GetAllRegisteredStates();

        foreach (var block in KatanaPatchBlocks.All)
        {
            var blockData = new byte[block.Size];
            var baseAddr = block.Address;
            var hasAnyData = false;

            for (var i = 0; i < block.Size; i++)
            {
                var addrKey = Utilities.AddressToKey(Utilities.AddressOffset(baseAddr, i));
                if (!allStates.TryGetValue(addrKey, out var ampState)) continue;

                hasAnyData = true;
                var writeBytes = ampState.GetWriteBytes();
                blockData[i] = writeBytes[0];
                if (writeBytes.Length == 2 && i + 1 < block.Size)
                    blockData[i + 1] = writeBytes[1];
            }

            if (hasAnyData)
                patch.Blocks[block.TslKey] = blockData;
        }

        return patch;
    }
}
