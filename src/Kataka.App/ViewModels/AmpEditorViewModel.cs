using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using Kataka.App.KatanaState;
using Kataka.App.Services;
using Kataka.Application.Katana;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class AmpEditorViewModel : ViewModelBase
{
    private readonly Action<string> _appendStatus;
    private readonly IKatanaSession _katanaSession;
    private readonly IKatanaState _katanaState;
    private readonly ILogger<AmpEditorViewModel> _logger;
    private readonly IAmpSyncService _syncService;

    public AmpEditorViewModel(
        IKatanaSession katanaSession,
        IKatanaState katanaState,
        IAmpSyncService syncService,
        Action<string> appendStatus,
        ILogger<AmpEditorViewModel> logger)
    {
        _katanaSession = katanaSession;
        _katanaState = katanaState;
        _syncService = syncService;
        _appendStatus = appendStatus;
        _logger = logger;

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

        syncService.ReadCompleted.Subscribe(meta =>
        {
            if (meta.PanelControlsStatus.Length > 0) PanelControlsStatus = meta.PanelControlsStatus;
            if (meta.PedalControlsStatus.Length > 0) PedalControlsStatus = meta.PedalControlsStatus;
        }).DisposeWith(Disposables);


        this.WhenAnyValue(x => x.IsAmpVariation)
            .Subscribe(v => _katanaState.AmpVariation.Value = v ? 1 : 0)
            .DisposeWith(Disposables);
    }

    public ObservableCollection<AmpControlViewModel> AmpControls { get; } = [];
    public ObservableCollection<PedalViewModel> PanelEffects { get; } = [];
    public PedalFxViewModel PedalFx { get; } = new();
    public PedalboardViewModel Pedalboard { get; }
    public PanelViewModel Panel { get; }
    public ChannelSelectionViewModel ChannelSelection { get; }

    [Reactive] public bool IsAmpVariation { get; set; }
    [Reactive] public string PanelControlsStatus { get; set; } = "Panel controls have not been read yet.";
    [Reactive] public string PedalControlsStatus { get; set; } = "Pedal controls have not been read yet.";

    [RelayCommand]
    private async Task WritePanelControlsAsync()
    {
        if (!_katanaSession.IsConnected)
        {
            _appendStatus("Connect to a MIDI port before writing panel controls.");
            return;
        }

        try
        {
            _logger.LogInformation("Writing Katana panel controls.");

            foreach (var effect in PanelEffects)
            {
                var confirmedValue = await _katanaSession.WriteParameterAsync(
                    effect.Definition.SwitchParameter,
                    effect.IsEnabled ? (byte)1 : (byte)0);
                effect.IsEnabled = confirmedValue != 0;
                _logger.LogInformation("{Name} confirmed {State}.", effect.DisplayName,
                    effect.IsEnabled ? "On" : "Off");

                if (effect.Definition.TypeParameter is not null &&
                    effect.TryGetTypeValue(effect.SelectedTypeOption, out var requestedType))
                {
                    var confirmedType = await _katanaSession.WriteParameterAsync(
                        effect.Definition.TypeParameter, requestedType);
                    effect.SelectedTypeOption = effect.ToTypeOption(confirmedType);
                    _logger.LogInformation("{Name} type confirmed at {Type}.", effect.DisplayName,
                        effect.SelectedTypeOption);
                }
            }

            _appendStatus("Panel controls updated successfully.");
        }
        catch (Exception ex)
        {
            PanelControlsStatus = "Panel control write failed.";
            _appendStatus(ex.Message);
            _logger.LogError(ex, "Panel control write failed.");
        }
    }

    [RelayCommand]
    private async Task WritePedalControlsAsync()
    {
        if (!_katanaSession.IsConnected)
        {
            _appendStatus("Connect to a MIDI port before writing pedal controls.");
            return;
        }

        try
        {
            _logger.LogInformation("Writing Katana pedal controls.");
            var mismatches = new List<string>();

            foreach (var parameter in PedalFx.GetManualWriteParameters())
            {
                if (!PedalFx.TryGetCurrentValue(parameter.Key, out var requestedValue)) continue;

                _logger.LogInformation("Writing {Name} = {Value}.", parameter.DisplayName, requestedValue);
                var confirmedValue = await _katanaSession.WriteParameterAsync(parameter, requestedValue);
                ApplyPedalValue(parameter.Key, confirmedValue);
                _logger.LogInformation("{Name} confirmed at {Value}.", parameter.DisplayName, confirmedValue);

                if (confirmedValue != requestedValue)
                    mismatches.Add($"{parameter.DisplayName} ({requestedValue}->{confirmedValue})");
            }

            _appendStatus(mismatches.Count == 0
                ? "Pedal controls updated successfully."
                : "Pedal write completed, but some read-back values differed.");
            PedalControlsStatus = mismatches.Count == 0
                ? "Pedal FX values were written and confirmed."
                : $"Pedal read-back mismatches: {string.Join(", ", mismatches)}";
        }
        catch (Exception ex)
        {
            PedalControlsStatus = "Pedal control write failed.";
            _appendStatus(ex.Message);
            _logger.LogError(ex, "Pedal control write failed.");
        }
    }

    private void ApplyPedalValue(string parameterKey, byte value)
    {
        switch (parameterKey)
        {
            case "pedal-fx-switch": PedalFx.IsEnabled = value != 0; break;
            case "pedal-fx-type": PedalFx.SelectedTypeOption = PedalFxViewModel.ToPedalTypeOption(value); break;
            case "pedal-fx-position": PedalFx.SelectedPositionOption = PedalFxViewModel.ToPositionOption(value); break;
            case "pedal-fx-wah-type": PedalFx.SelectedWahTypeOption = PedalFxViewModel.ToWahTypeOption(value); break;
            case "pedal-fx-wah-position": PedalFx.PedalPosition = value; break;
            case "pedal-fx-wah-min": PedalFx.PedalMinimum = value; break;
            case "pedal-fx-wah-max": PedalFx.PedalMaximum = value; break;
            case "pedal-fx-wah-effect-level": PedalFx.EffectLevel = value; break;
            case "pedal-fx-wah-direct-mix": PedalFx.DirectMix = value; break;
            case "pedal-fx-foot-volume": PedalFx.FootVolume = value; break;
        }
    }
}
