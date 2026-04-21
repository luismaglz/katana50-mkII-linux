using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.Controls.Templates;

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
        ChainNodes.Clear();
        var nodes = Chains[chainValue] ?? null;
        if (nodes != null)
            foreach (var node in nodes)
                ChainNodes.Add(node);
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

public enum ChainNodeType
{
    Input,
    Booster,
    Amp,
    Mod,
    Fx,
    Delay,
    Delay2,
    Reverb,
    Speaker
}

public class ChainNode(ChainNodeType chainNodeType, string color)
{
    public ChainNodeType ChainNodeType { get; } = chainNodeType;
    public string Color { get; } = color;
}
