using System.Text.Json;
using System.Text.Json.Nodes;

namespace Kataka.App.Services;

/// <summary>
///     Reads and writes Boss Tone Studio .tsl patch files (format rev 0002, KATANA MkII).
/// </summary>
public static class TslPatchSerializer
{
    private const string TslKeyPrefix = "UserPatch%";

    public static TslPatchCollection DeserializeAll(string json)
    {
        var root = JsonNode.Parse(json) ?? throw new InvalidDataException("Invalid TSL JSON.");
        var fileName = root["name"]?.GetValue<string>() ?? "Unknown";
        var formatRev = root["formatRev"]?.GetValue<string>() ?? "0002";
        var device = root["device"]?.GetValue<string>() ?? "KATANA MkII";

        var dataArray = root["data"]?[0]?.AsArray()
                        ?? throw new InvalidDataException("TSL file missing data array.");

        var patches = new List<TslPatch>();
        foreach (var patchNode in dataArray)
        {
            if (patchNode is null) continue;
            var paramSet = patchNode["paramSet"]?.AsObject();
            if (paramSet is null) continue;

            var patch = new TslPatch { FormatRev = formatRev, Device = device };

            var memoNode = patchNode["memo"];
            patch.Memo = memoNode switch
            {
                JsonValue v => v.TryGetValue<string>(out var s) ? s : string.Empty,
                JsonObject obj => obj["memo"]?.GetValue<string>() ?? string.Empty,
                _ => string.Empty
            };

            foreach (var (key, node) in paramSet)
            {
                if (!key.StartsWith(TslKeyPrefix, StringComparison.Ordinal)) continue;
                var blockName = key[TslKeyPrefix.Length..];
                if (node is not JsonArray arr) continue;
                var bytes = arr.Select(b => Convert.ToByte(b?.GetValue<string>(), 16)).ToArray();
                patch.Blocks[blockName] = bytes;
            }

            patch.Name = patch.Blocks.TryGetValue("PatchName", out var nameBytes)
                ? new string(nameBytes.Select(b => (char)b).ToArray()).TrimEnd()
                : fileName;

            patches.Add(patch);
        }

        return new TslPatchCollection { FileName = fileName, Patches = patches };
    }

    public static TslPatch Deserialize(string json) => DeserializeAll(json).Patches.FirstOrDefault()
        ?? throw new InvalidDataException("TSL file contains no patches.");

    public static string Serialize(TslPatch patch)
    {
        var paramSet = new JsonObject();
        foreach (var (blockName, bytes) in patch.Blocks)
        {
            var arr = new JsonArray();
            foreach (var b in bytes)
                arr.Add(b.ToString("X2"));
            paramSet[TslKeyPrefix + blockName] = arr;
        }

        var root = new JsonObject
        {
            ["name"] = patch.Name,
            ["formatRev"] = patch.FormatRev,
            ["device"] = patch.Device,
            ["data"] = new JsonArray(
                new JsonArray(
                    new JsonObject { ["memo"] = patch.Memo, ["paramSet"] = paramSet }
                )
            )
        };

        return root.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
    }
}
