using System.Collections.ObjectModel;

using Kataka.App.KatanaState;
using Kataka.Domain.Midi;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public class PedalboardViewModel : ViewModelBase
{
    private static readonly string[][] ChainBeforeAmp =
    [
        ["booster"],
        ["booster", "mod"],
        ["booster", "mod", "fx"],
        ["booster", "mod", "fx", "delay", "delay2"],
        ["mod", "booster"],
        ["mod", "booster", "fx"],
        ["mod", "booster", "fx", "delay", "delay2"]
    ];

    private static readonly string[][] ChainAfterAmp =
    [
        ["mod", "fx", "delay", "delay2", "reverb"],
        ["fx", "delay", "delay2", "reverb"],
        ["delay", "delay2", "reverb"],
        ["reverb"],
        ["fx", "delay", "delay2", "reverb"],
        ["delay", "delay2", "reverb"],
        ["reverb"]
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

        _katanaState.SelectedChannelChanged += newChannel =>
        {
            SelectedChannel = newChannel;
            Refresh();
        };

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

        var items = new List<PedalboardItemViewModel>
        {
            new()
            {
                Key = "input",
                DisplayName = "INPUT",
                Detail = "Guitar",
                IsEndpoint = true,
                Family = "io"
            }
        };

        var chainIdx = Math.Clamp(SelectedChainPattern, 0, ChainBeforeAmp.Length - 1);
        AddPedalsByKeys(items, ChainBeforeAmp[chainIdx]);

        _ampItem = new PedalboardItemViewModel
        {
            Key = "amp",
            DisplayName = "AMP",
            Detail = SelectedChannel.ToString().ToUpperInvariant(),
            IsAmp = true,
            IsConnectedFromPrevious = items.Count > 0,
            Family = "amp"
        };
        items.Add(_ampItem);

        AddPedalsByKeys(items, ChainAfterAmp[chainIdx]);

        items.Add(new PedalboardItemViewModel
        {
            Key = "output",
            DisplayName = "OUTPUT",
            Detail = "Speaker / Rec Out",
            IsEndpoint = true,
            IsConnectedFromPrevious = items.Count > 0,
            Family = "io"
        });

        PedalboardItems.Clear();
        foreach (var item in items)
            PedalboardItems.Add(item);
    }

    private void AddPedalsByKeys(List<PedalboardItemViewModel> items, string[] keys)
    {
        foreach (var key in keys)
        {
            if (!_pedalsByKey.TryGetValue(key, out var effect)) continue;

            items.Add(new PedalboardItemViewModel
            {
                Key = effect.Definition.Key,
                DisplayName = effect.DisplayName.ToUpperInvariant(),
                Detail = effect.DisplayName,
                IsConnectedFromPrevious = items.Count > 0,
                Family = effect.Definition.Key,
                PanelEffect = effect
            });
        }
    }
}
