# StructuredLogLines

Structured Log Lines is a flexible, .NET core compatible JSON logger based on the ideas of [JSON Lines](http://jsonlines.org/) and the use of a simple JSON schema.
It implements the ILogger interface and can be configured for use in existing projects with minimal changes to your code.

**NOTE** This project is not ready for production use. Mainly due to performance implications and possible thread safety issues. Please feel free to contribute in any way possible!

**NOTE** This project is made possible using a hack for getting access to all arguments passed to the ILogger when using extensions like LogInformation, LogDebug etc.
More details on why can be found here: https://github.com/aspnet/Logging/issues/533 

## Getting started

```bash
dotnet add package StructuredLogLines
```

## Schema

StructuredLogLines has a simple, but exstensible schema. The schema can be customized by overriding the default conventions and all of the property names can be changed to fit your needs. See conventions for more info.

```
{
  "timestamp": "2016-12-01T02:23:12.236543Z",    // Consistent ISO8601 dates with nanosecond precision
  "level": "info",                               // The level of the log event
  "severity": 6,                                 // The severity of the log event
  "message": "POST /checkout for 192.321.22.21", // Human readable message
  "context": { ... },                            // Contextual metadata about the environment the event takes place in.
  "event": { ... }                               // Structured representation of the log event
}
```

### Context

Context is an open ended object containing metadata associated with the log event.
It will usually contain things like the name of the logger, the name of the application etc.

To push context data onto the structured log message, use `BeginScope`.

```csharp
logger.BeginScope(new Origin(name: "HelloWorld", logger: logger.Name)))
```

By default, type names are mapped to context by convention.

```json
{
  "timestamp": "2018-09-06T09:54:03.3639210+02:00",
  "level": "info",
  "severity": 6,
  "message": "Hello StructuredLogLines!",
  "context": {
    "origin": {
      "name": "HelloWorld",
      "logger": "HelloWorld.Program"
    }
  }
}
```

### Event

Event is an open ended object containing the actual log event itself.

```csharp
using (logger.BeginScope(new Origin(name: "HelloWorld", logger: logger.Name))))
{
    logger.LogInformation("Hello {world}!", "StructuredLogLines", new ProgramInitialized { Args: args });
}
```

By default, type names are mapped to context by convention.

```json
{
  "timestamp": "2018-09-06T09:54:03.0619800+02:00",
  "level": "info",
  "severity": 6,
  "message": "Hello StructuredLogLines!",
  "context": {
    "origin": {
      "name": "HelloWorld",
      "logger": "HelloWorld.Program"
    }
  },
  "event": {
    "program": {
      "initialized": {
        "args": []
      }
    }
  }
}
```

## Conventions

`StructuredLogLines` heavily relies on configurable conventions for maximum flexibility.
Everything (*almost*) can be changed by implementing the `ILoggerConventions` interface,
or by inheriting from the `DefaultLoggerConventions` and overriding selected pieces.

By allowing most aspects of how the logger works to be changed, you are able to create your own set of conventions geared towards a custom schema, and package them up for easy re-use across projects.

The best way to learn how and what can be changed is to have a look at the [example project(s)](./examples).