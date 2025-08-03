using Microsoft.Extensions.Logging;

namespace ProntoReserva.Tests.Integration.Helpers;

public class InMemoryLogger<T> : ILogger<T>, IDisposable
{
    public readonly List<string> LogMessages = new();

    public IDisposable BeginScope<TState>(TState state) => this;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);
        LogMessages.Add(message);
    }

    public void Dispose() { }
}