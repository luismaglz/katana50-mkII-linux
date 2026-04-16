using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text;

using Kataka.Application.Midi;
using Kataka.Domain.Midi;

namespace Kataka.Infrastructure.Midi;

/// <summary>
/// Persistent MIDI connection that reads and writes directly to an ALSA raw MIDI device file
/// (e.g. <c>/dev/snd/midiC1D0</c>). Requires no native library — the Linux kernel exposes the
/// ALSA device as a plain character device.
///
/// Uses an address-keyed TCS correlator for solicited RQ1→DT1 replies (same pattern as BTS):
/// bytes [8..11] of the outgoing RQ1 are stored as the lookup key; when a DT1 arrives with
/// the same bytes the waiting <see cref="RequestAsync"/> call is completed.
/// Unsolicited DT1 messages (amp pushing live parameter changes) are surfaced via
/// <see cref="PushNotificationReceived"/>.
///
/// <para>
/// The background listener uses a dedicated OS thread and the raw Linux <c>read(2)</c> syscall
/// via P/Invoke. .NET's <c>FileStream.ReadAsync</c> and <c>FileStream.Close</c> do not work
/// correctly with ALSA character devices: async reads never complete, and <c>SafeFileHandle</c>
/// reference-counting prevents <c>close()</c> from interrupting a blocked <c>read()</c>.
/// By owning the file descriptor directly we can call <c>close(fd)</c> from <see cref="DisposeAsync"/>
/// which reliably unblocks the background reader.
/// </para>
/// </summary>
internal sealed partial class AlsaRawMidiConnection : IMidiConnection
{
    /// <summary>
    /// Path of the MIDI receive log written by the background listener.
    /// Every byte received is appended with context annotations so channel-change
    /// experiments can be diagnosed without a debugger.
    /// </summary>
    public static readonly string MidiLogPath = "/tmp/kataka-midi-rx.log";

    private const byte DataSetCommand = 0x12;   // Roland DT1 command byte at offset 7
    private const int AddressOffset = 8;         // address bytes are always at [8..11]
    private const int MinDt1Length = 15;         // F0 41 DevId x4Model Cmd x4Addr x1Data Cksum F7

    // Raw file descriptor managed via P/Invoke so close(fd) can interrupt a blocked read()
    // without SafeFileHandle reference-counting deferral.
    private int _fd = -1;

    private readonly Thread _listenerThread;
    private readonly TaskCompletionSource _listenerExited =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private volatile bool _disposed;

    // Keyed by "XX-XX-XX-XX" hex of the 4 address bytes expected in the DT1 reply.
    private readonly ConcurrentDictionary<string, TaskCompletionSource<SysExMessage>> _pending = new();

    // Serialises concurrent writes — the reader runs independently on its own thread.
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    // Log writer — opened once on construction, flushed after each read burst.
    private readonly StreamWriter _log;

    /// <inheritdoc />
    public event EventHandler<SysExMessage>? PushNotificationReceived;

    /// <inheritdoc />
    public event EventHandler<byte>? ProgramChangeReceived;

    /// <param name="devicePath">Absolute path to the ALSA raw MIDI device, e.g. <c>/dev/snd/midiC1D0</c>.</param>
    public AlsaRawMidiConnection(string devicePath)
    {
        _fd = Libc.open(devicePath, Libc.O_RDWR);
        if (_fd < 0)
        {
            var errno = Marshal.GetLastWin32Error();
            throw new IOException($"Failed to open MIDI device '{devicePath}': errno={errno}");
        }

        _log = new StreamWriter(MidiLogPath, append: false, Encoding.UTF8) { AutoFlush = false };
        _log.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] AlsaRawMidiConnection opened: {devicePath}");

        _listenerThread = new Thread(RunListener)
        {
            Name = "AlsaMidiListener",
            IsBackground = true,
        };
        _listenerThread.Start();
    }

    // ── IMidiConnection ───────────────────────────────────────────────────────

    public async Task SendAsync(SysExMessage message, CancellationToken cancellationToken = default)
    {
        var bytes = message.Bytes.ToArray();
        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            WriteRaw(bytes);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public Task SendProgramChangeAsync(byte program, CancellationToken cancellationToken = default) =>
        WriteRawLockedAsync([(byte)0xC0, program], cancellationToken);

    public Task SendControlChangeAsync(byte control, byte value, CancellationToken cancellationToken = default) =>
        WriteRawLockedAsync([(byte)0xB0, control, value], cancellationToken);

    public async Task<SysExMessage> RequestAsync(
        SysExMessage request,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        var bytes = request.Bytes;
        var addressKey = AddressKey(bytes, AddressOffset);

        var tcs = new TaskCompletionSource<SysExMessage>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Register before sending so a very fast reply is never missed.
        if (!_pending.TryAdd(addressKey, tcs))
        {
            throw new InvalidOperationException(
                $"A concurrent RQ1 request for address {addressKey} is already in flight.");
        }

        await SendAsync(request, cancellationToken);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCts.CancelAfter(timeout);

        using var reg = linkedCts.Token.Register(() =>
        {
            _pending.TryRemove(addressKey, out _);
            tcs.TrySetCanceled(linkedCts.Token);
        });

        try
        {
            return await tcs.Task;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"No MIDI DT1 reply for address {addressKey} within {timeout.TotalSeconds:F1}s.", ex);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        // Close the raw fd directly via P/Invoke. This bypasses SafeFileHandle reference-counting
        // and immediately issues close(2) to the kernel, which wakes up the background thread
        // blocked in read(2) on this fd (ALSA returns EBADF when the fd is closed).
        var fd = Interlocked.Exchange(ref _fd, -1);
        if (fd >= 0)
        {
            Libc.close(fd);
        }

        // Wait for the reader thread to acknowledge the close before clearing state.
        try
        {
            await _listenerExited.Task.WaitAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
        }
        catch (TimeoutException) { /* reader didn't exit cleanly — proceed with cleanup anyway */ }

        foreach (var tcs in _pending.Values)
        {
            tcs.TrySetCanceled();
        }

        _pending.Clear();
        _writeLock.Dispose();

        _log.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] AlsaRawMidiConnection disposed.");
        await _log.DisposeAsync().ConfigureAwait(false);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void WriteRaw(byte[] bytes)
    {
        var fd = _fd;
        if (fd < 0) throw new ObjectDisposedException(nameof(AlsaRawMidiConnection));

        var offset = 0;
        while (offset < bytes.Length)
        {
            var written = (int)Libc.write(fd, bytes, bytes.Length - offset);
            if (written < 0)
            {
                if (_disposed) return;
                var errno = Marshal.GetLastWin32Error();
                throw new IOException($"MIDI write failed: errno={errno}");
            }
            offset += written;
        }
    }

    private async Task WriteRawLockedAsync(byte[] bytes, CancellationToken ct)
    {
        await _writeLock.WaitAsync(ct);
        try { WriteRaw(bytes); }
        finally { _writeLock.Release(); }
    }

    /// <summary>
    /// Blocking reader loop — runs on a dedicated OS thread.
    /// Assembles SysEx frames (F0…F7) and routes each to a pending RQ1 TCS or
    /// fires <see cref="PushNotificationReceived"/> for unsolicited messages.
    /// </summary>
    private void RunListener()
    {
        var buf = new byte[256];
        var frame = new List<byte>(256);
        var awaitingPcByte = false; // true after 0xCx status, waiting for program number
        var lineBuilder = new StringBuilder();

        try
        {
            while (true)
            {
                var fd = _fd;
                if (fd < 0) break;

                var n = (int)Libc.read(fd, buf, buf.Length);

                if (n < 0)
                {
                    var errno = Marshal.GetLastWin32Error();
                    if (_disposed || errno == Libc.EBADF) break;  // fd was closed intentionally
                    if (errno == Libc.EINTR) continue;             // signal interrupted — retry
                    break;                                         // other error
                }

                if (n == 0) break; // EOF

                // Log the raw burst as a hex dump with context annotations.
                lineBuilder.Clear();
                lineBuilder.Append($"[{DateTime.Now:HH:mm:ss.fff}] RX {n,3}B:");
                for (var j = 0; j < n; j++)
                    lineBuilder.Append($" {buf[j]:X2}");
                _log.WriteLine(lineBuilder.ToString());

                for (var i = 0; i < n; i++)
                {
                    var b = buf[i];

                    if (b == 0xF0)
                    {
                        frame.Clear();       // start of new SysEx; discard any partial frame
                        awaitingPcByte = false;
                    }

                    if (frame.Count > 0 || b == 0xF0)
                    {
                        // Inside a SysEx frame — accumulate bytes.
                        frame.Add(b);
                    }
                    else
                    {
                        // Outside SysEx — handle non-SysEx MIDI messages.
                        if ((b & 0xF0) == 0xC0)
                        {
                            // Program Change status byte (channel in low nibble, irrelevant).
                            _log.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]   -> PC status byte: {b:X2} (channel {b & 0x0F})");
                            awaitingPcByte = true;
                        }
                        else if (b >= 0x80)
                        {
                            // Any other status byte resets pending state.
                            _log.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]   -> Other status: {b:X2}");
                            awaitingPcByte = false;
                        }
                        else if (awaitingPcByte)
                        {
                            // Data byte following a Program Change status.
                            _log.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]   -> PC program byte: {b:X2} (decimal {b}) — firing ProgramChangeReceived");
                            awaitingPcByte = false;
                            ProgramChangeReceived?.Invoke(this, b);
                        }
                        else
                        {
                            _log.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]   -> Non-SysEx data (no PC pending): {b:X2}");
                        }
                    }

                    if (b == 0xF7 && frame.Count >= MinDt1Length)
                    {
                        _log.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]   -> SysEx frame complete ({frame.Count}B), firing push/reply");
                        OnFrame(frame.ToArray());
                        frame.Clear();
                    }
                }

                _log.Flush();
            }
        }
        finally
        {
            _log.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] RunListener exiting.");
            _log.Flush();
            _listenerExited.TrySetResult();
        }
    }

    private void OnFrame(byte[] bytes)
    {
        if (bytes[7] != DataSetCommand) return;

        SysExMessage msg;
        try { msg = new SysExMessage(bytes); }
        catch { return; }

        var key = AddressKey(bytes, AddressOffset);

        if (_pending.TryRemove(key, out var tcs))
        {
            tcs.TrySetResult(msg);
        }
        else
        {
            // Unsolicited push — amp changed a parameter (knob turn, switch press, etc.)
            PushNotificationReceived?.Invoke(this, msg);
        }
    }

    private static string AddressKey(IReadOnlyList<byte> bytes, int offset) =>
        $"{bytes[offset]:X2}-{bytes[offset + 1]:X2}-{bytes[offset + 2]:X2}-{bytes[offset + 3]:X2}";

    private static string AddressKey(byte[] bytes, int offset) =>
        $"{bytes[offset]:X2}-{bytes[offset + 1]:X2}-{bytes[offset + 2]:X2}-{bytes[offset + 3]:X2}";

    /// <summary>Linux libc P/Invoke stubs for direct file descriptor control.</summary>
    private static partial class Libc
    {
        public const int O_RDWR = 2;
        public const int EINTR = 4;   // Interrupted system call
        public const int EBADF = 9;   // Bad file descriptor (fd was closed)

        [LibraryImport("libc", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        public static partial int open(string path, int flags);

        [LibraryImport("libc", SetLastError = true)]
        public static partial nint read(int fd, [Out] byte[] buf, nint count);

        [LibraryImport("libc", SetLastError = true)]
        public static partial nint write(int fd, [In] byte[] buf, nint count);

        [LibraryImport("libc", SetLastError = true)]
        public static partial int close(int fd);
    }
}
