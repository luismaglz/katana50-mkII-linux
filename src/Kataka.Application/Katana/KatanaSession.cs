using Kataka.Application.Midi;
using Kataka.Domain.Midi;

namespace Kataka.Application.Katana;

public sealed class KatanaSession(IMidiTransport midiTransport) : IKatanaSession
{
    private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(1.5);
    private readonly IMidiTransport midiTransport = midiTransport;
    private IMidiConnection? activeConnection;

    public bool IsConnected => activeConnection is not null;

    public Task<IReadOnlyList<MidiPortDescriptor>> ListPortsAsync(CancellationToken cancellationToken = default) =>
        midiTransport.ListPortsAsync(cancellationToken);

    public async Task ConnectAsync(string inputPortId, string outputPortId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPortId);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPortId);

        await DisconnectAsync();
        activeConnection = await midiTransport.OpenAsync(inputPortId, outputPortId, cancellationToken);
    }

    public async Task DisconnectAsync()
    {
        if (activeConnection is null)
        {
            return;
        }

        await activeConnection.DisposeAsync();
        activeConnection = null;
    }

    public Task<SysExMessage> RequestIdentityAsync(CancellationToken cancellationToken = default) =>
        RequireConnection().RequestAsync(UniversalDeviceIdentity.CreateIdentityRequest(), DefaultRequestTimeout, cancellationToken);

    public async Task<byte> ReadParameterAsync(KatanaParameterDefinition parameter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        var reply = await RequireConnection().RequestAsync(
            KatanaMkIIProtocol.CreateParameterReadRequest(parameter),
            DefaultRequestTimeout,
            cancellationToken);

        if (!KatanaMkIIProtocol.TryParseParameterReply(parameter, reply, out var value))
        {
            throw new InvalidOperationException(
                $"{parameter.DisplayName} reply did not match the expected Katana MKII format.");
        }

        return value;
    }

    public async Task<byte> WriteParameterAsync(
        KatanaParameterDefinition parameter,
        byte value,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        await RequireConnection().SendAsync(
            KatanaMkIIProtocol.CreateParameterWriteRequest(parameter, value),
            cancellationToken);

        return await ReadParameterAsync(parameter, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }

    private IMidiConnection RequireConnection()
    {
        return activeConnection ?? throw new InvalidOperationException("No Katana MIDI connection is currently open.");
    }
}
