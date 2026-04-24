using System.Runtime.CompilerServices;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    protected CompositeDisposable Disposables { get; } = new();

    /// <summary>
    ///     Sets <paramref name="field" /> to <paramref name="value" />, fires PropertyChanging/Changed, and returns
    ///     <c>true</c> if the value actually changed. Mirrors ObservableObject.SetProperty for legacy VMs.
    /// </summary>
    protected bool ChangeProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        ((IReactiveObject)this).RaisePropertyChanging(propertyName);
        field = value;
        ((IReactiveObject)this).RaisePropertyChanged(propertyName);
        return true;
    }
}
