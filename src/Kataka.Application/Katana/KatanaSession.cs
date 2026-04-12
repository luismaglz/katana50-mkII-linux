using Kataka.Application.Midi;
using Kataka.Domain.Midi;

namespace Kataka.Application.Katana;

public sealed class KatanaSession(IMidiTransport midiTransport) : IKatanaSession
{
    private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(2);
    private static readonly int MidiBytesPerSecond = 3125;           // MIDI 1.0 wire speed at 31250 baud
    private static readonly TimeSpan DeviceInterval = TimeSpan.FromMilliseconds(20); // BTS ProductSetting.interval

    // Special address for the Boss editor communication mode flag.
    // Writing 0x01 tells the amp to start sending unsolicited DT1 push notifications for all parameter changes.
    // Writing 0x00 tells the amp to stop. See BTS address_const.js: EDITOR_COMMUNICATION_MODE = 0x7f000001.
    private static readonly byte[] EditorCommunicationModeAddress = [0x7F, 0x00, 0x00, 0x01];

    private static readonly byte[] CurrentChannelAddress = [0x00, 0x01, 0x00, 0x00];
    private static readonly byte[] CurrentChannelSize = [0x00, 0x00, 0x00, 0x02];
    private readonly IMidiTransport midiTransport = midiTransport;
    private IMidiConnection? activeConnection;
    private DateTimeOffset _nextSendAllowedAt = DateTimeOffset.MinValue;

    public bool IsConnected => activeConnection is not null;

    /// <inheritdoc />
    public event EventHandler<SysExMessage>? PushNotificationReceived;

    public Task<IReadOnlyList<MidiPortDescriptor>> ListPortsAsync(CancellationToken cancellationToken = default) =>
        midiTransport.ListPortsAsync(cancellationToken);

    public async Task ConnectAsync(string inputPortId, string outputPortId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPortId);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPortId);

        await DisconnectAsync();
        activeConnection = await midiTransport.OpenAsync(inputPortId, outputPortId, cancellationToken);

        // Forward push notifications from the connection to session subscribers.
        activeConnection.PushNotificationReceived += OnConnectionPushNotification;

        // Tell the amp to start sending unsolicited DT1 messages whenever any parameter changes.
        // This mirrors the BTS startCommunication() handshake (EDITOR_COMMUNICATION_MODE = 1).
        await WriteEditorCommunicationModeAsync(enable: true, cancellationToken);
    }

    public async Task DisconnectAsync()
    {
        if (activeConnection is null)
        {
            return;
        }

        // Unsubscribe before disposing to prevent callbacks on a dead connection.
        activeConnection.PushNotificationReceived -= OnConnectionPushNotification;

        // Tell the amp to stop sending push notifications before closing the port.
        try
        {
            await WriteEditorCommunicationModeAsync(enable: false, CancellationToken.None);
        }
        catch
        {
            // Best-effort — the port may already be closing.
        }

        await activeConnection.DisposeAsync();
        activeConnection = null;
    }

    public async Task<SysExMessage> RequestIdentityAsync(CancellationToken cancellationToken = default)
    {
        var message = UniversalDeviceIdentity.CreateIdentityRequest();
        await EnforcePacingAsync(message.Bytes.Count, cancellationToken);
        return await RequireConnection().RequestAsync(message, DefaultRequestTimeout, cancellationToken);
    }

    public async Task<KatanaPanelChannel?> ReadCurrentPanelChannelAsync(CancellationToken cancellationToken = default)
    {
        var request = RolandSysExBuilder.BuildDataRequest1(0x00, [0x00, 0x00, 0x00, 0x33], CurrentChannelAddress, CurrentChannelSize);
        await EnforcePacingAsync(request.Bytes.Count, cancellationToken);
        var reply = await RequireConnection().RequestAsync(request, DefaultRequestTimeout, cancellationToken);

        if (!TryParseCurrentPanelChannel(reply, out var channel))
        {
            return null;
        }

        return channel;
    }

    public async Task SelectPanelChannelAsync(KatanaPanelChannel channel, CancellationToken cancellationToken = default)
    {
        await EnforcePacingAsync(2, cancellationToken); // MIDI program change = 2 bytes
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
            var request = RolandSysExBuilder.BuildDataRequest1(0x00, [0x00, 0x00, 0x00, 0x33], startAddress, size);
            await EnforcePacingAsync(request.Bytes.Count, cancellationToken);
            var reply = await RequireConnection().RequestAsync(request, DefaultRequestTimeout, cancellationToken);

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

        var writeMessage = KatanaMkIIProtocol.CreateParameterWriteRequest(parameter, value);
        await EnforcePacingAsync(writeMessage.Bytes.Count, cancellationToken);
        await RequireConnection().SendAsync(writeMessage, cancellationToken);

        return await ReadParameterAsync(parameter, cancellationToken);
    }

    public async Task<byte[]> ReadBlockAsync(
        IReadOnlyList<byte> address,
        int length,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(address);

        var request = KatanaMkIIProtocol.CreateDataReadRequest(address, length);
        await EnforcePacingAsync(request.Bytes.Count, cancellationToken);
        var reply = await RequireConnection().RequestAsync(request, DefaultRequestTimeout, cancellationToken);

        if (!KatanaMkIIProtocol.TryParseParameterBlockReply(address, length, reply, out var data))
        {
            throw new InvalidOperationException("A Katana data block reply did not match the expected MKII format.");
        }

        return data;
    }

    public async Task WriteBlockAsync(
        IReadOnlyList<byte> address,
        IReadOnlyList<byte> data,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(data);

        var message = KatanaMkIIProtocol.CreateDataWriteRequest(address, data);
        await EnforcePacingAsync(message.Bytes.Count, cancellationToken);
        await RequireConnection().SendAsync(message, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }

    private IMidiConnection RequireConnection()
    {
        return activeConnection ?? throw new InvalidOperationException("No Katana MIDI connection is currently open.");
    }

    /// <summary>
    /// Forwards unsolicited DT1 push messages from the active connection to session subscribers.
    /// </summary>
    private void OnConnectionPushNotification(object? sender, SysExMessage message)
    {
        PushNotificationReceived?.Invoke(this, message);
    }

    /// <summary>
    /// Writes the EDITOR_COMMUNICATION_MODE flag to the amp.
    /// When enabled (value 1), the amp sends unsolicited DT1 messages for all live parameter changes.
    /// When disabled (value 0), the amp stops sending push notifications.
    /// </summary>
    private async Task WriteEditorCommunicationModeAsync(bool enable, CancellationToken cancellationToken)
    {
        var message = KatanaMkIIProtocol.CreateDataWriteRequest(
            EditorCommunicationModeAddress,
            [enable ? (byte)0x01 : (byte)0x00]);
        await EnforcePacingAsync(message.Bytes.Count, cancellationToken);
        await RequireConnection().SendAsync(message, cancellationToken);
    }

    /// <summary>
    /// Enforces BTS-style byte-rate inter-message pacing before every MIDI send or request.
    /// Formula: delay = ceil(msgBytes * 1000 / 3125) + 20ms (Boss ProductSetting.interval).
    /// </summary>
    private async Task EnforcePacingAsync(int messageBytes, CancellationToken ct = default)
    {
        var delay = _nextSendAllowedAt - DateTimeOffset.UtcNow;
        if (delay > TimeSpan.Zero)
            await Task.Delay(delay, ct);
        var byteRateMs = (int)Math.Ceiling(messageBytes * 1000.0 / MidiBytesPerSecond);
        _nextSendAllowedAt = DateTimeOffset.UtcNow + TimeSpan.FromMilliseconds(byteRateMs) + DeviceInterval;
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
        // BTS uses SYSEX_MAXLEN=128; 32 is conservative and hardware-safe.
        // With span=32, variation colors (0x39-3D) + ChainPattern (0x20) batch into 1 read.
        private const int MaximumSpanLength = 32;
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
