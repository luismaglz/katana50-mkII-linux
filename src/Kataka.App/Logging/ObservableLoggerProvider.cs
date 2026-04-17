using System;
using System.Reactive.Subjects;

using Microsoft.Extensions.Logging;

namespace Kataka.App.Logging;

/// <summary>
/// Logging provider that publishes formatted log lines to an observable so any subscriber
/// (e.g. DiagnosticsViewModel) can display them without a direct ViewModel dependency.
/// </summary>
public sealed class ObservableLoggerProvider : ILoggerProvider
{
    private readonly Subject<string> _subject = new();

    public IObservable<string> LogMessages => _subject;

    public ILogger CreateLogger(string categoryName) => new ObservableLogger(_subject);

    public void Dispose() => _subject.OnCompleted();

    private sealed class ObservableLogger(Subject<string> subject) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            var message = formatter(state, exception);
            var line = $"[{DateTimeOffset.Now:HH:mm:ss}] {message}";
            if (exception is not null)
                line += Environment.NewLine + exception;
            subject.OnNext(line);
        }
    }
}
