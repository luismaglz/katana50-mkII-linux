using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Kataka.Application.Katana;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels.Design;

/// <summary>
/// No-op IKatanaSession used exclusively by design-time ViewModels so the Avalonia
/// previewer can instantiate MainWindowViewModel without attempting MIDI I/O.
/// </summary>
internal sealed class NullKatanaSession : IKatanaSession
{
    public bool IsConnected => false;

    // Design-time stub — push notifications are never raised without a real MIDI connection.
    public event EventHandler<SysExMessage>? PushNotificationReceived { add { } remove { } }
    public event EventHandler<KatanaPanelChannel>? PanelChannelChanged { add { } remove { } }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task<IReadOnlyList<MidiPortDescriptor>> ListPortsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<MidiPortDescriptor>>([]);

    public Task ConnectAsync(string inputPortId, string outputPortId, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task DisconnectAsync() => Task.CompletedTask;

    public Task<SysExMessage> RequestIdentityAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new SysExMessage([]));

    public Task<KatanaPanelChannel?> ReadCurrentPanelChannelAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<KatanaPanelChannel?>(null);

    public Task SelectPanelChannelAsync(KatanaPanelChannel channel, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task<byte> ReadParameterAsync(KatanaParameterDefinition parameter, CancellationToken cancellationToken = default)
        => Task.FromResult<byte>(0);

    public Task<IReadOnlyDictionary<string, byte>> ReadParametersAsync(
        IReadOnlyList<KatanaParameterDefinition> parameters,
        CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyDictionary<string, byte>>(new Dictionary<string, byte>());

    public Task<byte> WriteParameterAsync(KatanaParameterDefinition parameter, byte value, CancellationToken cancellationToken = default)
        => Task.FromResult(value);

    public Task<byte[]> ReadBlockAsync(IReadOnlyList<byte> address, int length, CancellationToken cancellationToken = default)
        => Task.FromResult(new byte[length]);

    public Task WriteBlockAsync(IReadOnlyList<byte> address, IReadOnlyList<byte> data, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task<TslPatch> ReadCurrentPatchAsync(string name, CancellationToken cancellationToken = default)
        => Task.FromResult(new TslPatch { Name = name });

    public Task LoadPatchAsync(TslPatch patch, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
