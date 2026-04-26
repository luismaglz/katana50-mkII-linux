using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Kataka.App.Components.Pedalboard;

public partial class PedalboardHardwareCard : UserControl
{
    public static readonly StyledProperty<bool> ShowConnectorProperty =
        AvaloniaProperty.Register<PedalboardHardwareCard, bool>(nameof(ShowConnector));

    public static readonly StyledProperty<Bitmap?> ImagePathProperty =
        AvaloniaProperty.Register<PedalboardHardwareCard, Bitmap?>(nameof(ImagePath));

    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<PedalboardHardwareCard, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<IBrush> AccentBrushProperty =
        AvaloniaProperty.Register<PedalboardHardwareCard, IBrush>(nameof(AccentBrush), Brushes.Gray);

    public static readonly StyledProperty<IBrush> CardBorderBrushProperty =
        AvaloniaProperty.Register<PedalboardHardwareCard, IBrush>(nameof(CardBorderBrush), Brushes.Gray);

    public static readonly StyledProperty<IBrush> LabelBrushProperty =
        AvaloniaProperty.Register<PedalboardHardwareCard, IBrush>(nameof(LabelBrush), Brushes.White);

    public static readonly StyledProperty<double> CardWidthProperty =
        AvaloniaProperty.Register<PedalboardHardwareCard, double>(nameof(CardWidth), 100);

    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<PedalboardHardwareCard, double>(nameof(LabelFontSize), 11);

    public PedalboardHardwareCard()
    {
        InitializeComponent();
    }

    public bool ShowConnector
    {
        get => GetValue(ShowConnectorProperty);
        set => SetValue(ShowConnectorProperty, value);
    }

    public Bitmap? ImagePath
    {
        get => GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public IBrush AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public IBrush CardBorderBrush
    {
        get => GetValue(CardBorderBrushProperty);
        set => SetValue(CardBorderBrushProperty, value);
    }

    public IBrush LabelBrush
    {
        get => GetValue(LabelBrushProperty);
        set => SetValue(LabelBrushProperty, value);
    }

    public double CardWidth
    {
        get => GetValue(CardWidthProperty);
        set => SetValue(CardWidthProperty, value);
    }

    public double LabelFontSize
    {
        get => GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, value);
    }
}
