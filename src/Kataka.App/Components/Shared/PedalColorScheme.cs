using Avalonia.Media;

namespace Kataka.App.ViewModels;

/// <summary>
///     Explicit color overrides for a pedal card. Any null field falls back to the dynamic
///     contrast-derived value computed from the background color in <see cref="PedalViewModel" />.
/// </summary>
public readonly record struct PedalColorScheme(
    IBrush? Background = null,
    Color? ForegroundColor = null,
    Color? AccentColor = null
);
