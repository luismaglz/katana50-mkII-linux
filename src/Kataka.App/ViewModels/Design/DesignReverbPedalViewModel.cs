using Kataka.Domain.KatanaState;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignReverbPedalViewModel : ReverbPedalViewModel
{
    public static DesignReverbPedalViewModel Instance => new();

    public DesignReverbPedalViewModel() : base(new KatanaState())
    {
        IsEnabled = true;
        if (TypeOptions.Count > 0) SelectedTypeOption = TypeOptions[0];
    }
}
