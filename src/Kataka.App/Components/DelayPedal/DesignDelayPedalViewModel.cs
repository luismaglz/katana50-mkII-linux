using Kataka.Domain.Models;

using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignDelayPedalViewModel : DelayPedalViewModel
{
    public DesignDelayPedalViewModel() : base(PedalPosition.Delay,
        new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance))
    {
        IsEnabled = true;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }

    public static DesignDelayPedalViewModel Instance => new();
}
