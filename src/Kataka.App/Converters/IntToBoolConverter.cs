using System.Globalization;

using Avalonia.Data.Converters;

namespace Kataka.App.Converters;

public sealed class IntToBoolConverter : IValueConverter
{
    public static readonly IntToBoolConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i && i != 0;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? 1 : 0;
}
