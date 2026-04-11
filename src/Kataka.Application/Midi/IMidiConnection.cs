using Kataka.Domain.Midi;

namespace Kataka.Application.Midi;

public interface IMidiConnection : IAsyncDisposable
{
    Task SendAsync(SysExMessage message, CancellationToken cancellationToken = default);

    Task<SysExMessage> RequestAsync(
        SysExMessage request,
        TimeSpan timeout,
        CancellationToken cancellationToken = default);
}
