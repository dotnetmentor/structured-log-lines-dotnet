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
        public static object[] GetAllFormattedLogValues<TState>(TState state)
        {
            var flv = state as Microsoft.Extensions.Logging.Internal.FormattedLogValues;
            if (flv != null)
            {
                return ReflectionHelper.FormattedLogValuesValuesField.GetValue(flv) as object[];
            }
            return null;
        }
    }
}