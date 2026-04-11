using Kataka.Application.Midi;
using Kataka.Domain.Midi;

namespace Kataka.Application.Katana;

public sealed class KatanaSession(IMidiTransport midiTransport) : IKatanaSession
{
    private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(1.5);
    private static readonly byte[] CurrentChannelAddress = [0x00, 0x01, 0x00, 0x00];
    private static readonly byte[] CurrentChannelSize = [0x00, 0x00, 0x00, 0x02];
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

    public async Task<KatanaPanelChannel?> ReadCurrentPanelChannelAsync(CancellationToken cancellationToken = default)
    {
        var reply = await RequireConnection().RequestAsync(
            RolandSysExBuilder.BuildDataRequest1(0x00, [0x00, 0x00, 0x00, 0x33], CurrentChannelAddress, CurrentChannelSize),
            DefaultRequestTimeout,
            cancellationToken);

        if (!TryParseCurrentPanelChannel(reply, out var channel))
        {
            return null;
        }

        return channel;
    }

    public async Task SelectPanelChannelAsync(KatanaPanelChannel channel, CancellationToken cancellationToken = default)
    {
        await RequireConnection().SendProgramChangeAsync(ToProgramChange(channel), cancellationToken);
    }

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

    private static bool TryParseCurrentPanelChannel(SysExMessage message, out KatanaPanelChannel channel)
    {
        channel = KatanaPanelChannel.Panel;
        var bytes = message.Bytes;
        if (bytes.Count != 16 ||
            bytes[0] != 0xF0 ||
            bytes[1] != 0x41 ||
            bytes[7] != 0x12 ||
            bytes[8] != 0x00 ||
            bytes[9] != 0x01 ||
            bytes[10] != 0x00 ||
            bytes[11] != 0x00 ||
            bytes[^1] != 0xF7)
        {
            return false;
        }

        return TryMapCurrentChannelCode(bytes[13], out channel);
    }

    private static bool TryMapCurrentChannelCode(byte code, out KatanaPanelChannel channel)
    {
        channel = code switch
        {
            0 => KatanaPanelChannel.Panel,
            1 => KatanaPanelChannel.ChA1,
            2 => KatanaPanelChannel.ChA2,
            5 => KatanaPanelChannel.ChB1,
            6 => KatanaPanelChannel.ChB2,
            _ => KatanaPanelChannel.Panel,
        };

        return code is 0 or 1 or 2 or 5 or 6;
    }

    private static byte ToProgramChange(KatanaPanelChannel channel)
    {
        return channel switch
        {
            KatanaPanelChannel.ChA1 => 0,
            KatanaPanelChannel.ChA2 => 1,
            KatanaPanelChannel.Panel => 4,
            KatanaPanelChannel.ChB1 => 5,
            KatanaPanelChannel.ChB2 => 6,
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, "Unsupported Katana panel channel."),
        };
    }
}
