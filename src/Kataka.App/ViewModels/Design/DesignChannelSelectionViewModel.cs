using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignChannelSelectionViewModel : ChannelSelectionViewModel
{
    public DesignChannelSelectionViewModel()
        : base(new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance))
    {
    }

    public static DesignChannelSelectionViewModel Instance => new();
}
