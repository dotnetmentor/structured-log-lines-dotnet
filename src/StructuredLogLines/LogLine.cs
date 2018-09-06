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
            return logLevel.ToString().ToLower();
        }

        private int GetLogLevelSeverity(LogLevel logLevel)
        {
            return (int)logLevel;
        }
    }
}
