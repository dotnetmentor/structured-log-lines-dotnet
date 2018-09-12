using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StructuredLogLines
{
    public class LogLine : Dictionary<string, object>
    {

        private readonly ILoggerConventions _conventions;

        public LogLine(ILoggerConventions conventions, LogLevel logLevel, string message)
        {
            if (conventions == null)
            {
                throw new ArgumentNullException(nameof(conventions));
            }

            _conventions = conventions;

            AddProperty(conventions.TimestampPropertyName, conventions.TimestampResolver?.Invoke());
            AddProperty(conventions.LevelPropertyName, conventions.LevelResolver?.Invoke(logLevel));
            AddProperty(conventions.SevertiyPropertyName, conventions.SeverityResolver?.Invoke(logLevel));
            AddProperty(conventions.MessagePropertyName, message);
        }

        public void WithContext<TValue>(TValue value)
        {
            var data = _conventions.ContextDataResolver?.Invoke(value) ?? null;
            if (data == null)
            {
                return;
            }

            var path = _conventions.ContextPathResolver?.Invoke(data) ?? null;
            if (String.IsNullOrEmpty(path))
            {
                return;
            }

            var propertyName = _conventions.ContextPropertyName;
            if (!this.ContainsKey(propertyName) && value != null)
            {
                this[propertyName] = new Dictionary<string, object>();
            }

            ApplyDeep((Dictionary<string, object>)this[propertyName], path.Split('.').ToList(), data);
        }

        public void WithEvent<TValue>(TValue value)
        {
            var data = _conventions.EventDataResolver?.Invoke(value) ?? null;
            if (data == null)
            {
                return;
            }

            var path = _conventions.EventPathResolver?.Invoke(data) ?? null;
            if (String.IsNullOrEmpty(path))
            {
                return;
            }

            var propertyName = _conventions.EventPropertyName;
            if (!this.ContainsKey(propertyName) && value != null)
            {
                this[propertyName] = new Dictionary<string, object>();
            }

            ApplyDeep((Dictionary<string, object>)this[propertyName], path.Split('.').ToList(), data);
        }

        private void ApplyDeep<TValue>(Dictionary<string, object> dictionary, IList<string> keys, TValue value)
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
                if (dictionary.TryGetValue(key, out object currentValue))
                {
                    dictionary[key] = _conventions.PropertyConflictResolver?.Invoke(currentValue, value) ?? value;
                }
                else
                {
                    dictionary[key] = value;
                }
            }
        }

        private void AddProperty(string key, object value)
        {
            if (!String.IsNullOrEmpty(key) && value != null)
            {
                this[key] = value;
            }
        }
    }
}
