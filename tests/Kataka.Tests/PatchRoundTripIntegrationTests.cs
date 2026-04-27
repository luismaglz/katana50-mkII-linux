using System.IO.Abstractions.TestingHelpers;

using Kataka.App.KatanaState;
using Kataka.App.Services;
using Kataka.Infrastructure.Midi;

namespace Kataka.Tests;

/// <summary>
///     Requires a connected Katana amp. Excluded from CI runs via --filter "FullyQualifiedName!~Integration".
///     Run with: dotnet test --filter "FullyQualifiedName~Integration"
/// </summary>
public sealed class PatchRoundTripIntegrationTests
{
    private const string FixturePath = "/fixture/single-patch.tsl";

    [Fact]
    public async Task LoadAndApplyPatch_RoundTrip_BlockValuesMatch()
    {
        var fixtureJson = await File.ReadAllTextAsync(
            Path.Combine(AppContext.BaseDirectory, "Fixtures", "single-patch.tsl"));

        var mockFs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [FixturePath] = new MockFileData(fixtureJson)
        });

        var katanaState = new KatanaState(Microsoft.Extensions.Logging.Abstractions.NullLogger<KatanaState>.Instance);

        var transport = DefaultMidiTransport.Create();
        var session = new KatanaSession(transport);

        var ports = await session.ListPortsAsync();
        var katanaPort = ports.FirstOrDefault(p => p.Name.Contains("KATANA", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(katanaPort);

        await session.ConnectAsync(katanaPort.Id, katanaPort.Id);

        try
        {
            var service = new PatchLibraryService(katanaState, mockFs);
            await service.LoadFromPathAsync(FixturePath);

            var inputPatch = TslPatchSerializer.DeserializeAll(fixtureJson).Patches[0];

            await session.LoadPatchAsync(inputPatch);

            var outputPatch = await session.ReadCurrentPatchAsync("round-trip");

            // Only Patch_0 is compared: it contains the core preamp parameters (gain, volume, EQ, cab, chain)
            // and is byte-stable across the load/read round-trip. Other blocks (Fx, Patch_1, Delay, Status,
            // assignment blocks) may be partially normalized by the amp firmware when it receives them.
            var stableBlocks = new HashSet<string>(StringComparer.Ordinal) { "Patch_0" };

            var diffs = new List<string>();
            foreach (var block in KatanaPatchBlocks.All.Where(b => stableBlocks.Contains(b.TslKey)))
            {
                if (!inputPatch.Blocks.TryGetValue(block.TslKey, out var inputBytes)) continue;
                if (!outputPatch.Blocks.TryGetValue(block.TslKey, out var outputBytes)) continue;

                for (var i = 0; i < Math.Min(inputBytes.Length, outputBytes.Length); i++)
                {
                    if (inputBytes[i] != outputBytes[i])
                        diffs.Add($"{block.TslKey}[{i}]: expected 0x{inputBytes[i]:X2}, got 0x{outputBytes[i]:X2}");
                }
            }

            Assert.True(diffs.Count == 0, $"Block mismatches:\n{string.Join("\n", diffs)}");
        }
        finally
        {
            await session.DisconnectAsync();
            await session.DisposeAsync();
        }
    }
}
