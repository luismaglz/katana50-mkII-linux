using System.Collections.ObjectModel;

using Kataka.App.KatanaState;
using Kataka.Domain.Midi;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public class PedalboardViewModel : ViewModelBase
{
    private static readonly List<string[]> Chains =
    [
        ["input", "booster", "amp", "mod", "fx", "delay", "delay2", "reverb", "output"],
        ["input", "booster", "mod", "amp", "fx", "delay", "delay2", "reverb", "output"],
        ["input", "booster", "mod", "fx", "amp", "delay", "delay2", "reverb", "output"],
        ["input", "booster", "mod", "fx", "delay", "amp", "delay2", "reverb", "output"],
        ["input", "mod", "booster", "amp", "fx", "delay", "delay2", "reverb", "output"],
        ["input", "mod", "booster", "fx", "amp", "delay", "delay2", "reverb", "output"],
        ["input", "mod", "booster", "fx", "delay", "amp", "delay2", "reverb", "output"],
    ];

    private readonly IKatanaState _katanaState;
    private readonly IReadOnlyDictionary<string, PedalViewModel> _pedalsByKey;
    private PedalboardItemViewModel? _ampItem;
    private int _lastRenderedChain = -1;
    private KatanaPanelChannel _lastRenderedChannel;

    public PedalboardViewModel(
        IKatanaState katanaState,
        IReadOnlyDictionary<string, PedalViewModel> pedalsByKey)
    {
        _katanaState = katanaState;
        _pedalsByKey = pedalsByKey;

        // _katanaState.SelectedChannelChanged += newChannel =>
        // {
        //     SelectedChannel = newChannel;
        //     Refresh();
        // };

        _katanaState.PedalChain.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(SelectedChainPattern));
            Refresh();
        };
    }

    public static string[] ChainPatternNames { get; } =
        ["CHAIN 1", "CHAIN 2-1", "CHAIN 3-1", "CHAIN 4-1", "CHAIN 2-2", "CHAIN 3-2", "CHAIN 4-2"];

    public ObservableCollection<PedalboardItemViewModel> PedalboardItems { get; } = [];

    [Reactive] public KatanaPanelChannel SelectedChannel { get; set; } = KatanaPanelChannel.Panel;

    public int SelectedChainPattern
    {
        get => _katanaState.PedalChain.Value;
        set
        {
            _katanaState.PedalChain.Value = value;
            this.RaisePropertyChanged();
        }
    }

    private void Refresh()
    {
        var chain = _katanaState.PedalChain.Value;
        var channel = SelectedChannel;
        if (chain == _lastRenderedChain && channel == _lastRenderedChannel) return;
        _lastRenderedChain = chain;
        _lastRenderedChannel = channel;

        var chainIdx = Math.Clamp(chain, 0, Chains.Count - 1);
        var keys = Chains[chainIdx];

        var items = new List<PedalboardItemViewModel>();
        foreach (var key in keys)
        {
            var isConnected = items.Count > 0;
            if (key == "input")
                items.Add(new() { Key = "input", DisplayName = "INPUT", Detail = "Guitar", IsEndpoint = true, Family = "io", IsConnectedFromPrevious = isConnected });
            else if (key == "output")
                items.Add(new() { Key = "output", DisplayName = "OUTPUT", Detail = "Speaker / Rec Out", IsEndpoint = true, Family = "io", IsConnectedFromPrevious = isConnected });
            else if (key == "amp")
                items.Add(_ampItem = new() { Key = "amp", DisplayName = "AMP", Detail = SelectedChannel.ToString().ToUpperInvariant(), IsAmp = true, Family = "amp", IsConnectedFromPrevious = isConnected });
            else if (_pedalsByKey.TryGetValue(key, out var effect))
                items.Add(new() { Key = effect.Definition.Key, DisplayName = effect.DisplayName.ToUpperInvariant(), Detail = effect.DisplayName, Family = effect.Definition.Key, PanelEffect = effect, IsConnectedFromPrevious = isConnected });
        }

        PedalboardItems.Clear();
        foreach (var item in items)
            PedalboardItems.Add(item);
    }
}
