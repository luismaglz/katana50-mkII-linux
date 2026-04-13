using System.Text.Json;
using System.Text.Json.Nodes;

namespace Kataka.Application.Katana;

/// <summary>
/// Reads and writes Boss Tone Studio .tsl patch files (format rev 0002, KATANA MkII).
/// </summary>
public static class TslPatchSerializer
{
    private const string TslKeyPrefix = "UserPatch%";

    public static TslPatch Deserialize(string json)
    {
        var root = JsonNode.Parse(json) ?? throw new InvalidDataException("Invalid TSL JSON.");

        var patch = new TslPatch
        {
            Name = root["name"]?.GetValue<string>() ?? "Unknown",
            FormatRev = root["formatRev"]?.GetValue<string>() ?? "0002",
            Device = root["device"]?.GetValue<string>() ?? "KATANA MkII",
        };

        var paramSet = root["data"]?[0]?[0]?["paramSet"]?.AsObject()
            ?? throw new InvalidDataException("TSL file missing data/paramSet.");

        patch.Memo = root["data"]?[0]?[0]?["memo"]?.GetValue<string>() ?? string.Empty;

        foreach (var (key, node) in paramSet)
        {
            if (!key.StartsWith(TslKeyPrefix, StringComparison.Ordinal)) continue;
            var blockName = key[TslKeyPrefix.Length..];

            if (node is not JsonArray arr) continue;
            var bytes = arr.Select(b => Convert.ToByte(b?.GetValue<string>(), 16)).ToArray();
            patch.Blocks[blockName] = bytes;
        }

        return patch;
    }

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
                    new JsonObject
                    {
                        ["memo"] = patch.Memo,
                        ["paramSet"] = paramSet,
                    }
                )
            ),
        };

        return root.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
    }
}
