using System.Collections.ObjectModel;

using Kataka.App.Components.Pedalboard;
using Kataka.App.KatanaState;
using Kataka.Domain.Midi;
using Kataka.Domain.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public class PedalboardViewModel : ViewModelBase
{
    private readonly IKatanaState _katanaState;
    private PedalboardItemViewModel? _ampItem;

    /// <summary>
    ///     List of possible pedalboard chains to select from
    /// </summary>
    private List<PedalboardPosition>[] _chains;

    private int _lastRenderedChain = -1;
    private KatanaPanelChannel _lastRenderedChannel;

    public PedalboardViewModel(IKatanaState katanaState)
    {
        _katanaState = katanaState;

        // Initialize Pedals
        _boosterPedal = new PedalboardPedal<BoosterPedalViewModel>(new BoosterPedalViewModel(katanaState), "gold");
        _modPedal = new PedalboardPedal<ModFxPedalViewModel>(new ModFxPedalViewModel(PedalPosition.Mod, katanaState),
            "blue");
        _fxPedal = new PedalboardPedal<ModFxPedalViewModel>(new ModFxPedalViewModel(PedalPosition.Fx, katanaState),
            "purple");
        _delayPedal =
            new PedalboardPedal<DelayPedalViewModel>(new DelayPedalViewModel(PedalPosition.Delay, katanaState),
                "lightgray");


        _katanaState.PedalChain.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(SelectedChainPattern));
            Refresh();
        };
    }

    public IReadOnlyDictionary<string, PedalViewModel> PedalsByKey { get; }

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
                items.Add(new PedalboardItemViewModel
                {
                    Key = "input",
                    DisplayName = "INPUT",
                    Detail = "Guitar",
                    IsEndpoint = true,
                    Family = "io",
                    IsConnectedFromPrevious = isConnected
                });
            else if (key == "output")
                items.Add(new PedalboardItemViewModel
                {
                    Key = "output",
                    DisplayName = "OUTPUT",
                    Detail = "Speaker / Rec Out",
                    IsEndpoint = true,
                    Family = "io",
                    IsConnectedFromPrevious = isConnected
                });
            else if (key == "amp")
                items.Add(_ampItem = new PedalboardItemViewModel
                {
                    Key = "amp",
                    DisplayName = "AMP",
                    Detail = SelectedChannel.ToString().ToUpperInvariant(),
                    IsAmp = true,
                    Family = "amp",
                    IsConnectedFromPrevious = isConnected
                });
            else if (PedalsByKey.TryGetValue(key, out var effect))
                items.Add(new PedalboardItemViewModel
                {
                    Key = effect.Definition.Key,
                    DisplayName = effect.DisplayName.ToUpperInvariant(),
                    Detail = effect.DisplayName,
                    Family = effect.Definition.Key,
                    PanelEffect = effect,
                    IsConnectedFromPrevious = isConnected
                });
        }

        PedalboardItems.Clear();
        foreach (var item in items)
            PedalboardItems.Add(item);
    }

    #region Pedalboard Hardware

    // Represents the input/output/amp positions on the pedalboard, which are not actual pedals but still need to be rendered
    private readonly PedalboardHardware _inputHardware = new("Assets/electric-guitar.png", "white");
    private readonly PedalboardHardware _outputHardware = new("Assets/speakers.png", "white");
    private readonly PedalboardHardware _ampHardware = new("Assets/amp.png", "white");

    #endregion

    #region Pedalboard Pedals

    // Pedals that need to be rendered in different positions in the pedalboard
    private readonly PedalboardPedal<BoosterPedalViewModel> _boosterPedal;
    private readonly PedalboardPedal<ModFxPedalViewModel> _modPedal;
    private readonly PedalboardPedal<ModFxPedalViewModel> _fxPedal;
    private readonly PedalboardPedal<DelayPedalViewModel> _delayPedal;
    private readonly PedalboardPedal<DelayPedalViewModel> _delayPedal2;
    private readonly PedalboardPedal<ReverbPedalViewModel> _reverbPedal;

    #endregion
}
