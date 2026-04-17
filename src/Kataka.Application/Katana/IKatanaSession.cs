using Kataka.Domain.Midi;

namespace Kataka.Application.Katana;

public interface IKatanaSession : IAsyncDisposable
{
    bool IsConnected { get; }

    /// <summary>
    /// Raised when the amp sends an unsolicited DT1 message (e.g., a knob was turned on the device).
    /// Fired on the MIDI receive thread — subscribers must marshal to the UI thread if needed.
    /// Only raised when the connection supports persistent listening (DryWetMidiConnection).
    /// </summary>
    event EventHandler<SysExMessage>? PushNotificationReceived;

    /// <summary>
    /// Raised when the user presses a channel button on the amp (Program Change).
    /// Fired on the MIDI receive thread — subscribers must marshal to the UI thread if needed.
    /// </summary>
    event EventHandler<KatanaPanelChannel>? PanelChannelChanged;

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

    /// <summary>
    /// Reads all known patch blocks from the Temporary area and returns a populated <see cref="TslPatch"/>.
    /// </summary>
    Task<TslPatch> ReadCurrentPatchAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads all known patch blocks and returns a flat address-string → byte map,
    /// keyed identically to <see cref="KatanaParameterDefinition.AddressString"/> (e.g. "60-00-06-52").
    /// Pass the result directly to <c>IKatanaState.SetStates()</c> to seed all domain state on connect.
    /// </summary>
    Task<IReadOnlyDictionary<string, byte>> ReadAllPatchStatesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes all blocks in <paramref name="patch"/> to the Temporary area, replacing the active patch state.
    /// </summary>
    Task LoadPatchAsync(TslPatch patch, CancellationToken cancellationToken = default);
}
