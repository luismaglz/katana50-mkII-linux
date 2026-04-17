using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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
    private readonly IKatanaSession _katanaSession;
    private readonly IKatanaState _katanaState;
    private readonly IAmpSyncService _syncService;
    private readonly Action<string> _appendStatus;
    private readonly ILogger<AmpEditorViewModel> _logger;

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

        foreach (var effectViewModel in new IBasePedal[]
                 {
                     new BoosterPedalViewModel(katanaState),
                     new ModFxPedalViewModel("mod"),
                     new ModFxPedalViewModel("fx"),
                     new DelayPedalViewModel("delay", katanaState),
                     new DelayPedalViewModel("delay2", katanaState),
                     new ReverbPedalViewModel(katanaState)
                 })
        {
            PanelEffects.Add(effectViewModel);
        }

        foreach (var channel in PanelChannels)
            PanelChannelOptions.Add(new PanelChannelOptionViewModel(channel));

        var panelEffectsByDefinitionKey = PanelEffects.ToDictionary(e => e.Definition.Key);
        Pedalboard = new PedalboardViewModel(panelEffectsByDefinitionKey, SelectedPanelChannel);
        Panel = new PanelViewModel(katanaState);

        syncService.ReadCompleted.Subscribe(meta =>
        {
            if (meta.PanelControlsStatus.Length > 0) PanelControlsStatus = meta.PanelControlsStatus;
            if (meta.PedalControlsStatus.Length > 0) PedalControlsStatus = meta.PedalControlsStatus;
        });

        this.WhenAnyValue(x => x.SelectedAmpType)
            .Subscribe(v =>
            {
                var idx = Array.IndexOf(AmpTypeOptions, v);
                if (idx < 0) return;
                _katanaState.AmpType.Value = idx;
            });

        this.WhenAnyValue(x => x.SelectedCabinetResonance)
            .Subscribe(v =>
            {
                var idx = Array.IndexOf(CabinetResonanceOptions, v);
                if (idx < 0) return;
                _katanaState.CabinetResonance.Value = idx;
            });

        this.WhenAnyValue(x => x.IsAmpVariation)
            .Subscribe(v => _katanaState.AmpVariation.Value = v ? 1 : 0);

        this.WhenAnyValue(x => x.ActiveWriteSync)
            .Subscribe(v =>
            {
                _logger.LogInformation("Active write sync {State}.", v ? "enabled" : "disabled");
                syncService.UpdateWriteSyncTimer();
            });
    }

    public ObservableCollection<AmpControlViewModel> AmpControls { get; } = [];
    public ObservableCollection<IBasePedal> PanelEffects { get; } = [];
    public PedalFxViewModel PedalFx { get; } = new();
    public PedalboardViewModel Pedalboard { get; }
    public PanelViewModel Panel { get; }

    public ObservableCollection<string> PanelChannels { get; } =
    [
        "Panel", "CH A1", "CH A2", "CH B1", "CH B2"
    ];

    public ObservableCollection<PanelChannelOptionViewModel> PanelChannelOptions { get; } = [];

    public static string[] AmpTypeOptions { get; } = ["ACOUSTIC", "CLEAN", "CRUNCH", "LEAD", "BROWN"];
    public static string[] CabinetResonanceOptions { get; } = ["LOW", "MIDDLE", "HIGH"];

    [Reactive] public string SelectedAmpType { get; set; } = "CLEAN";
    [Reactive] public string SelectedCabinetResonance { get; set; } = "MIDDLE";
    [Reactive] public bool IsAmpVariation { get; set; } = false;
    [Reactive] public string SelectedPanelChannel { get; set; } = "Panel";
    [Reactive] public bool ActiveWriteSync { get; set; } = true;
    [Reactive] public string PanelControlsStatus { get; set; } = "Panel controls have not been read yet.";
    [Reactive] public string PedalControlsStatus { get; set; } = "Pedal controls have not been read yet.";

    internal bool SuppressChangeTracking { get; set; }

    internal void Initialize()
    {
        this.WhenAnyValue(x => x.SelectedPanelChannel)
            .Subscribe(v =>
            {
                UpdatePanelChannelSelection();
                Pedalboard.SelectedChannel = v;
                // _syncService.TrackPanelChannelChange(v);
            });

        _syncService.PanelChannelPushed
            .Subscribe(displayName =>
            {
                SuppressChangeTracking = true;
                SelectedPanelChannel = displayName;
                SuppressChangeTracking = false;
            });

        UpdatePanelChannelSelection();
        Pedalboard.Refresh();
    }

    [RelayCommand]
    private void SelectPanelChannel(string? channel)
    {
        if (!string.IsNullOrWhiteSpace(channel)) SelectedPanelChannel = channel;
    }

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
            var channel = Utilities.ParsePanelChannelDisplay(SelectedPanelChannel);
            await _katanaSession.SelectPanelChannelAsync(channel);
            _logger.LogInformation("Selected panel channel: {Channel}", SelectedPanelChannel);
            // var patchLevelWritten = await _syncService.TryWritePatchLevelAsync();

            foreach (var effect in PanelEffects)
            {
                var confirmedValue = await _katanaSession.WriteParameterAsync(
                    effect.Definition.SwitchParameter,
                    effect.IsEnabled ? (byte)1 : (byte)0);
                effect.IsEnabled = confirmedValue != 0;
                _logger.LogInformation("{Name} confirmed {State}.", effect.DisplayName, effect.IsEnabled ? "On" : "Off");

                if (effect.Definition.TypeParameter is not null &&
                    effect.TryGetTypeValue(effect.SelectedTypeOption, out var requestedType))
                {
                    var confirmedType = await _katanaSession.WriteParameterAsync(
                        effect.Definition.TypeParameter, requestedType);
                    effect.SelectedTypeOption = effect.ToTypeOption(confirmedType);
                    _logger.LogInformation("{Name} type confirmed at {Type}.", effect.DisplayName, effect.SelectedTypeOption);
                }
            }

            _appendStatus("Panel controls updated successfully.");
            // PanelControlsStatus = patchLevelWritten
            //     ? "Panel channel, patch level, effect toggles, and effect types were written and confirmed."
            //     : "Panel channel, effect toggles, and effect types were written and confirmed. Patch level mapping is still pending.";
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

    private void UpdatePanelChannelSelection()
    {
        foreach (var option in PanelChannelOptions)
            option.IsSelected = option.DisplayName == SelectedPanelChannel;
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
