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

        var values = await ReadParametersAsync([parameter], cancellationToken);
        return values[parameter.Key];
    }

    public async Task<IReadOnlyDictionary<string, byte>> ReadParametersAsync(
        IReadOnlyList<KatanaParameterDefinition> parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var result = new Dictionary<string, byte>(StringComparer.Ordinal);
        foreach (var group in CreateReadGroups(parameters))
        {
            var startAddress = group.StartAddress;
            var size = new byte[] { 0x00, 0x00, 0x00, (byte)group.Length };
            var reply = await RequireConnection().RequestAsync(
                RolandSysExBuilder.BuildDataRequest1(0x00, [0x00, 0x00, 0x00, 0x33], startAddress, size),
                DefaultRequestTimeout,
                cancellationToken);

            if (!KatanaMkIIProtocol.TryParseParameterBlockReply(startAddress, group.Length, reply, out var data))
            {
                throw new InvalidOperationException("A batched Katana parameter reply did not match the expected MKII format.");
            }

            foreach (var parameter in group.Parameters)
            {
                var offset = parameter.Address[3] - startAddress[3];
                result[parameter.Key] = data[offset];
            }
        }

        return result;
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

    private static IReadOnlyList<ParameterReadGroup> CreateReadGroups(IReadOnlyList<KatanaParameterDefinition> parameters)
    {
        var ordered = parameters
            .DistinctBy(parameter => parameter.Key)
            .OrderBy(parameter => parameter.Address[0])
            .ThenBy(parameter => parameter.Address[1])
            .ThenBy(parameter => parameter.Address[2])
            .ThenBy(parameter => parameter.Address[3])
            .ToList();

        var groups = new List<ParameterReadGroup>();
        foreach (var parameter in ordered)
        {
            if (groups.Count == 0 || !groups[^1].CanInclude(parameter))
            {
                groups.Add(new ParameterReadGroup(parameter));
                continue;
            }

            groups[^1].Add(parameter);
        }

        return groups;
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

    private sealed class ParameterReadGroup
    {
        private const int MaximumSpanLength = 8;
        private readonly List<KatanaParameterDefinition> parameters = [];
        private readonly byte[] prefix = new byte[3];
        private byte startOffset;
        private byte endOffset;

        public ParameterReadGroup(KatanaParameterDefinition parameter)
        {
            prefix[0] = parameter.Address[0];
            prefix[1] = parameter.Address[1];
            prefix[2] = parameter.Address[2];
            startOffset = parameter.Address[3];
            endOffset = parameter.Address[3];
            parameters.Add(parameter);
        }

        public IReadOnlyList<KatanaParameterDefinition> Parameters => parameters;

        public int Length => endOffset - startOffset + 1;

        public byte[] StartAddress => [prefix[0], prefix[1], prefix[2], startOffset];

        public bool CanInclude(KatanaParameterDefinition parameter)
        {
            return parameter.Address[0] == prefix[0] &&
                   parameter.Address[1] == prefix[1] &&
                   parameter.Address[2] == prefix[2] &&
                   parameter.Address[3] - startOffset + 1 <= MaximumSpanLength;
        }

        public void Add(KatanaParameterDefinition parameter)
        {
            parameters.Add(parameter);
            endOffset = parameter.Address[3];
        }
    }
}
