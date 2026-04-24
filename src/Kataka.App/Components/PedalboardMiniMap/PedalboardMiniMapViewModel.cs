using System.Collections.ObjectModel;
using System.Globalization;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

using Kataka.App.KatanaState;

namespace Kataka.App.ViewModels;

public class PedalboardMiniMapViewModel : ViewModelBase
{
    private readonly IKatanaState _katanaState;
    private readonly ChainNode Amp = new(ChainNodeType.Amp, "white");
    private readonly ChainNode Booster = new(ChainNodeType.Booster, "gold");

    private readonly List<ChainNode[]> Chains = new();
    private readonly ChainNode Delay = new(ChainNodeType.Delay, "lightgray");
    private readonly ChainNode Delay2 = new(ChainNodeType.Delay2, "lightgray");
    private readonly ChainNode Fx = new(ChainNodeType.Fx, "purple");

    private readonly ChainNode Guitar = new(ChainNodeType.Input, "white");
    private readonly ChainNode Mod = new(ChainNodeType.Mod, "blue");
    private readonly ChainNode Reverb = new(ChainNodeType.Reverb, "lightblue");
    private readonly ChainNode Speaker = new(ChainNodeType.Speaker, "white");

    public PedalboardMiniMapViewModel(IKatanaState katanaState)
    {
        _katanaState = katanaState;

        Chains.Add([Guitar, Booster, Amp, Mod, Fx, Delay, Delay2, Reverb, Speaker]);
        Chains.Add([Guitar, Booster, Mod, Amp, Fx, Delay, Delay2, Reverb, Speaker]);
        Chains.Add([Guitar, Booster, Mod, Fx, Amp, Delay, Delay2, Reverb, Speaker]);
        Chains.Add([Guitar, Booster, Mod, Fx, Delay, Amp, Delay2, Reverb, Speaker]);
        Chains.Add([Guitar, Mod, Booster, Amp, Fx, Delay, Delay2, Reverb, Speaker]);
        Chains.Add([Guitar, Mod, Booster, Fx, Amp, Delay, Delay2, Reverb, Speaker]);
        Chains.Add([Guitar, Mod, Booster, Fx, Delay, Amp, Delay2, Reverb, Speaker]);

        _katanaState.PedalChain.ValueChanged += () => { UpdateChain(_katanaState.PedalChain.Value); };
    }

    public ObservableCollection<ChainNode> ChainNodes { get; set; } = new();

    private void UpdateChain(int chainValue)
    {
        // Ensure we are on the UI thread for collection changes
        Dispatcher.UIThread.Post(() =>
        {
            ChainNodes.Clear();
            if (chainValue >= 0 && chainValue < Chains.Count)
                foreach (var node in Chains[chainValue])
                    ChainNodes.Add(node);
        });
    }
}

public class PedalTypeSelector : IDataTemplate
{
    // These properties act as slots you fill in your XAML
    public IDataTemplate? GuitarTemplate { get; set; }
    public IDataTemplate? AmpTemplate { get; set; }
    public IDataTemplate? PedalTemplate { get; set; }
    public IDataTemplate? SpeakerTemplate { get; set; }

    // This is the method that Avalonia calls for every item in your collection
    public Control? Build(object? data)
    {
        if (data is not ChainNode node) return null;

        // The "Switch Statement" logic
        var template = node.ChainNodeType switch
        {
            ChainNodeType.Input => GuitarTemplate,
            ChainNodeType.Amp => AmpTemplate,
            ChainNodeType.Speaker => SpeakerTemplate,
            _ => PedalTemplate // Default for Booster, Mod, FX, Delay
        };

        return template?.Build(data);
    }

    // This tells Avalonia: "I only handle ChainNode objects"
    public bool Match(object? data) => data is ChainNode;
}

public class NodeTypeToImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ChainNodeType type)
        {
            var assetName = type switch
            {
                ChainNodeType.Input => "electric-guitar.png",
                ChainNodeType.Amp => "amplifier.png",
                ChainNodeType.Speaker => "speakers.png",
                _ => "guitar-pedal.png"
            };

            // Important: Use the full avares path to your project
            var uri = new Uri($"avares://Kataka.App/Assets/{assetName}");

            // AssetLoader is the reliable way to grab these for a converter
            return new Bitmap(AssetLoader.Open(uri));
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
