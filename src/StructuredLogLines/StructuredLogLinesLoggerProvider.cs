using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace StructuredLogLines
{
    [ProviderAlias("StructuredLogLiness")]
    public class StructuredLogLinesLoggerProvider : ILoggerProvider
    {
        private readonly StructuredLogLinesLoggerConfiguration _config;

        private readonly ConcurrentDictionary<string, StructuredLogLinesLogger> _loggers = new ConcurrentDictionary<string, StructuredLogLinesLogger>();

        public StructuredLogLinesLoggerProvider(StructuredLogLinesLoggerConfiguration config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name =>
            {
                var logger = new StructuredLogLinesLogger(name, _config);
                if (_config.LoggerCreatedHandler != null)
                {
                    _config.LoggerCreatedHandler.Invoke(logger);
                }
                return logger;
            });
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}