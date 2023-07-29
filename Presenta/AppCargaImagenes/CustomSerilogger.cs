using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCargaImagenes;

public class CustomSerilogger : Microsoft.Extensions.Logging.ILogger
{
    private readonly Serilog.ILogger _logger;
    public CustomSerilogger(Serilog.ILogger logger)
    { _logger = logger; }

#pragma warning disable CS8633 // La nulabiklidad e las restricciones del parámetro TState del método
    public IDisposable BeginScope<TState>(TState state) => default!;
#pragma warning restore CS8633 // La nulabiklidad e las restricciones del parámetro TState del método

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) { return _logger.IsEnabled(LogLevelToLogEventLevel(logLevel)); }
    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        if (state != null)
            _logger.Write(LogLevelToLogEventLevel(logLevel), exception, state.ToString() ?? "");
        else
            _logger.Write(LogLevelToLogEventLevel(logLevel), exception, "");
    }

    private static LogEventLevel LogLevelToLogEventLevel(Microsoft.Extensions.Logging.LogLevel loglevel)
    {
        switch (loglevel)
        {
            case Microsoft.Extensions.Logging.LogLevel.Debug:
                return LogEventLevel.Debug;
            case Microsoft.Extensions.Logging.LogLevel.Information:
                return LogEventLevel.Information;
            case Microsoft.Extensions.Logging.LogLevel.Warning:
                return LogEventLevel.Warning;
            case Microsoft.Extensions.Logging.LogLevel.Error:
                return LogEventLevel.Error;
            case Microsoft.Extensions.Logging.LogLevel.Critical:
                return LogEventLevel.Fatal;
            case Microsoft.Extensions.Logging.LogLevel.None:
                return LogEventLevel.Verbose;
            case Microsoft.Extensions.Logging.LogLevel.Trace:
                return LogEventLevel.Verbose;
            default:
                break;
        }
        return LogEventLevel.Verbose;
    }
}
