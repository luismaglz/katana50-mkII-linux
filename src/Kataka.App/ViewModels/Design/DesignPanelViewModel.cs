using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignPanelViewModel : PanelViewModel
{
    public static DesignPanelViewModel Instance => new();

    public DesignPanelViewModel()
        : base(new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance))
    {
    }
}
