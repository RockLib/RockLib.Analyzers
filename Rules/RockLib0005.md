---
sidebar_label: 'No log level specified'
---

# RockLib0005: No log level specified

## Cause

This rule fires when a `LogEntry` is passed to the `ILogger.Log` method without setting its `Level` property.

## Reason for rule

Logs should not have a level of `NotSet`.

## How to fix violations

To fix a violation of this rule, apply the appropriate code fix:

- Set LogEntry.Level to Debug
- Set LogEntry.Level to Info
- Set LogEntry.Level to Warn
- Set LogEntry.Level to Error
- Set LogEntry.Level to Fatal
- Set LogEntry.Level to Audit

## Examples

### Violates

```csharp
public void Example(ILogger logger)
{
    var logEntry = new LogEntry("Example log with no level");
    logger.Log(logEntry);
}
```

## How to suppress violations

```csharp
#pragma warning disable RockLib0005 // No log level specified
#pragma warning restore RockLib0005 // No log level specified
```
