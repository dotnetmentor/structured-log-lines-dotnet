using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StructuredLogLines
{
    public class DefaultLoggerConventions : ILoggerConventions
    {
        private static ConcurrentDictionary<Type, string> _contextPathCache = new ConcurrentDictionary<Type, string>();

        private static ConcurrentDictionary<Type, string> _eventPathCache = new ConcurrentDictionary<Type, string>();

        public virtual string TimestampPropertyName => "Timestamp";

        public virtual string MessagePropertyName => "Message";

        public virtual string LevelPropertyName => "Level";

        public virtual string SevertiyPropertyName => "Severity";

        public virtual string ContextPropertyName => "Context";

        public virtual string EventPropertyName => "Event";

        public virtual Func<string> TimestampResolver { get; } = () => DateTimeOffset.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

        public Func<LogLevel, string> LevelResolver { get; } = logLevel => logLevel.ToString().ToLower();

        public Func<LogLevel, int> SeverityResolver { get; } = logLevel => (int)logLevel;

        public virtual Func<object, string> ContextPathResolver { get; } = ctx =>
        {
            var path = _contextPathCache.GetOrAdd(ctx.GetType(), t =>
            {
                string p = null;
                if (t.IsClass && !t.FullName.StartsWith("System."))
                {
                    p = String.Join(".", SplitCamelCase(t.Name));
                }
                return p;
            });

            return path;
        };

        public virtual Func<object, string> EventPathResolver { get; } = evt =>
        {
            var path = _eventPathCache.GetOrAdd(evt.GetType(), t =>
            {
                string p = null;
                if (t.IsClass && !t.FullName.StartsWith("System."))
                {
                    p = String.Join(".", SplitCamelCase(t.Name));
                }
                return p;
            });

            return path;
        };

        public virtual Func<object, object> ContextDataResolver { get; } = ctx =>
        {
            var ctxType = ctx.GetType();
            if (ctxType.IsClass && !ctxType.FullName.StartsWith("System."))
            {
                return ctx;
            }

            return new Scope { ctx };
        };

        public virtual Func<object, object> EventDataResolver { get; } = evt =>
        {
            if (evt is Exception exception)
            {
                var exceptionType = exception.GetType();
                return new Error
                {
                    Type = exceptionType.Name,
                    Message = exception.Message,
                    Details = exceptionType.AssemblyQualifiedName,
                    Stacktrace = (exception.StackTrace ?? String.Empty).Split(Environment.NewLine.ToCharArray())
                };
            }

            return evt;
        };

        public virtual Func<object, object, object> PropertyConflictResolver { get; } = (currentValue, newValue) =>
        {
            if (currentValue is Scope currentScope && newValue is Scope newScope)
            {
                var resolution = new Scope();
                resolution.AddRange(currentScope);
                resolution.AddRange(newScope);
                return resolution;
            }

            return newValue;
        };

        public virtual Action<StructuredLogLinesLogger> OnLoggerCreated => null;

        public virtual JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver { },
            NullValueHandling = NullValueHandling.Ignore
        };

        private class Scope : List<object> { }

        private class Error
        {
            public string Type { get; set; }
            public string Message { get; set; }
            public string Details { get; set; }
            public string[] Stacktrace { get; set; }
        }

        private static string[] SplitCamelCase(string source)
        {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }
    }
}