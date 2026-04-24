using System.Collections.ObjectModel;

using Avalonia.Threading;

using Kataka.App.KatanaState;
using Kataka.App.ViewModels;
using Kataka.Domain.Midi;
using Kataka.Domain.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.Components.Pedalboard;

public class PedalboardViewModel : ViewModelBase
{
    private readonly IKatanaState _katanaState;
    private readonly List<PedalboardPosition[]> _chains = [];

    private readonly PedalboardInput _inputHardware = new("white", "/Assets/electric-guitar.png");
    private readonly PedalboardOutput _outputHardware = new("white", "/Assets/speakers.png");
    private readonly PedalboardAmp _ampHardware = new("white", "/Assets/amplifier.png");

    private readonly PedalboardPedal<BoosterPedalViewModel> _boosterPedal;
    private readonly PedalboardPedal<ModFxPedalViewModel> _modPedal;
    private readonly PedalboardPedal<ModFxPedalViewModel> _fxPedal;
    private readonly PedalboardPedal<DelayPedalViewModel> _delayPedal;
    private readonly PedalboardPedal<DelayPedalViewModel> _delayPedal2;
    private readonly PedalboardPedal<ReverbPedalViewModel> _reverbPedal;

    public PedalboardViewModel(IKatanaState katanaState)
    {
        _katanaState = katanaState;

        _boosterPedal = new PedalboardPedal<BoosterPedalViewModel>(new BoosterPedalViewModel(katanaState), "gold");
        _modPedal = new PedalboardPedal<ModFxPedalViewModel>(new ModFxPedalViewModel(PedalPosition.Mod, katanaState), "blue");
        _fxPedal = new PedalboardPedal<ModFxPedalViewModel>(new ModFxPedalViewModel(PedalPosition.Fx, katanaState), "purple");
        _delayPedal = new PedalboardPedal<DelayPedalViewModel>(new DelayPedalViewModel(PedalPosition.Delay, katanaState), "lightgray");
        _delayPedal2 = new PedalboardPedal<DelayPedalViewModel>(new DelayPedalViewModel(PedalPosition.Delay2, katanaState), "lightgray");
        _reverbPedal = new PedalboardPedal<ReverbPedalViewModel>(new ReverbPedalViewModel(katanaState), "cyan");

        _chains.Add([_inputHardware, _boosterPedal, _ampHardware, _modPedal, _fxPedal, _delayPedal, _delayPedal2, _reverbPedal, _outputHardware]);
        _chains.Add([_inputHardware, _boosterPedal, _modPedal, _ampHardware, _fxPedal, _delayPedal, _delayPedal2, _reverbPedal, _outputHardware]);
        _chains.Add([_inputHardware, _boosterPedal, _modPedal, _fxPedal, _ampHardware, _delayPedal, _delayPedal2, _reverbPedal, _outputHardware]);
        _chains.Add([_inputHardware, _boosterPedal, _modPedal, _fxPedal, _delayPedal, _ampHardware, _delayPedal2, _reverbPedal, _outputHardware]);
        _chains.Add([_inputHardware, _modPedal, _boosterPedal, _ampHardware, _fxPedal, _delayPedal, _delayPedal2, _reverbPedal, _outputHardware]);
        _chains.Add([_inputHardware, _modPedal, _boosterPedal, _fxPedal, _ampHardware, _delayPedal, _delayPedal2, _reverbPedal, _outputHardware]);
        _chains.Add([_inputHardware, _modPedal, _boosterPedal, _fxPedal, _delayPedal, _ampHardware, _delayPedal2, _reverbPedal, _outputHardware]);

        _katanaState.PedalChain.ValueChanged += () => UpdateChain(_katanaState.PedalChain.Value);
        UpdateChain(_katanaState.PedalChain.Value);
    }

    public ObservableCollection<PedalboardPosition> ChainNodes { get; } = [];

    public static string[] ChainPatternNames { get; } =
        ["CHAIN 1", "CHAIN 2-1", "CHAIN 3-1", "CHAIN 4-1", "CHAIN 2-2", "CHAIN 3-2", "CHAIN 4-2"];

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

    private void UpdateChain(int chainValue)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ChainNodes.Clear();
            if (chainValue >= 0 && chainValue < _chains.Count)
                foreach (var node in _chains[chainValue])
                    ChainNodes.Add(node);
        });
    }
}
