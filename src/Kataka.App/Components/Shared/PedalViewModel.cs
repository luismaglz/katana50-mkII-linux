using Avalonia;
using Avalonia.Media;

using Kataka.App.Controls;
using Kataka.Domain.Midi;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public abstract class PedalViewModel : ViewModelBase
{
    public static readonly IBrush DefaultCardBackground = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
        GradientStops = { new GradientStop(Color.Parse("#2E3138"), 0), new GradientStop(Color.Parse("#1C1F24"), 1) }
    };

    protected static readonly IBrush OffVariationBrush = new SolidColorBrush(Color.Parse("#35383f"));
    protected static readonly IBrush GreenVariationBrush = new SolidColorBrush(Color.Parse("#91ff92"));
    protected static readonly IBrush RedVariationBrush = new SolidColorBrush(Color.Parse("#ff6f61"));
    protected static readonly IBrush YellowVariationBrush = new SolidColorBrush(Color.Parse("#ffd65c"));

    private bool _isEnabled;

    protected PedalViewModel(KatanaPanelEffectDefinition definition)
    {
        Definition = definition;
    }

    public virtual IBrush CardBackgroundBrush => DefaultCardBackground;

    // Extracts a representative solid Color from any brush for color math.
    protected Color CardBgColor => CardBackgroundBrush switch
    {
        SolidColorBrush s => s.Color,
        LinearGradientBrush g when g.GradientStops.Count > 0 => g.GradientStops[0].Color,
        _ => Color.Parse("#2E3138")
    };

    public virtual IBrush CardTextBrush => new SolidColorBrush(ColorUtils.GetBestContrast(CardBgColor));
    public virtual IBrush KnobLabelBrush => new SolidColorBrush(ColorUtils.DeriveLabel(CardBgColor));
    public virtual IBrush KnobValueBrush => new SolidColorBrush(ColorUtils.DeriveValue(CardBgColor));
    public virtual Color KnobAccentColor => ColorUtils.DeriveAccent(CardBgColor);

    public KatanaPanelEffectDefinition Definition { get; }

    public string DisplayName => Definition.DisplayName.ToUpper();

    public virtual bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }

    public abstract string? SelectedTypeOption { get; set; }
    public abstract string TypeCaption { get; }
    public abstract string Variation { get; set; }
    public abstract bool TryGetTypeValue(string? option, out byte value);
    public abstract string ToTypeOption(byte rawValue);

    protected static IBrush GetVariationBrush(string variation) => variation switch
    {
        "Green" => GreenVariationBrush,
        "Red" => RedVariationBrush,
        "Yellow" => YellowVariationBrush,
        _ => OffVariationBrush
    };

    protected static string ToVariationString(int rawValue) => rawValue switch
    {
        0 => "Green",
        1 => "Red",
        2 => "Yellow",
        _ => "Unknown"
    };
}
