using Kataka.App.ViewModels;

namespace Kataka.App.Components.Pedalboard;

public abstract class PedalboardPosition(string color)
{
    public string Color { get; } = color;
}

public class PedalboardInput(string color, string imagePath) : PedalboardPosition(color)
{
    public string ImagePath { get; } = imagePath;
}

public class PedalboardOutput(string color, string imagePath) : PedalboardPosition(color)
{
    public string ImagePath { get; } = imagePath;
}

public class PedalboardAmp(string color, string imagePath) : PedalboardPosition(color)
{
    public string ImagePath { get; } = imagePath;
}

public abstract class PedalboardPedalBase(ViewModelBase viewModel, string color) : PedalboardPosition(color)
{
    public ViewModelBase ViewModel { get; } = viewModel;
}

public class PedalboardPedal<T>(T viewModel, string color) : PedalboardPedalBase(viewModel, color)
    where T : ViewModelBase;
