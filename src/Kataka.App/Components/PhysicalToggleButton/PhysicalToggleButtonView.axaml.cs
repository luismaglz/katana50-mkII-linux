using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace Kataka.App.Components.PhysicalToggleButton;

public partial class PhysicalToggleButtonView : UserControl
{
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<PhysicalToggleButtonView, bool>(nameof(IsChecked), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> DisplayNameProperty =
        AvaloniaProperty.Register<PhysicalToggleButtonView, string>(nameof(DisplayName), string.Empty);

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public string DisplayName
    {
        get => GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public PhysicalToggleButtonView() => InitializeComponent();
}
