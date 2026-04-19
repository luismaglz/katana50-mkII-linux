using Kataka.App.Services;

using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignChannelSelectionViewModel : ChannelSelectionViewModel
{
    public static DesignChannelSelectionViewModel Instance => new();

    public DesignChannelSelectionViewModel()
        : base(
            new AmpSyncService(
                new NullKatanaSession(),
                new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance),
                NullLogger<AmpSyncService>.Instance),
            new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance))
    {
    }
}
