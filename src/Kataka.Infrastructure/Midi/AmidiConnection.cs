using System.Diagnostics;
using System.Globalization;
using Kataka.Application.Midi;
using Kataka.Domain.Midi;

namespace Kataka.Infrastructure.Midi;

internal sealed class AmidiConnection(string inputPortId, string outputPortId) : IMidiConnection
{
    public string InputPortId { get; } = inputPortId;

    public string OutputPortId { get; } = outputPortId;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task SendAsync(SysExMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        EnsureSinglePort();
        return RunAmidiAsync(
            $"-p {Quote(OutputPortId)} -S {Quote(message.ToHexString())}",
            cancellationToken);
    }

    public async Task<SysExMessage> RequestAsync(
        SysExMessage request,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        EnsureSinglePort();

        var tempFile = Path.GetTempFileName();
        try
        {
            var timeoutSeconds = Math.Max(timeout.TotalSeconds, 1);
            await RunAmidiAsync(
                $"-p {Quote(OutputPortId)} -S {Quote(request.ToHexString())} -r {Quote(tempFile)} -t {timeoutSeconds.ToString("0.###", CultureInfo.InvariantCulture)}",
                cancellationToken);

            var responseBytes = await File.ReadAllBytesAsync(tempFile, cancellationToken);
            if (responseBytes.Length == 0)
            {
                throw new TimeoutException("No SysEx reply was received before the timeout elapsed.");
            }

            var messages = SplitSysExMessages(responseBytes);
            if (messages.Count == 0)
            {
                throw new InvalidOperationException("amidi returned data, but no complete SysEx frame could be extracted.");
            }

            return new SysExMessage(messages[^1]);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    private static async Task RunAmidiAsync(string arguments, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "amidi",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = startInfo };
        if (!process.Start())
        {
            throw new InvalidOperationException("Failed to start amidi.");
        }

        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"amidi failed with exit code {process.ExitCode}: {string.Join(Environment.NewLine, new[] { stdout, stderr }.Where(text => !string.IsNullOrWhiteSpace(text)))}".Trim());
        }
    }

    private void EnsureSinglePort()
    {
        if (!string.Equals(InputPortId, OutputPortId, StringComparison.Ordinal))
        {
            throw new NotSupportedException("The current amidi transport expects the same ALSA raw MIDI device for input and output.");
        }
    }

    private static string Quote(string value)
    {
        return $"\"{value.Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
    }

    private static List<byte[]> SplitSysExMessages(byte[] bytes)
    {
        var messages = new List<byte[]>();

        for (var index = 0; index < bytes.Length; index++)
        {
            if (bytes[index] != 0xF0)
            {
                continue;
            }

            var start = index;
            while (index < bytes.Length && bytes[index] != 0xF7)
            {
                index++;
            }

            if (index >= bytes.Length)
            {
                break;
            }

            var length = index - start + 1;
            var message = new byte[length];
            Array.Copy(bytes, start, message, 0, length);
            messages.Add(message);
        }

        return messages;
    }
}
