using Microsoft.Extensions.Logging.Abstractions;

namespace Kataka.App.ViewModels.Design;

public sealed class DesignNoiseSuppressorViewModel : NoiseSuppressorViewModel
{
    public DesignNoiseSuppressorViewModel()
        : base(new KatanaState.KatanaState(NullLogger<KatanaState.KatanaState>.Instance))
    {
        IsEnabled = true;
    }

    public static DesignNoiseSuppressorViewModel Instance => new();
}
