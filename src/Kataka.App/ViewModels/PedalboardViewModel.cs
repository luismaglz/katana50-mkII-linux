using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Kataka.App.ViewModels;

public partial class PedalboardViewModel : ViewModelBase
{
    private readonly IReadOnlyDictionary<string, PedalViewModel> _pedalsByKey;
    private readonly PedalFxViewModel _pedalFx;
    private readonly Func<string> _getSelectedChannel;

    public static string[] ChainPatternNames { get; } =
        ["CHAIN 1", "CHAIN 2-1", "CHAIN 3-1", "CHAIN 4-1", "CHAIN 2-2", "CHAIN 3-2", "CHAIN 4-2"];

    private static readonly string[][] ChainBeforeAmp =
    [
        ["booster"],
        ["booster", "mod"],
        ["booster", "mod", "fx"],
        ["booster", "mod", "fx", "delay", "delay2"],
        ["mod", "booster"],
        ["mod", "booster", "fx"],
        ["mod", "booster", "fx", "delay", "delay2"],
    ];

    private static readonly string[][] ChainAfterAmp =
    [
        ["mod", "fx", "delay", "delay2", "reverb"],
        ["fx", "delay", "delay2", "reverb"],
        ["delay", "delay2", "reverb"],
        ["reverb"],
        ["fx", "delay", "delay2", "reverb"],
        ["delay", "delay2", "reverb"],
        ["reverb"],
    ];

    public PedalboardViewModel(
        IReadOnlyDictionary<string, PedalViewModel> pedalsByKey,
        PedalFxViewModel pedalFx,
        Func<string> getSelectedChannel)
    {
        _pedalsByKey = pedalsByKey;
        _pedalFx = pedalFx;
        _getSelectedChannel = getSelectedChannel;

        // Refresh when PedalFx position or type changes.
        _pedalFx.PropertyChanged += (_, _) => Refresh();
    }

    public ObservableCollection<PedalboardItemViewModel> PedalboardItems { get; } = [];

    [ObservableProperty]
    public partial int SelectedChainPattern { get; set; } = 2;

    partial void OnSelectedChainPatternChanged(int value) => Refresh();

    [ObservableProperty]
    public partial string? SelectedPedalboardKey { get; set; }

    partial void OnSelectedPedalboardKeyChanged(string? value)
    {
        OnPropertyChanged(nameof(SelectedPedalDetail));
        OnPropertyChanged(nameof(SelectedIsPedalFx));
        OnPropertyChanged(nameof(SelectedHasDetail));
        OnPropertyChanged(nameof(SelectedDetailTitle));

        foreach (var item in PedalboardItems)
            item.IsSelected = string.Equals(item.Key, value, StringComparison.Ordinal);
    }

    public PedalViewModel? SelectedPedalDetail =>
        SelectedPedalboardKey is not null &&
        _pedalsByKey.TryGetValue(SelectedPedalboardKey, out var vm) ? vm : null;

    public bool SelectedIsPedalFx =>
        string.Equals(SelectedPedalboardKey, "pedal-fx", StringComparison.Ordinal);

    public bool SelectedHasDetail => SelectedPedalDetail is not null || SelectedIsPedalFx;

    public string SelectedDetailTitle =>
        SelectedPedalDetail?.DisplayName ??
        (SelectedIsPedalFx ? "Pedal FX" : "Select a pedal in the chain below to edit its settings");

    [RelayCommand]
    private void SelectPedalboardItem(string? key)
    {
        SelectedPedalboardKey = string.Equals(key, SelectedPedalboardKey, StringComparison.Ordinal) ? null : key;
    }

    [RelayCommand]
    private void TogglePedalboardItem(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return;

        if (string.Equals(key, "pedal-fx", StringComparison.Ordinal))
        {
            _pedalFx.IsEnabled = !_pedalFx.IsEnabled;
            Refresh();
            return;
        }

        if (_pedalsByKey.TryGetValue(key, out var effect))
        {
            effect.IsEnabled = !effect.IsEnabled;
            Refresh();
        }
    }

    public void Refresh()
    {
        var items = new List<PedalboardItemViewModel>
        {
            new()
            {
                Key = "input",
                DisplayName = "INPUT",
                Detail = "Guitar",
                IsActive = true,
                IsEndpoint = true,
                Family = "io",
            },
        };

        var chainIdx = Math.Clamp(SelectedChainPattern, 0, ChainBeforeAmp.Length - 1);
        AddPedalsByKeys(items, ChainBeforeAmp[chainIdx]);

        if (string.Equals(_pedalFx.SelectedPositionOption, "Input", StringComparison.Ordinal))
            items.Add(CreatePedalFxBoardItem(items.Count > 0));

        items.Add(new PedalboardItemViewModel
        {
            Key = "amp",
            DisplayName = "AMP",
            Detail = _getSelectedChannel(),
            IsActive = true,
            IsAmp = true,
            IsConnectedFromPrevious = items.Count > 0,
            Family = "amp",
        });

        if (string.Equals(_pedalFx.SelectedPositionOption, "Post Amp", StringComparison.Ordinal))
            items.Add(CreatePedalFxBoardItem(items.Count > 0));

        AddPedalsByKeys(items, ChainAfterAmp[chainIdx]);

        items.Add(new PedalboardItemViewModel
        {
            Key = "output",
            DisplayName = "OUTPUT",
            Detail = "Speaker / Rec Out",
            IsActive = true,
            IsEndpoint = true,
            IsConnectedFromPrevious = items.Count > 0,
            Family = "io",
        });

        PedalboardItems.Clear();
        foreach (var item in items)
            PedalboardItems.Add(item);

        // Re-sync selection state after rebuild.
        OnSelectedPedalboardKeyChanged(SelectedPedalboardKey);
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
                Detail = effect.Definition.TypeParameter is null
                    ? effect.VariationCaption
                    : $"{effect.TypeCaption} / {effect.VariationCaption}",
                IsActive = effect.IsEnabled,
                IsConnectedFromPrevious = items.Count > 0,
                CanToggle = true,
                Family = effect.Definition.Key,
                PanelEffect = effect,
            });
        }
    }

    private PedalboardItemViewModel CreatePedalFxBoardItem(bool isConnectedFromPrevious) =>
        new()
        {
            Key = "pedal-fx",
            DisplayName = "PEDAL FX",
            Detail = $"{_pedalFx.SelectedTypeOption} / {_pedalFx.SelectedPositionOption}",
            IsActive = _pedalFx.IsEnabled,
            IsConnectedFromPrevious = isConnectedFromPrevious,
            CanToggle = true,
            Family = "pedal",
        };
}
