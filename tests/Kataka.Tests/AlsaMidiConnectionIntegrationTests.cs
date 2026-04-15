using Kataka.Application.Midi;
using Kataka.Domain.Midi;
using Kataka.Infrastructure.Midi;

namespace Kataka.Tests;

/// <summary>
/// Integration tests that require a Boss Katana MkII to be physically connected via USB.
/// Run with: dotnet test --filter "Category=Integration"
/// </summary>
[Trait("Category", "Integration")]
public sealed class AlsaMidiConnectionIntegrationTests : IAsyncLifetime
{
    private const string KatanaPortName = "KATANA MIDI 1";

    private AmidiTransport _transport = null!;
    private IMidiConnection _connection = null!;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    public async Task InitializeAsync()
    {
        _transport = new AmidiTransport();

        var ports = await _transport.ListPortsAsync();

        var input = ports.FirstOrDefault(p =>
            p.Direction == MidiPortDirection.Input &&
            p.Name.Contains(KatanaPortName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException(
                $"No KATANA MIDI input found. Ensure the amp is connected. Available ports: " +
                string.Join(", ", ports.Select(p => p.Name)));

        var output = ports.FirstOrDefault(p =>
            p.Direction == MidiPortDirection.Output &&
            p.Name.Contains(KatanaPortName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException(
                $"No KATANA MIDI output found. Available ports: " +
                string.Join(", ", ports.Select(p => p.Name)));

        _connection = await _transport.OpenAsync(input.Id, output.Id);
    }

    public async Task DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }

    // ── Port discovery ────────────────────────────────────────────────────────

    [Fact]
    public async Task ListPortsAsync_KatanaConnected_ReturnsInputAndOutputPorts()
    {
        var ports = await _transport.ListPortsAsync();

        Assert.Contains(ports, p =>
            p.Direction == MidiPortDirection.Input &&
            p.Name.Contains(KatanaPortName, StringComparison.OrdinalIgnoreCase));

        Assert.Contains(ports, p =>
            p.Direction == MidiPortDirection.Output &&
            p.Name.Contains(KatanaPortName, StringComparison.OrdinalIgnoreCase));
    }

    // ── Single parameter reads ────────────────────────────────────────────────

    [Fact]
    public async Task RequestAsync_AmpGain_ReturnsDt1Reply()
    {
        var request = KatanaMkIIProtocol.CreateParameterReadRequest(KatanaMkIIParameterCatalog.AmpGain);
        var reply = await _connection.RequestAsync(request, TimeSpan.FromSeconds(5));

        AssertValidDt1(reply);
        AssertAddressMatches(reply, KatanaMkIIParameterCatalog.AmpGain.Address);
    }

    [Fact]
    public async Task RequestAsync_AmpVolume_ReturnsDt1Reply()
    {
        var request = KatanaMkIIProtocol.CreateParameterReadRequest(KatanaMkIIParameterCatalog.AmpVolume);
        var reply = await _connection.RequestAsync(request, TimeSpan.FromSeconds(5));

        AssertValidDt1(reply);
        AssertAddressMatches(reply, KatanaMkIIParameterCatalog.AmpVolume.Address);
    }

    // ── Sequential reads (validates no framing corruption across messages) ───

    [Fact]
    public async Task RequestAsync_AllAmpEditorControls_AllSucceedWithinTimeout()
    {
        foreach (var param in KatanaMkIIParameterCatalog.AmpEditorControls)
        {
            var request = KatanaMkIIProtocol.CreateParameterReadRequest(param);
            var reply = await _connection.RequestAsync(request, TimeSpan.FromSeconds(5));

            AssertValidDt1(reply);
            AssertAddressMatches(reply, param.Address);
        }
    }

    // ── Protocol parsing ──────────────────────────────────────────────────────

    [Fact]
    public async Task RequestAsync_AmpGain_ParsedValueIsInRange()
    {
        var request = KatanaMkIIProtocol.CreateParameterReadRequest(KatanaMkIIParameterCatalog.AmpGain);
        var reply = await _connection.RequestAsync(request, TimeSpan.FromSeconds(5));

        var parsed = KatanaMkIIProtocol.TryParseParameterReply(KatanaMkIIParameterCatalog.AmpGain, reply, out var value);

        Assert.True(parsed, "TryParseParameterReply returned false — reply may be malformed.");
        // Gain is 0-100 on the Katana
        Assert.InRange(value, 0, 100);
    }

    // ── Push notification streaming ───────────────────────────────────────────

    [Fact]
    public async Task PushNotificationReceived_AfterEditorModeEnabled_ReceivesAmpState()
    {
        var received = new List<SysExMessage>();
        var tcs = new TaskCompletionSource<bool>();

        _connection.PushNotificationReceived += (_, msg) =>
        {
            received.Add(msg);
            if (received.Count >= 1)
            {
                tcs.TrySetResult(true);
            }
        };

        // Writing EDITOR_COMMUNICATION_MODE = 1 tells the amp to push live parameter changes.
        var editorModeOn = RolandSysExBuilder.BuildDataSet1(
            deviceId: 0x00,
            modelId: [0x00, 0x00, 0x00, 0x33],
            address: [0x7F, 0x00, 0x00, 0x01],
            data: [0x01]);

        await _connection.SendAsync(editorModeOn);

        // Also read a parameter to provoke any activity on the line
        var request = KatanaMkIIProtocol.CreateParameterReadRequest(KatanaMkIIParameterCatalog.AmpGain);
        await _connection.RequestAsync(request, TimeSpan.FromSeconds(5));

        // Give the amp up to 5s to send at least one unsolicited message
        var gotPush = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(5))) == tcs.Task;

        // Disable editor mode on the way out
        var editorModeOff = RolandSysExBuilder.BuildDataSet1(
            deviceId: 0x00,
            modelId: [0x00, 0x00, 0x00, 0x33],
            address: [0x7F, 0x00, 0x00, 0x01],
            data: [0x00]);
        await _connection.SendAsync(editorModeOff);

        Assert.True(gotPush,
            $"Expected at least one push notification from the amp within 5s, but received {received.Count}.");

        foreach (var msg in received)
        {
            AssertValidDt1(msg);
        }
    }

    [Fact]
    public async Task Streaming_ListenerDoesNotThrow_UnderSequentialLoad()
    {
        // Fire 6 sequential reads — validates framing doesn't break across multiple round-trips
        var parameters = new[]
        {
            KatanaMkIIParameterCatalog.AmpGain,
            KatanaMkIIParameterCatalog.AmpVolume,
            KatanaMkIIParameterCatalog.AmpBass,
            KatanaMkIIParameterCatalog.AmpMiddle,
            KatanaMkIIParameterCatalog.AmpTreble,
            KatanaMkIIParameterCatalog.AmpPresence,
        };

        var exception = await Record.ExceptionAsync(async () =>
        {
            foreach (var param in parameters)
            {
                var request = KatanaMkIIProtocol.CreateParameterReadRequest(param);
                var reply = await _connection.RequestAsync(request, TimeSpan.FromSeconds(5));
                AssertValidDt1(reply);
            }
        });

        Assert.Null(exception);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void AssertValidDt1(SysExMessage msg)
    {
        var bytes = msg.Bytes;
        Assert.Equal(0xF0, bytes[0]);              // SysEx start
        Assert.Equal(0x41, bytes[1]);              // Roland manufacturer ID
        Assert.Equal(0x12, bytes[7]);              // DT1 command
        Assert.Equal(0xF7, bytes[bytes.Count - 1]); // SysEx end
        Assert.True(bytes.Count >= 15, $"DT1 reply too short: {bytes.Count} bytes");
    }

    private static void AssertAddressMatches(SysExMessage reply, IReadOnlyList<byte> expectedAddress)
    {
        var bytes = reply.Bytes;
        Assert.Equal(expectedAddress[0], bytes[8]);
        Assert.Equal(expectedAddress[1], bytes[9]);
        Assert.Equal(expectedAddress[2], bytes[10]);
        Assert.Equal(expectedAddress[3], bytes[11]);
    }
}
