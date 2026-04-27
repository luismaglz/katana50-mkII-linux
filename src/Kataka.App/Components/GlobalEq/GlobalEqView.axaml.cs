using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kataka.App.Views;

public partial class GlobalEqView : UserControl
{
    public GlobalEqView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
