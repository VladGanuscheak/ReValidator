# ReValidator.Validation.MinimalApi

**Dynamic, rule-based request validation** for ASP.NET Minimal APIs using **Endpoint Filters**.

This package integrates `ReValidator` into Minimal APIs, allowing request DTOs to be validated using **runtime-loaded rules** instead of compiled validators or attributes.

## Installation

```bash
dotnet add package ReValidator.Validation.MinimalApi
```

```c#
builder.Services.AddReValidator();
```

## Basic usage

1. Define a model

```c#
public class Person
{
    public string? Name { get; set; }
    public int Age { get; set; }
}
```

2. Register a validation rule

Rules are injected at runtime and can be changed without redeploying the application.

```c#
app.Services.ApplyReconfiguration(
    new DynamicReconfiguration
    {
        RuleName = "NameRequired",
        PropertyName = "Name",
        ErrorMessage = "The \"{PropertyName}\" is required",
        Expression = "x => !string.IsNullOrWhiteSpace(x.Name)",
        FullPathToModel = typeof(Person).FullName
    });
```

3. Enable validation on an endpoint

Validate a specific request type:

```c#
app.MapPost("/persons", (Person person) => Results.Ok())
   .AddReValidator<Person>();
```

Or validate all request DTOs on the endpoint:

```c#
app.MapPost("/persons", (Person person) => Results.Ok())
   .AddEndpointFilter<ReValidatorFilter>();
```

## Validation result

If any rule fails, ReValidator returns:

`422 Unprocessable Entity`

With a standard validation payload:

```json
{
  "errors": {
    "Name": [
      "The \"Name\" is required"
    ]
  }
}
```

When validation runs

Validation is executed only when:
- `.AddReValidator<T>()` is applied
- `.AddEndpointFilter<ReValidatorFilter>()` is used

If no rules are registered for a model type, validation is skipped.

## What this package provides

- Minimal API integration for `ReValidator`
- Endpoint filters for request validation
- Runtime rule execution using expressions
- Standardized HTTP 422 responses

`ReValidator` is designed for systems where validation rules must be managed outside of code and updated at runtime without redeployment.
