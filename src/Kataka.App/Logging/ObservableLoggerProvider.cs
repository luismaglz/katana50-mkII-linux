using System.Reactive.Subjects;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kataka.App.Logging;

/// <summary>
///     Logging provider that publishes formatted log lines to an observable so any subscriber
///     (e.g. DiagnosticsViewModel) can display them without a direct ViewModel dependency.
/// </summary>
public sealed class ObservableLoggerProvider : ILoggerProvider
{
    private readonly IConfigurationSection? _logLevelSection;
    private readonly Subject<string> _subject = new();

    public ObservableLoggerProvider(IConfiguration? config = null)
    {
        _logLevelSection = config?.GetSection("Logging:LogLevel");
    }

    public IObservable<string> LogMessages => _subject;

    public ILogger CreateLogger(string categoryName) =>
        new ObservableLogger(_subject, ResolveMinLevel(categoryName));

    public void Dispose() => _subject.OnCompleted();

    private LogLevel ResolveMinLevel(string categoryName)
    {
        if (_logLevelSection is null) return LogLevel.Information;

        // Walk from most-specific category prefix down to "Default"
        var name = categoryName;
        while (name.Length > 0)
        {
            if (TryGet(name, out var level)) return level;
            var dot = name.LastIndexOf('.');
            name = dot < 0 ? string.Empty : name[..dot];
        }

        return TryGet("Default", out var def) ? def : LogLevel.Information;
    }

    private bool TryGet(string key, out LogLevel level)
    {
        var value = _logLevelSection![key];
        return Enum.TryParse(value, true, out level);
    }

    private sealed class ObservableLogger(Subject<string> subject, LogLevel minLevel) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= minLevel;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            var message = formatter(state, exception);
            var line = $"[{DateTimeOffset.Now:HH:mm:ss}] [{logLevel}] {message}";
            if (exception is not null)
                line += Environment.NewLine + exception;
            subject.OnNext(line);
        }
    }
}
