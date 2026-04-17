using Avalonia.Media;

using Kataka.Domain.Midi;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public abstract partial class PedalViewModel : ViewModelBase
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

    public abstract string? SelectedTypeOption { get; set; }
    public abstract bool TryGetTypeValue(string? option, out byte value);
    public abstract string ToTypeOption(byte rawValue);
    public abstract string TypeCaption { get; }
    public abstract string Variation { get; set; }

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
