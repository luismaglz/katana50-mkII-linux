using System.Collections.ObjectModel;

using Kataka.App.KatanaState;
using Kataka.App.Services;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public class AmpEditorViewModel : ViewModelBase
{
    private readonly IKatanaState _katanaState;

    public AmpEditorViewModel(
        IKatanaState katanaState,
        IAmpSyncService syncService)
    {
        _katanaState = katanaState;

        katanaState.AmpType.ValueChanged += () =>
        {
            var idx = katanaState.AmpType.Value;
            if (idx < AmpTypeOptions.Length) SelectedAmpType = AmpTypeOptions[idx];
        };
        katanaState.CabinetResonance.ValueChanged += () =>
        {
            var idx = katanaState.CabinetResonance.Value;
            if (idx < CabinetResonanceOptions.Length) SelectedCabinetResonance = CabinetResonanceOptions[idx];
        };
        katanaState.AmpVariation.ValueChanged += () =>
            IsAmpVariation = katanaState.AmpVariation.Value != 0;

        foreach (var effectViewModel in new PedalViewModel[]
                 {
                     new BoosterPedalViewModel(katanaState), new ModFxPedalViewModel("mod", katanaState),
                     new ModFxPedalViewModel("fx", katanaState), new DelayPedalViewModel("delay", katanaState),
                     new DelayPedalViewModel("delay2", katanaState), new ReverbPedalViewModel(katanaState)
                 })
            PanelEffects.Add(effectViewModel);

        var panelEffectsByDefinitionKey = PanelEffects.ToDictionary(e => e.Definition.Key);
        Pedalboard = new PedalboardViewModel(katanaState, panelEffectsByDefinitionKey);
        Panel = new PanelViewModel(katanaState);
        ChannelSelection = new ChannelSelectionViewModel(syncService, katanaState);
        PedalFx = new PedalFxViewModel(katanaState);

        syncService.ReadCompleted.Subscribe(meta =>
        {
            if (meta.PanelControlsStatus.Length > 0) PanelControlsStatus = meta.PanelControlsStatus;
            if (meta.PedalControlsStatus.Length > 0) PedalControlsStatus = meta.PedalControlsStatus;
        }).DisposeWith(Disposables);

        this.WhenAnyValue(x => x.SelectedAmpType)
            .Subscribe(v =>
            {
                var idx = Array.IndexOf(AmpTypeOptions, v);
                if (idx < 0) return;
                _katanaState.AmpType.Value = idx;
            }).DisposeWith(Disposables);

        this.WhenAnyValue(x => x.SelectedCabinetResonance)
            .Subscribe(v =>
            {
                var idx = Array.IndexOf(CabinetResonanceOptions, v);
                if (idx < 0) return;
                _katanaState.CabinetResonance.Value = idx;
            }).DisposeWith(Disposables);

        this.WhenAnyValue(x => x.IsAmpVariation)
            .Subscribe(v => _katanaState.AmpVariation.Value = v ? 1 : 0)
            .DisposeWith(Disposables);
    }

    public ObservableCollection<AmpControlViewModel> AmpControls { get; } = [];
    public ObservableCollection<PedalViewModel> PanelEffects { get; } = [];
    public PedalFxViewModel PedalFx { get; }
    public PedalboardViewModel Pedalboard { get; }
    public PanelViewModel Panel { get; }
    public ChannelSelectionViewModel ChannelSelection { get; }

    public static string[] AmpTypeOptions { get; } = ["ACOUSTIC", "CLEAN", "CRUNCH", "LEAD", "BROWN"];
    public static string[] CabinetResonanceOptions { get; } = ["LOW", "MIDDLE", "HIGH"];

    [Reactive] public string SelectedAmpType { get; set; } = "CLEAN";
    [Reactive] public string SelectedCabinetResonance { get; set; } = "MIDDLE";
    [Reactive] public bool IsAmpVariation { get; set; }
    [Reactive] public string PanelControlsStatus { get; set; } = "Panel controls have not been read yet.";
    [Reactive] public string PedalControlsStatus { get; set; } = "Pedal controls have not been read yet.";
}
