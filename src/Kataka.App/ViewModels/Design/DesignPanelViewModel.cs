using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignPanelViewModel : PanelViewModel
{
    public DesignPanelViewModel()
        : base(new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance))
    {
    }

    public static DesignPanelViewModel Instance => new();
}
