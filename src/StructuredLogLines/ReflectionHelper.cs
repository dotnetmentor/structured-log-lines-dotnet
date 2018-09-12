using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StructuredLogLines
{
    internal class ReflectionHelper
    {
        public static FieldInfo FormattedLogValuesValuesField = typeof(Microsoft.Extensions.Logging.Internal.FormattedLogValues)
                    .GetRuntimeFields()
                    .Where(f => f.IsPrivate && !f.IsStatic && f.Name == "_values")
                    .FirstOrDefault();

        // In answer to the issue of not being able to access all the arguments passed when using ILogger extensions like LogInformation etc.
        // https://github.com/aspnet/Logging/issues/533
        public static object[] TryGetAllFormattedLogValues<TState>(TState state)
        {
            var flv = state as Microsoft.Extensions.Logging.Internal.FormattedLogValues;
            if (flv != null)
            {
                var count = flv.Count;
                var skip = count > 0 ? count - 1 : 0;
                var rawValues = (ReflectionHelper.FormattedLogValuesValuesField.GetValue(flv) as object[]).Skip(skip).ToArray();
                var result = new List<object> { flv.ToString() };
                result.AddRange(rawValues);
                return result.ToArray();
            }
            return null;
        }
    }
}