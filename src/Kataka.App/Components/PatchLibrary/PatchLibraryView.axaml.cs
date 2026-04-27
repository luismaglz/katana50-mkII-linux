using Avalonia.Controls;

using Kataka.App.ViewModels;

namespace Kataka.App.Views;

public partial class PatchLibraryView : UserControl
{
    public PatchLibraryView()
    {
        InitializeComponent();
    }

    private void PatchList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is PatchLibraryViewModel vm && sender is ListBox lb && lb.SelectedIndex >= 0)
            vm.SelectPatchCommand.Execute(lb.SelectedIndex);
    }
}
