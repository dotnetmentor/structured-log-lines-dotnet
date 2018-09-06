# StructuredLogLines

Structured Log Lines is .NET core compatible JSON logger based on the ideas of [JSON Lines](http://jsonlines.org/) and the use of a simple JSON schema.
It implements the ILogger of Microsoft.Extensions.Logging and can be configured for use in existing projects with minimal changes to your code.

**NOTE** This project is made possible using a hack for getting access to all arguments passed to the ILogger when using extensions like LogInformation, LogDebug etc.
More details on why can be found here: https://github.com/aspnet/Logging/issues/533 

## Getting started

```bash
dotnet add package StructuredLogLines
```

## Schema

StructuredLogLines has a simple but exstensible schema.

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

To push context data onto the structured log message, use `BeginScope` of `ILogger`.

```csharp
logger.BeginScope(new Origin(name: "HelloWorld", logger: logger.Name)))
```

By default, class types are mapped to context by convention.

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

By default, class types are mapped to event by convention.

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