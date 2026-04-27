using Avalonia.Media;

using Kataka.App.Components.DelayPedal;
using Kataka.App.KatanaState;
using Kataka.Domain.Midi;
using Kataka.Domain.Models;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class DelayPedalViewModel : PedalViewModel
{
    private static readonly IReadOnlyDictionary<byte, string> TypeTable = KatanaTypeNameTables.DelayTypes;

    private static readonly IReadOnlyDictionary<string, byte> ReverseTypeTable =
        TypeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    private readonly AmpControlState _delayPhase;
    private readonly AmpControlState _directMix;
    private readonly AmpControlState _effectLevel;

    // Domain state fields — resolved from KatanaState based on slot
    private readonly AmpControlState _enabledState;
    private readonly AmpControlState _feedback;
    private readonly AmpControlState _feedbackPhase;
    private readonly AmpControlState _filter;

    private readonly AmpControlState _highCut;
    private readonly AmpControlState? _levelState; // null for delay2
    private readonly AmpControlState _modDepth;
    private readonly AmpControlState _modRate;
    private readonly AmpControlState _modSw;
    private readonly AmpControlState _range;
    private readonly AmpControlState _tapTime;
    private readonly AmpControlState _typeState;
    private readonly AmpControlState? _variationState; // null for delay2

    public DelayPedalViewModel(PedalPosition slot, IKatanaState katanaState) : base(
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == slot))
    {
        TypeOptions = TypeTable.Values.ToList().AsReadOnly();


        if (PedalPosition.Delay == slot)
        {
            var s = katanaState.DelayPedal;
            _enabledState = s.EnabledState;
            _typeState = s.Type;
            _variationState = s.Variation;
            _levelState = s.Level;
            _feedback = s.Feedback;
            _highCut = s.HighCut;
            _effectLevel = s.EffectLevel;
            _directMix = s.DirectMix;
            _tapTime = s.TapTime;
            _modRate = s.ModRate;
            _modDepth = s.ModDepth;
            _range = s.Range;
            _filter = s.Filter;
            _feedbackPhase = s.FeedbackPhase;
            _delayPhase = s.DelayPhase;
            _modSw = s.ModSw;
            HasVariation = true;
        }

        if (PedalPosition.Delay2 == slot)
        {
            var s = katanaState.Delay2Pedal;
            _enabledState = s.EnabledState;
            _typeState = s.Type;
            _variationState = null;
            _levelState = null;
            _feedback = s.Feedback;
            _highCut = s.HighCut;
            _effectLevel = s.EffectLevel;
            _directMix = s.DirectMix;
            _tapTime = s.TapTime;
            _modRate = s.ModRate;
            _modDepth = s.ModDepth;
            _range = s.Range;
            _filter = s.Filter;
            _feedbackPhase = s.FeedbackPhase;
            _delayPhase = s.DelayPhase;
            _modSw = s.ModSw;
            HasVariation = false;
        }

        _enabledState.ValueChanged += () => this.RaisePropertyChanged(nameof(IsEnabled));
        _typeState.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(SelectedTypeOption));
            this.RaisePropertyChanged(nameof(TypeCaption));
            this.RaisePropertyChanged(nameof(CardBackgroundBrush));
            this.RaisePropertyChanged(nameof(CardTextBrush));
            this.RaisePropertyChanged(nameof(KnobLabelBrush));
            this.RaisePropertyChanged(nameof(KnobValueBrush));
            this.RaisePropertyChanged(nameof(KnobAccentColor));
            this.RaisePropertyChanged(nameof(IsTypePan));
            this.RaisePropertyChanged(nameof(IsTypeModulateOrSde));
            this.RaisePropertyChanged(nameof(IsTypeSde3000));
        };
        _variationState?.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(Variation));
            this.RaisePropertyChanged(nameof(VariationBrush));
        };
        _feedback.ValueChanged += () => this.RaisePropertyChanged(nameof(Feedback));
        _highCut.ValueChanged += () => this.RaisePropertyChanged(nameof(HighCut));
        _effectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(EffectLevel));
        _directMix.ValueChanged += () => this.RaisePropertyChanged(nameof(DirectMix));
        _tapTime.ValueChanged += () => this.RaisePropertyChanged(nameof(TapTime));
        _modRate.ValueChanged += () => this.RaisePropertyChanged(nameof(ModRate));
        _modDepth.ValueChanged += () => this.RaisePropertyChanged(nameof(ModDepth));
        _range.ValueChanged += () => this.RaisePropertyChanged(nameof(RangeHigh));
        _filter.ValueChanged += () => this.RaisePropertyChanged(nameof(FilterOn));
        _feedbackPhase.ValueChanged += () => this.RaisePropertyChanged(nameof(FeedbackPhaseInverse));
        _delayPhase.ValueChanged += () => this.RaisePropertyChanged(nameof(DelayPhaseInverse));
        _modSw.ValueChanged += () => this.RaisePropertyChanged(nameof(ModSwOn));
    }

    /// <summary> View-only properties ────────────────────────────────────────────────────── </summary>
    public IReadOnlyList<string> TypeOptions { get; }

    public bool IsTypePan => _typeState.Value == 1;
    public bool IsTypeModulateOrSde => _typeState.Value == 9 || _typeState.Value == 10;
    public bool IsTypeSde3000 => _typeState.Value == 10;

    public bool HasTypeOptions => TypeOptions.Count > 0;

    public override IBrush CardBackgroundBrush => DelayPedalColors.GetBackgroundBrush(SelectedTypeOption);
    public bool HasVariation { get; }

    public IBrush VariationBrush => HasVariation ? GetVariationBrush(Variation) : OffVariationBrush;


    public override bool IsEnabled
    {
        get => _enabledState.Value != 0;
        set => _enabledState.Value = value ? 1 : 0;
    }

    public override string? SelectedTypeOption
    {
        get => TypeTable.TryGetValue((byte)_typeState.Value, out var name) ? name : null;
        set
        {
            if (value is not null && ReverseTypeTable.TryGetValue(value, out var byteVal))
                _typeState.Value = byteVal;
        }
    }

    public override string Variation
    {
        get => _variationState is not null ? ToVariationString(_variationState.Value) : "N/A";
        set
        {
            if (_variationState is null) return;
            var raw = value switch { "Green" => 0, "Red" => 1, "Yellow" => 2, _ => -1 };
            if (raw >= 0) _variationState.Value = raw;
        }
    }

    public override string TypeCaption => SelectedTypeOption ?? "—";

    /// <summary> Delay-specific controls ─────────────────────────────────────────────────── </summary>
    public int Feedback
    {
        get => _feedback.Value;
        set => _feedback.Value = value;
    }

    public int HighCut
    {
        get => _highCut.Value;
        set => _highCut.Value = value;
    }

    public int EffectLevel
    {
        get => _effectLevel.Value;
        set => _effectLevel.Value = value;
    }

    public int DirectMix
    {
        get => _directMix.Value;
        set => _directMix.Value = value;
    }

    public int TapTime
    {
        get => _tapTime.Value;
        set => _tapTime.Value = value;
    }

    public int ModRate
    {
        get => _modRate.Value;
        set => _modRate.Value = value;
    }

    public int ModDepth
    {
        get => _modDepth.Value;
        set => _modDepth.Value = value;
    }

    public bool RangeHigh
    {
        get => _range.Value != 0;
        set => _range.Value = value ? 1 : 0;
    }

    public bool FilterOn
    {
        get => _filter.Value != 0;
        set => _filter.Value = value ? 1 : 0;
    }

    public bool FeedbackPhaseInverse
    {
        get => _feedbackPhase.Value != 0;
        set => _feedbackPhase.Value = value ? 1 : 0;
    }

    public bool DelayPhaseInverse
    {
        get => _delayPhase.Value != 0;
        set => _delayPhase.Value = value ? 1 : 0;
    }

    public bool ModSwOn
    {
        get => _modSw.Value != 0;
        set => _modSw.Value = value ? 1 : 0;
    }

    public override bool TryGetTypeValue(string? option, out byte value)
    {
        if (option is not null && ReverseTypeTable.TryGetValue(option, out value))
            return true;
        value = 0;
        return false;
    }

    public override string ToTypeOption(byte rawValue) =>
        TypeTable.TryGetValue(rawValue, out var name) ? name : $"Type {rawValue}";
}
