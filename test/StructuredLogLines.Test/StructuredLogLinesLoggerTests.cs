using System;
using Xunit;
using StructuredLogLines;
using Microsoft.Extensions.Logging;
using System.IO;

namespace StructuredLogLines.Test
{
    public class StructuredLogLinesLoggerTests
    {
        [Theory]
        [InlineData(LogLevel.Trace, LogLevel.Trace, true)]
        [InlineData(LogLevel.Trace, LogLevel.Debug, true)]
        [InlineData(LogLevel.Trace, LogLevel.Information, true)]
        [InlineData(LogLevel.Debug, LogLevel.Trace, false)]
        [InlineData(LogLevel.Debug, LogLevel.Debug, true)]
        [InlineData(LogLevel.Debug, LogLevel.Information, true)]
        public void IsEnabled(LogLevel minLevel, LogLevel level, bool expectEnabled)
        {
            var config = new StructuredLogLinesLoggerConfiguration();
            config.MinimumLogLevel = minLevel;
            var logger = new StructuredLogLinesLogger("test", config);

            Assert.Equal(expectEnabled, logger.IsEnabled(level));
        }

        [Theory]
        [InlineData(LogLevel.Trace, true)]
        [InlineData(LogLevel.Debug, true)]
        [InlineData(LogLevel.Information, true)]
        [InlineData(LogLevel.Warning, true)]
        [InlineData(LogLevel.Error, false)]
        [InlineData(LogLevel.Critical, false)]
        [InlineData(LogLevel.None, true)]
        public void WritesToStdOut(LogLevel level, bool expectLogOutput)
        {
            var stdOut = new StringWriter();
            var stdErr = new StringWriter();
            Console.SetOut(stdOut);
            Console.SetError(stdErr);

            var config = new StructuredLogLinesLoggerConfiguration();
            config.MinimumLogLevel = LogLevel.Trace;
            var logger = new StructuredLogLinesLogger("test", config);

            logger.Log(level, "Hello");

            var loggedToStdOut = !String.IsNullOrEmpty(stdOut.ToString()) && String.IsNullOrEmpty(stdErr.ToString());
            Assert.True(expectLogOutput == loggedToStdOut);
        }

        [Theory]
        [InlineData(LogLevel.Trace, false)]
        [InlineData(LogLevel.Debug, false)]
        [InlineData(LogLevel.Information, false)]
        [InlineData(LogLevel.Warning, false)]
        [InlineData(LogLevel.Error, true)]
        [InlineData(LogLevel.Critical, true)]
        [InlineData(LogLevel.None, false)]
        public void WritesToStdErr(LogLevel level, bool expectLogOutput)
        {
            var stdOut = new StringWriter();
            var stdErr = new StringWriter();
            Console.SetOut(stdOut);
            Console.SetError(stdErr);

            var config = new StructuredLogLinesLoggerConfiguration();
            config.MinimumLogLevel = LogLevel.Trace;
            var logger = new StructuredLogLinesLogger("test", config);

            logger.Log(level, "Hello");

            var loggedToStdErr = !String.IsNullOrEmpty(stdErr.ToString()) && String.IsNullOrEmpty(stdOut.ToString());
            Assert.True(expectLogOutput == loggedToStdErr);
        }
    }
}
