using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StructuredLogLines
{
    public class StructuredLogLinesLogger : ILogger
    {
        public StructuredLogLinesLogger(string name, StructuredLogLinesLoggerConfiguration config)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Name = name;
            Config = config;
            ScopeProvider = new LoggerExternalScopeProvider();
        }

        internal IExternalScopeProvider ScopeProvider { get; }

        internal StructuredLogLinesLoggerConfiguration Config { get; }

        public string Name { get; }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= Config.MinimumLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (state == null && exception == null)
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message))
            {
                message = string.Empty;
            }

            var logLine = new LogLine(logLevel, message);

            ScopeProvider?.ForEachScope((scope, data) =>
            {
                var scopeType = scope.GetType();
                if (scopeType.IsClass)
                {
                    var path = Config.JsonPathResolver.Invoke(JsonPathPrefix.Context, scope);
                    logLine.WithContext(path, scope);
                }
                else
                {
                    IDictionary<string, object> dictionary = null;

                    if (scope is IEnumerable<KeyValuePair<string, object>> properties)
                    {
                        dictionary = properties.ToDictionary(p => p.Key, p => p.Value);
                    }

                    if (scope is IDictionary<string, object> kvps)
                    {
                        dictionary = kvps;
                    }

                    if (dictionary != null)
                    {
                        data.Add(dictionary);
                    }
                    else
                    {
                        data.Add(scope);
                    }

                    var path = Config.JsonPathResolver.Invoke(JsonPathPrefix.Context, scope);
                    logLine.WithContext(path, data);
                }
            }, new List<object>());

            if (exception != null)
            {
                var path = Config.JsonPathResolver.Invoke(JsonPathPrefix.Event, exception);
                var error = Config.ExcpetionConverter.Invoke(exception);
                logLine.WithEvent(path, error);
            }
            else if (state != null)
            {
                var formattedLogValues = ReflectionHelper.GetAllFormattedLogValues(state);
                if (formattedLogValues != null)
                {
                    foreach (var item in formattedLogValues)
                    {
                        var path = Config.JsonPathResolver.Invoke(JsonPathPrefix.Event, item);
                        logLine.WithEvent(path, item);
                    }
                }
            }

            var json = JsonConvert.SerializeObject(logLine, Config.UsePretty ? Formatting.Indented : Formatting.None, Config.JsonSerializerSettings);
            var color = Console.ForegroundColor;

            if (Config.UseColor)
            {
                Console.ForegroundColor = GetLogLevelColor(logLevel);
            }

            switch (logLevel)
            {
                case LogLevel.Error:
                case LogLevel.Critical:
                    Console.Error.WriteLine(json);
                    break;
                default:
                    Console.Out.WriteLine(json);
                    break;
            }

            if (Config.UseColor)
            {
                Console.ForegroundColor = color;
            }
        }

        private ConsoleColor GetLogLevelColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return ConsoleColor.Gray;
                case LogLevel.Debug:
                    return ConsoleColor.White;
                case LogLevel.Information:
                    return ConsoleColor.Green;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                case LogLevel.Critical:
                    return ConsoleColor.DarkRed;
            }

            return ConsoleColor.White;
        }
    }
}