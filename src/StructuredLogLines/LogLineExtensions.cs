using System;
using System.Collections.Generic;
using System.Linq;

namespace StructuredLogLines
{
    public static class LogLineExtensions
    {
        public static void WithContext<TValue>(this LogLine logLine, string path, TValue value)
        {
            if (String.IsNullOrEmpty(path))
            {
                return;
            }

            if (logLine.Context == null && value != null)
            {
                logLine.Context = new Dictionary<string, object>();
            }

            ApplyDeep(logLine.Context, path.Split('.').ToList(), value);
        }

        public static void WithEvent<TValue>(this LogLine logLine, string path, TValue value)
        {
            if (String.IsNullOrEmpty(path))
            {
                return;
            }

            if (logLine.Event == null && value != null)
            {
                logLine.Event = new Dictionary<string, object>();
            }

            ApplyDeep(logLine.Event, path.Split('.').ToList(), value);
        }

        private static void ApplyDeep<TValue>(Dictionary<string, object> dictionary, IList<string> keys, TValue value)
        {
            var key = keys.FirstOrDefault();

            if (String.IsNullOrEmpty(key) || value == null)
            {
                return;
            }

            if (!dictionary.ContainsKey(key) && keys.Count() > 1)
            {
                dictionary[key] = new Dictionary<string, object>();
            }

            if (keys.Count() > 1)
            {
                ApplyDeep(dictionary[key] as Dictionary<string, object>, keys.Skip(1).ToList(), value);
            }
            else
            {
                dictionary[key] = value;
            }
        }
    }
}
