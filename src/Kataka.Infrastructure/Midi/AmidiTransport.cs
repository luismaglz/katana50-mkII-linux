using System.Diagnostics;
using System.Runtime.InteropServices;
using Kataka.Application.Midi;
using Kataka.Domain.Midi;

namespace Kataka.Infrastructure.Midi;

public sealed class AmidiTransport : IMidiTransport
{
    public async Task<IReadOnlyList<MidiPortDescriptor>> ListPortsAsync(CancellationToken cancellationToken = default)
    {
        var entries = await ReadPortsAsync(cancellationToken);
        var ports = new List<MidiPortDescriptor>();

        foreach (var entry in entries)
        {
            ports.Add(new MidiPortDescriptor($"in:{entry.Id}", entry.DisplayName, MidiPortDirection.Input));
            ports.Add(new MidiPortDescriptor($"out:{entry.Id}", entry.DisplayName, MidiPortDirection.Output));
        }

        return ports;
    }

    public async Task<IMidiConnection> OpenAsync(
        string inputPortId,
        string outputPortId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPortId);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPortId);

        var ports = await ListPortsAsync(cancellationToken);
        var inputExists = ports.Any(port => port.Direction == MidiPortDirection.Input && port.Id == inputPortId);
        var outputExists = ports.Any(port => port.Direction == MidiPortDirection.Output && port.Id == outputPortId);

        if (!inputExists)
        {
            throw new InvalidOperationException($"Input port '{inputPortId}' is no longer available.");
        }

        if (!outputExists)
        {
            throw new InvalidOperationException($"Output port '{outputPortId}' is no longer available.");
        }

        return new AmidiConnection(StripPrefix(inputPortId), StripPrefix(outputPortId));
    }

    public static bool IsSupported()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return false;
        }

        return TryLocateExecutable("amidi");
    }

    private static async Task<IReadOnlyList<AmidiPortEntry>> ReadPortsAsync(CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "amidi",
            Arguments = "-l",
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
                $"amidi -l failed with exit code {process.ExitCode}: {stderr}".Trim());
        }

        return Parse(stdout);
    }

    private static List<AmidiPortEntry> Parse(string output)
    {
        var entries = new List<AmidiPortEntry>();

        foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (line.StartsWith("Dir ", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var parts = line.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length < 3)
            {
                continue;
            }

            var direction = parts[0];
            var id = parts[1];
            var name = parts[2];

            if (!direction.Contains('I') && !direction.Contains('O'))
            {
                continue;
            }

            entries.Add(new AmidiPortEntry(id, $"{name} ({id})"));
        }

        return entries;
    }

    private static string StripPrefix(string portId) =>
        portId.Contains(':', StringComparison.Ordinal)
            ? portId[(portId.IndexOf(':', StringComparison.Ordinal) + 1)..]
            : portId;

    private static bool TryLocateExecutable(string name)
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        return path
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(directory => Path.Combine(directory, name))
            .Any(File.Exists);
    }

    private sealed record AmidiPortEntry(string Id, string DisplayName);
}
