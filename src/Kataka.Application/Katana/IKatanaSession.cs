using Kataka.Domain.Midi;

namespace Kataka.Application.Katana;

public interface IKatanaSession : IAsyncDisposable
{
    bool IsConnected { get; }

    Task<IReadOnlyList<MidiPortDescriptor>> ListPortsAsync(CancellationToken cancellationToken = default);

    Task ConnectAsync(string inputPortId, string outputPortId, CancellationToken cancellationToken = default);

    Task DisconnectAsync();

    Task<SysExMessage> RequestIdentityAsync(CancellationToken cancellationToken = default);

    Task<KatanaPanelChannel?> ReadCurrentPanelChannelAsync(CancellationToken cancellationToken = default);

    Task SelectPanelChannelAsync(KatanaPanelChannel channel, CancellationToken cancellationToken = default);

    Task<byte> ReadParameterAsync(KatanaParameterDefinition parameter, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, byte>> ReadParametersAsync(
        IReadOnlyList<KatanaParameterDefinition> parameters,
        CancellationToken cancellationToken = default);

    Task<byte> WriteParameterAsync(KatanaParameterDefinition parameter, byte value, CancellationToken cancellationToken = default);

    Task<byte[]> ReadBlockAsync(IReadOnlyList<byte> address, int length, CancellationToken cancellationToken = default);

    Task WriteBlockAsync(IReadOnlyList<byte> address, IReadOnlyList<byte> data, CancellationToken cancellationToken = default);
}
