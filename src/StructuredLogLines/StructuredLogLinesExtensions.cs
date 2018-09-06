using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace StructuredLogLines
{
    public static class StructuredLogLinesExtensions
    {
        public static ILoggingBuilder AddStructuredLogLines(this ILoggingBuilder builder, Action<StructuredLogLinesLoggerConfiguration> configure, bool dispose = false)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var config = new StructuredLogLinesLoggerConfiguration();

            if (configure != null)
            {
                configure(config);
            }

            if (dispose)
            {
                builder.Services.AddSingleton<ILoggerProvider, StructuredLogLinesLoggerProvider>(services => new StructuredLogLinesLoggerProvider(config));
            }
            else
            {
                builder.AddProvider(new StructuredLogLinesLoggerProvider(config));
            }

            return builder;
        }
    }

}