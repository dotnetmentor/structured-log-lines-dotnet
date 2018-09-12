using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace StructuredLogLines
{
    public interface ILoggerConventions
    {
        string TimestampPropertyName { get; }

        string MessagePropertyName { get; }

        string LevelPropertyName { get; }

        string SevertiyPropertyName { get; }

        string ContextPropertyName { get; }

        string EventPropertyName { get; }

        Func<string> TimestampResolver { get; }

        Func<LogLevel, string> LevelResolver { get; }

        Func<LogLevel, int> SeverityResolver { get; }

        Func<object, string> ContextPathResolver { get; }

        Func<object, string> EventPathResolver { get; }

        Func<object, object> ContextDataResolver { get; }

        Func<object, object> EventDataResolver { get; }

        Func<object, object, object> PropertyConflictResolver { get; }

        Action<StructuredLogLinesLogger> OnLoggerCreated { get; }

        JsonSerializerSettings JsonSerializerSettings { get; }
    }
}