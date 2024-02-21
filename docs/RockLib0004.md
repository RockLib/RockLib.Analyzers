---
sidebar_label: 'Add InfoLog attribute'
---

# :warning: Deprecation Warning :warning:

This library has been deprecated and will no longer receive updates.

---

## RockLib0004: Add InfoLog attribute

## Cause

This rule fires when an api controller or its methods are not using the `InfoLogAttribute` to automatically create logs.

## Reason for rule

Adding the `InfoLogAttribute` to a controller or its methods can simply logging by automatically logging for each endpoint.

## How to fix violations

To fix the viloation, use the provided "Add InfoLog attribute" for either the controller class or one of its endpoint methods.

## Examples

### Violates

```csharp
using Microsoft.AspNetCore.Mvc;

namespace TestingNamespace
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Get";
        }
    }
}
```

### Does not violate
```csharp
using Microsoft.AspNetCore.Mvc;

namespace TestingNamespace
{
    [ApiController]
    [Route("[controller]")]
    [InfoLog]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Get";
        }
    }
}
```

## How to suppress violations

```csharp
#pragma warning disable RockLib0004 // Add InfoLog attribute
#pragma warning restore RockLib0004 // Add InfoLog attribute
```
