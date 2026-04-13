using Kataka.Domain.Midi;

namespace Kataka.Application.Midi;

public interface IMidiConnection : IAsyncDisposable
{
    /// <summary>
    /// Sends a SysEx message to the device (fire-and-forget, no reply expected).
    /// </summary>
    Task SendAsync(SysExMessage message, CancellationToken cancellationToken = default);

    Task SendProgramChangeAsync(byte program, CancellationToken cancellationToken = default);

    Task SendControlChangeAsync(byte control, byte value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an RQ1 request and awaits the matching DT1 reply, correlated by address.
    /// </summary>
    Task<SysExMessage> RequestAsync(
        SysExMessage request,
        TimeSpan timeout,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when the device sends an unsolicited DT1 message (e.g., a parameter changed on the amp).
    /// Fired on the MIDI receive thread — subscribers must marshal to the UI thread if needed.
    /// </summary>
    event EventHandler<SysExMessage>? PushNotificationReceived;

    /// <summary>
    /// Raised when the device sends a Program Change message (e.g., the user pressed a channel button
    /// on the amp). The event argument is the raw program number byte (0–127).
    /// Fired on the MIDI receive thread — subscribers must marshal to the UI thread if needed.
    /// </summary>
    event EventHandler<byte>? ProgramChangeReceived;
}
