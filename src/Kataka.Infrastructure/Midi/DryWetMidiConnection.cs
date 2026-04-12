using System.Collections.Concurrent;
using Kataka.Application.Midi;
using Kataka.Domain.Midi;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace Kataka.Infrastructure.Midi;

/// <summary>
/// Persistent MIDI connection using DryWetMidi.
/// Opens input/output devices once and keeps them alive for the session.
/// Uses an address-keyed TCS correlator for solicited RQ1→DT1 replies,
/// and raises <see cref="PushNotificationReceived"/> for unsolicited DT1 messages
/// sent by the amp when parameters change on the device itself.
/// </summary>
internal sealed class DryWetMidiConnection : IMidiConnection
{
    // Roland DT1 (Data Set 1) command byte at offset 7 in the SysEx reply.
    private const byte DataSetCommand = 0x12;

    // Roland address bytes are always at offset 8 (after F0 41 DeviceID M1 M2 M3 M4 Command).
    private const int AddressOffset = 8;
    private const int AddressLength = 4;

    // Minimum valid DT1 reply: F0 41 DeviceID x4ModelId Command x4Address x1Data Checksum F7 = 15 bytes.
    private const int MinDt1Length = 15;

    private readonly InputDevice inputDevice;
    private readonly OutputDevice outputDevice;

    /// <summary>
    /// Pending RQ1 requests, keyed by the hex address string expected in the DT1 reply.
    /// ConcurrentDictionary because <see cref="OnEventReceived"/> fires on the ALSA background thread.
    /// </summary>
    private readonly ConcurrentDictionary<string, TaskCompletionSource<SysExMessage>> pendingRequests = new();

    /// <inheritdoc />
    public event EventHandler<SysExMessage>? PushNotificationReceived;

    public DryWetMidiConnection(InputDevice inputDevice, OutputDevice outputDevice)
    {
        this.inputDevice = inputDevice;
        this.outputDevice = outputDevice;

        // Subscribe before starting the listener to avoid missing early messages.
        inputDevice.EventReceived += OnEventReceived;
        inputDevice.StartEventsListening();
    }

    public ValueTask DisposeAsync()
    {
        // Unsubscribe first so no new events are processed after this point.
        inputDevice.EventReceived -= OnEventReceived;
        inputDevice.StopEventsListening();

        // Cancel all in-flight requests so their awaiters don't hang.
        foreach (var tcs in pendingRequests.Values)
        {
            tcs.TrySetCanceled();
        }

        pendingRequests.Clear();

        inputDevice.Dispose();
        outputDevice.Dispose();

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Sends a SysEx message to the amp (fire-and-forget, no reply expected).
    /// DryWetMidi's <see cref="NormalSysExEvent"/> data does NOT include the leading 0xF0 byte,
    /// so we strip it before sending.
    /// </summary>
    public Task SendAsync(SysExMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Skip index 0 (0xF0) — DryWetMidi adds it internally.
        var bytes = message.Bytes;
        var data = new byte[bytes.Count - 1];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = bytes[i + 1];
        }

        outputDevice.SendEvent(new NormalSysExEvent(data));
        return Task.CompletedTask;
    }

    public Task SendProgramChangeAsync(byte program, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        outputDevice.SendEvent(new ProgramChangeEvent((SevenBitNumber)program));
        return Task.CompletedTask;
    }

    public Task SendControlChangeAsync(byte control, byte value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        outputDevice.SendEvent(new ControlChangeEvent((SevenBitNumber)control, (SevenBitNumber)value));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends an RQ1 request and awaits the matching DT1 reply.
    /// The reply is matched by the 4-byte address at bytes [8..11] — the same address as in the request.
    /// Throws <see cref="TimeoutException"/> if no reply arrives within <paramref name="timeout"/>.
    /// </summary>
    public async Task<SysExMessage> RequestAsync(
        SysExMessage request,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        // Extract the address bytes from the RQ1 — the DT1 reply will carry the same address.
        var addressKey = ExtractAddressKey(request.Bytes, AddressOffset);

        var tcs = new TaskCompletionSource<SysExMessage>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Register the TCS before sending so we cannot miss a very fast reply.
        if (!pendingRequests.TryAdd(addressKey, tcs))
        {
            throw new InvalidOperationException(
                $"A concurrent RQ1 request for address {addressKey} is already in flight.");
        }

        await SendAsync(request, cancellationToken);

        // Build a linked token that fires on either caller cancellation or timeout.
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCts.CancelAfter(timeout);

        using var registration = linkedCts.Token.Register(() =>
        {
            // Remove from pending so OnEventReceived does not complete a cancelled TCS.
            pendingRequests.TryRemove(addressKey, out _);
            tcs.TrySetCanceled(linkedCts.Token);
        });

        try
        {
            return await tcs.Task;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // Timeout fired, not the caller's token — convert to TimeoutException for clarity.
            throw new TimeoutException(
                $"No MIDI DT1 reply received for address {addressKey} within {timeout.TotalSeconds:F1}s.", ex);
        }
    }

    /// <summary>
    /// Processes all incoming MIDI events on the ALSA background thread.
    /// Solicited DT1 replies complete a pending RQ1 TCS by address.
    /// Unsolicited DT1 messages (amp pushing a parameter change) are raised via <see cref="PushNotificationReceived"/>.
    /// </summary>
    private void OnEventReceived(object? sender, MidiEventReceivedEventArgs e)
    {
        if (e.Event is not NormalSysExEvent sysExEvent)
        {
            return;
        }

        // DryWetMidi strips the leading 0xF0 from received events — add it back.
        var rawData = sysExEvent.Data;
        var fullBytes = new byte[rawData.Length + 1];
        fullBytes[0] = 0xF0;
        Array.Copy(rawData, 0, fullBytes, 1, rawData.Length);

        var message = new SysExMessage(fullBytes);
        var bytes = message.Bytes;

        // Validate: must be long enough and carry the Roland DT1 command byte.
        if (bytes.Count < MinDt1Length || bytes[7] != DataSetCommand)
        {
            return;
        }

        var addressKey = ExtractAddressKey(bytes, AddressOffset);

        // Solicited reply: complete the registered TCS and return.
        if (pendingRequests.TryRemove(addressKey, out var tcs))
        {
            tcs.TrySetResult(message);
            return;
        }

        // Unsolicited push: the amp changed a parameter (user turned a knob, toggled a pedal switch, etc.).
        // Callers subscribed to PushNotificationReceived are responsible for marshalling to the UI thread.
        PushNotificationReceived?.Invoke(this, message);
    }

    /// <summary>
    /// Formats 4 consecutive bytes starting at <paramref name="offset"/> as a hex address key string,
    /// e.g. "60-00-06-20". Used as the correlation key between RQ1 requests and DT1 replies.
    /// </summary>
    private static string ExtractAddressKey(IReadOnlyList<byte> bytes, int offset) =>
        $"{bytes[offset]:X2}-{bytes[offset + 1]:X2}-{bytes[offset + 2]:X2}-{bytes[offset + 3]:X2}";
}
