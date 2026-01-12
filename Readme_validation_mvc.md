# ReValidator.Validation.Mvc

**ReValidator.Validation.Mvc** is a lightweight, generic **validation adapter** for ASP.NET Core MVC, built on top of the ReValidator engine. It allows you to apply **strongly-typed, reusable validators** to controller parameters using a simple `[Validate]` attribute, with full support for **dependency injection** and **dynamic rules**.

This package is fully compatible with **.NET 6, 7, 8, 9**, and works seamlessly alongside **MVC**.

---

## Features

- Apply `[Validate]` to **any controller parameter**.
- Leverages your existing `IValidator<T>` implementations.
- Returns **structured validation errors** (RFC-7807 style).
- Supports **dynamic rule injection** at runtime.
- Optionally configure **global MVC validation filter**.
- Compatible with **MVC** for consistent behavior.
- Returns **HTTP 422 Unprocessable Entity** for failed validations.

---

## Installation

Install via **NuGet**:

```bash
dotnet add package ReValidator.Validation.Mvc
```

## Usage

1. **Add ReValidator services in `Program.cs`**

```c#
var builder = WebApplication.CreateBuilder(args);

// Add core ReValidator services
builder.Services.AddReValidator();

// Add MVC support
builder.Services.AddControllers();
builder.Services.AddReValidatorMvc();

// Optional: add global filter
// or add the mentioned filter per controller's action
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ReValidatorActionFilter>();
});

var app = builder.Build();
app.MapControllers();
app.Run();

```

2. **Create a validator**

```c#
public sealed class PersonValidator : IValidator<Person>
{
    public ValidationResult Validate(Person model)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(model.Name))
            result.Errors.Add(new ValidationError
            {
                Rule = "FullNameRequired",
                Message = "The 'Name' property is required."
            });

        return result;
    }
}
```

3. **Apply `[Validate]` to controller parameters**

```c#
[ApiController]
[Route("persons")]
public sealed class PersonsController : ControllerBase
{
    [HttpPost("create")]
    public IActionResult Create([Validate] Person person)
    {
        // This action will only execute
    }
}
```

4. **Dynamic rule injection**

```c#
app.Services.ApplyReconfiguration(new DynamicReconfiguration
{
    RuleName = "FullNameRequired",
    PropertyName = "Name",
    ErrorMessage = "The \"{PropertyName}\" is required",
    Expression = "x => !string.IsNullOrWhiteSpace(x.Name)",
    FullPathToModel = typeof(Person).FullName
});
```

This allows you to inject or update rules at runtime, without recompiling validators.

5. **Validation response**

When a validation fails, the API returns HTTP 422 with a structured payload:

```
{
  "type": "about:blank",
  "title": "Validation failed",
  "status": 422,
  "detail": "One or more validation rules were violated.",
  "errors": [
    {
      "rule": "FullNameRequired",
      "message": "The 'Name' property is required."
    }
  ]
}

```

## Unit Testing

You can use Microsoft.AspNetCore.TestHost to test MVC endpoints:

```c#
using var app = await CreateApplication();
var client = app.GetTestClient();

var response = await client.PostAsJsonAsync("/persons/create", new Person());
Assert.Equal(422, (int)response.StatusCode);

```

## Requirements

- NET 6.0 or higher
- ReValidator.Contracts
- ASP.NET Core MVC (Abstractions + Core)