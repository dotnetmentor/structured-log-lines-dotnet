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

        internal Action<StructuredLogLinesLogger> LoggerCreatedHandler { get; private set; } = null;

        internal Func<JsonPathPrefix, object, string> JsonPathResolver { get; private set; } = Strategy.CamelCaseClassNameToJsonPathStrategy;

        internal Func<Exception, object> ExcpetionConverter { get; private set; } = Strategy.ExceptionToErrorObjectStrategy;

        internal JsonSerializerSettings JsonSerializerSettings { get; private set; } = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver { },
            NullValueHandling = NullValueHandling.Ignore
        };

        public StructuredLogLinesLoggerConfiguration WithLoggerCreatedHandler(Action<StructuredLogLinesLogger> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            LoggerCreatedHandler = handler;

            return this;
        }

        public StructuredLogLinesLoggerConfiguration WithJsonPathResolver(Func<JsonPathPrefix, object, string> resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            JsonPathResolver = resolver;

            return this;
        }

        public StructuredLogLinesLoggerConfiguration WithExceptionConverter(Func<Exception, object> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            ExcpetionConverter = converter;

            return this;
        }

        public StructuredLogLinesLoggerConfiguration WithJsonSerializerSettings(Action<JsonSerializerSettings> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            configure(JsonSerializerSettings);

            return this;
        }
    }
}
