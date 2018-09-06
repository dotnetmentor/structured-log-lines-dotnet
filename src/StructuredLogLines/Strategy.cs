using System;
using System.Text.RegularExpressions;

namespace StructuredLogLines
{
    public class Strategy
    {
        public static Func<Exception, object> ExceptionToErrorObjectStrategy = exception =>
        {
            var exceptionType = exception.GetType();
            return new
            {
                Type = exceptionType.Name,
                Message = exception.Message,
                Details = exceptionType.AssemblyQualifiedName,
                Stacktrace = (exception.StackTrace ?? String.Empty).Split(Environment.NewLine.ToCharArray())
            };
        };

        public static Func<JsonPathPrefix, object, string> CamelCaseClassNameToJsonPathStrategy = (prefix, obj) =>
        {
            if (obj == null)
            {
                return null;
            }

            if (prefix == JsonPathPrefix.Context)
            {
                var type = obj.GetType();
                if (type.IsClass)
                {
                    return String.Join(".", SplitCamelCase(type.Name));
                }

                return "custom.scope";
            }

            if (prefix == JsonPathPrefix.Event)
            {
                if (obj is Exception)
                {
                    return "error";
                }

                var type = obj.GetType();
                if (type.IsClass)
                {
                    return String.Join(".", SplitCamelCase(type.Name));
                }
            }

            return null;
        };

        private static string[] SplitCamelCase(string source)
        {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }
    }
}