using System;
using System.Collections.Generic;

using Avalonia.Media;

using Kataka.Domain.Midi;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public abstract partial class PedalViewModel : ViewModelBase, IBasePedal
{
    protected static readonly IBrush OffVariationBrush = new SolidColorBrush(Color.Parse("#35383f"));
    protected static readonly IBrush GreenVariationBrush = new SolidColorBrush(Color.Parse("#91ff92"));
    protected static readonly IBrush RedVariationBrush = new SolidColorBrush(Color.Parse("#ff6f61"));
    protected static readonly IBrush YellowVariationBrush = new SolidColorBrush(Color.Parse("#ffd65c"));

    protected PedalViewModel(KatanaPanelEffectDefinition definition)
    {
        Definition = definition;
    }

    public KatanaPanelEffectDefinition Definition { get; }

    public string DisplayName => Definition.DisplayName;

    private bool _isEnabled;
    public virtual bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }

    /// <summary>When true, property setters must not raise ParameterChanged (amp is pushing values in).</summary>
    protected bool SuppressingAmpApply { get; private set; }

    public event EventHandler<PedalParameterChangedEventArgs>? ParameterChanged;

    protected void RaiseParameterChanged(string key, int value)
    {
        if (!SuppressingAmpApply)
            ParameterChanged?.Invoke(this, new PedalParameterChangedEventArgs(key, value));
    }

    // ── IBasePedal domain members — each concrete pedal owns its type tables and config ──

    public abstract string? SelectedTypeOption { get; set; }
    public abstract bool TryGetTypeValue(string? option, out byte value);
    public abstract string ToTypeOption(byte rawValue);
    public abstract string TypeCaption { get; }
    public abstract string Variation { get; set; }
    public abstract IReadOnlyList<KatanaParameterDefinition> GetSyncParameters();

    public void ApplyAmpValues(IReadOnlyDictionary<string, int> values)
    {
        SuppressingAmpApply = true;
        try
        {
            ApplyAmpValuesCore(values);
        }
        finally
        {
            SuppressingAmpApply = false;
        }
    }

    /// <summary>
    /// Override in VMs that still use the legacy ApplyAmpValues path (ModFxPedalViewModel).
    /// Domain-migrated VMs leave this as a no-op — domain ValueChanged subscriptions handle updates.
    /// </summary>
    protected virtual void ApplyAmpValuesCore(IReadOnlyDictionary<string, int> values) { }

    protected static IBrush GetVariationBrush(string variation) => variation switch
    {
        "Green" => GreenVariationBrush,
        "Red" => RedVariationBrush,
        "Yellow" => YellowVariationBrush,
        _ => OffVariationBrush,
    };

    protected static string ToVariationString(int rawValue) => rawValue switch
    {
        0 => "Green",
        1 => "Red",
        2 => "Yellow",
        _ => "Unknown",
    };
}
