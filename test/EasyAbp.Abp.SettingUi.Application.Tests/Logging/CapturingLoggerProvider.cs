using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace EasyAbp.Abp.SettingUi.Logging
{
    /// <summary>
    /// Keeps every log entry written through the logger factory, for assertions.
    /// </summary>
    public class CapturingLoggerProvider : ILoggerProvider
    {
        public ConcurrentQueue<(LogLevel Level, string Message)> Entries { get; } = new();

        public ILogger CreateLogger(string categoryName)
        {
            return new CapturingLogger(this);
        }

        public void Dispose()
        {
        }

        private class CapturingLogger : ILogger
        {
            private readonly CapturingLoggerProvider _provider;

            public CapturingLogger(CapturingLoggerProvider provider)
            {
                _provider = provider;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                _provider.Entries.Enqueue((logLevel, formatter(state, exception)));
            }
        }
    }
}
