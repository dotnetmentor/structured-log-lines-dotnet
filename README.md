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
  "message": "POST /checkout for 192.321.22.21", // Human readable message
  "context": { ... },                            // Contextual metadata about the environment the event takes place in.
  "event": { ... }                               // Structured representation of the log event
}
```
