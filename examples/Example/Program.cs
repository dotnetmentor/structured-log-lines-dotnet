using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StructuredLogLines;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure =>
            {
                // Default config
                // configure.AddStructuredLogLines();

                // Configuration overrides
                configure.AddStructuredLogLines(sll =>
                {
                    sll.MinimumLogLevel = LogLevel.Trace;
                    sll.UseColor = true;
                    sll.UsePretty = true;

                    //sll.With<CompactLoggingConvetions>();
                    //sll.With<EnrichedLoggingConvetions>();
                    //sll.With<DotnetLoggingConventions>();
                });
            });


            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();

            // Log levels
            logger.LogTrace("Hello {level}", LogLevel.Trace);
            logger.LogDebug("Hello {level}", LogLevel.Debug);
            logger.LogInformation("Hello {level}", LogLevel.Information);
            logger.LogWarning("Hello {level}", LogLevel.Warning);
            logger.LogError("Hello {level}", LogLevel.Error);
            logger.LogCritical("Hello {level}", LogLevel.Critical);
            logger.Log(LogLevel.None, "Hello {level}", LogLevel.None);

            // Args and Events
            logger.LogTrace("Hello");
            logger.LogTrace("Hello {noargs}");
            logger.LogTrace("Hello {exactargs}", 1);
            logger.LogTrace("Hello {additionalargs}", 1, new Hello { Name = "Kristoffer" });
            logger.LogTrace("Hello {additionalargs}", 1, new Hello { Name = "Kristoffer" });
            logger.LogTrace("Hello {additionalargs}", 1, new Hello { Name = "Kristoffer" });
            logger.LogTrace("Hello {additionalargs}", 1, new Hello { Name = "Kristoffer" });
            logger.LogTrace("Hello {additionalargs}", 1, new Hello { Name = "Kristoffer" });
            logger.LogTrace("Hello {multipleevents}", 1, new Hello { Name = "Kristoffer" }, new Goodbye { Name = "Kristoffer" });

            // Exceptions and Events
            try
            {
                System.IO.File.ReadAllText("./doesnotexist.txt");
            }
            catch (Exception ex)
            {
                logger.LogTrace(ex, "Hello {error}", ex.Message);
            }

            // Scope and Contenxt
            using (logger.BeginScope("CorrelationId {id}", Guid.NewGuid(), "string", 2, new Dictionary<string, object> { { "foo", "bar" } }))
            using (logger.BeginScope(new Hello { Name = "Scope" }))
            {
                logger.LogDebug("context from scope");
            }
        }
    }

    class Hello
    {
        public string Name { get; set; }
    }

    class Goodbye
    {
        public string Name { get; set; }
    }

    class CompactLoggingConvetions : DefaultLoggerConventions
    {
        public override string MessagePropertyName { get; } = "@m";
        public override string TimestampPropertyName { get; } = "@t";
        public override string LevelPropertyName { get; } = "@l";
        public override string SevertiyPropertyName { get; } = null;
        public override string EventPropertyName { get; } = "@e";
        public override string ContextPropertyName { get; } = "@c";
    }

    class EnrichedLoggingConvetions : DefaultLoggerConventions
    {
        public override Action<StructuredLogLinesLogger> OnLoggerCreated { get; } = logger =>
        {
            logger.BeginScope(new Origin { Service = "example-app", Logger = logger.Name });
        };

        class Origin
        {
            public string Service { get; set; }

            public string Logger { get; set; }
        }
    }

    class DotnetLoggingConventions : DefaultLoggerConventions
    {
        public override JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public override Func<object, string> ContextPathResolver { get; } = ctx =>
        {
            var type = ctx.GetType();
            return type.FullName.StartsWith("System.") ? null : type.Name;
        };

        public override Func<object, string> EventPathResolver { get; } = ctx =>
        {
            var type = ctx.GetType();
            return (ctx is Exception == false && type.FullName.StartsWith("System.")) ? null : type.Name;
        };

        public override Func<object, object> EventDataResolver { get; } = evt => evt;
    }
}
