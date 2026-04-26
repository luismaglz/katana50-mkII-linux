using Avalonia.Media.Imaging;

using Kataka.App.ViewModels;

namespace Kataka.App.Components.Pedalboard;

public abstract class PedalboardPosition(string color)
{
    public string Color { get; } = color;
}

public class PedalboardInput(string color, Bitmap image) : PedalboardPosition(color)
{
    public Bitmap Image { get; } = image;
}

public class PedalboardOutput(string color, Bitmap image) : PedalboardPosition(color)
{
    public Bitmap Image { get; } = image;
}

public class PedalboardAmp(string color, Bitmap image) : PedalboardPosition(color)
{
    public Bitmap Image { get; } = image;
}

public abstract class PedalboardPedalBase(ViewModelBase viewModel, string color) : PedalboardPosition(color)
{
    public ViewModelBase ViewModel { get; } = viewModel;
}

public class PedalboardPedal<T>(T viewModel, string color) : PedalboardPedalBase(viewModel, color)
    where T : ViewModelBase;
