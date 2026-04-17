using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignDelayPedalViewModel : DelayPedalViewModel
{
    public static DesignDelayPedalViewModel Instance => new();

    public DesignDelayPedalViewModel() : base("delay", new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance))
    {
        IsEnabled = true;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
