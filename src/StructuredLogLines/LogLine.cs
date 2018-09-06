using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace StructuredLogLines
{
    public class LogLine
    {
        public LogLine(LogLevel logLevel, string message)
        {
            Level = GetLogLevelString(logLevel);
            Severity = GetLogLevelSeverity(logLevel);
            Message = message;
        }

        public string Timestamp { get; } = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

        public string Level { get; private set; }

        public int Severity { get; private set; }

        public string Message { get; private set; }

        public Dictionary<string, object> Context { get; set; }

        public Dictionary<string, object> Event { get; set; }

        private string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return "debug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "error";
                case LogLevel.Critical:
                    return "critical";
            }

            return "info";
        }

        private int GetLogLevelSeverity(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return 7;
                case LogLevel.Information:
                    return 6;
                case LogLevel.Warning:
                    return 4;
                case LogLevel.Error:
                    return 3;
                case LogLevel.Critical:
                    return 2;
            }

            return 6; // info
        }
    }
}
