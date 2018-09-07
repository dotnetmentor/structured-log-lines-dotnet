using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StructuredLogLines
{
    public class StructuredLogLinesLoggerConfiguration
    {
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;

        public bool UseColor { get; set; } = false;

        public bool UsePretty { get; set; } = false;

        internal ILoggerConventions Conventions { get; set; } = new DefaultLoggerConventions();

        public void With<TConventions>() where TConventions : ILoggerConventions, new() => Conventions = new TConventions();
    }
}
