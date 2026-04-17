using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class PedalboardViewModel : ViewModelBase
{
    private readonly IReadOnlyDictionary<string, PedalViewModel> _pedalsByKey;
    private PedalboardItemViewModel? _ampItem;

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
        string initialChannel)
    {
        _pedalsByKey = pedalsByKey;
        SelectedChannel = initialChannel;

        this.WhenAnyValue(x => x.SelectedChannel)
            .Subscribe(v => { if (_ampItem is not null) _ampItem.Detail = v; })
            .DisposeWith(Disposables);

        this.WhenAnyValue(x => x.SelectedChainPattern)
            .Subscribe(_ => Refresh())
            .DisposeWith(Disposables);

    }

    public ObservableCollection<PedalboardItemViewModel> PedalboardItems { get; } = [];

    [Reactive]
    public string SelectedChannel { get; set; } = string.Empty;

    [Reactive]
    public int SelectedChainPattern { get; set; } = 2;

    public void Refresh()
    {
        var items = new List<PedalboardItemViewModel>
        {
            new()
            {
                Key = "input",
                DisplayName = "INPUT",
                Detail = "Guitar",
                IsEndpoint = true,
                Family = "io",
            },
        };

        var chainIdx = Math.Clamp(SelectedChainPattern, 0, ChainBeforeAmp.Length - 1);
        AddPedalsByKeys(items, ChainBeforeAmp[chainIdx]);

        _ampItem = new PedalboardItemViewModel
        {
            Key = "amp",
            DisplayName = "AMP",
            Detail = SelectedChannel,
            IsAmp = true,
            IsConnectedFromPrevious = items.Count > 0,
            Family = "amp",
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
            Family = "io",
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
                PanelEffect = effect,
            });
        }
    }
}
