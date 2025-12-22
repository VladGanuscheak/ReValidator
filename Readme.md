# ReValidator

**ReValidator** provides **runtime-reconfigurable validation** for .NET applications.

Unlike traditional validation frameworks that rely on static rule definitions or reflection-heavy execution, ReValidator uses **compiled expression trees**, achieving performance close to **statically defined validation rules** while preserving **full runtime flexibility**.

This makes ReValidator well-suited for scenarios where validation rules must be modified without redeploying the application (e.g. configuration-driven systems, rule engines, multi-tenant platforms).

---

## Features

- Runtime configuration of validation rules
- Near-static execution performance using compiled expressions
- No reflection during validation execution
- Standard `IValidator<T>` integration
- Dependency Injection friendly
- Support for reusable helper methods in expressions

---

## Installation

Install via NuGet:

```bash
dotnet add package ReValidator
```

Or via the NuGet Package Manager:

```powershell
Install-Package ReValidator
```

---

## Getting Started
### Dependency Injection

Enable ReValidator by registering it in the service container:

```c#
services.AddReValidator();
```

This registers the `IValidator<T>` interface and all required internal services.

--- 

### Registering helper methods

If your validation expressions depend on reusable helper methods, you can register them explicitly:

```c#
using ReValidator;

services.AddReValidator(options =>
{
    options.RegisterType<Helpers>();
});
```

Registered types become available for use inside validation expressions.

---

### Defining validation rules

Validation rules can be added or updated at runtime:

```c#
using ReValidator;

services.ApplyReconfiguration(
    new DynamicReconfiguration
    {
        RuleName = "FullNameRequired",
        PropertyName = "Name",
        ErrorMessage = "The \"{PropertyName}\" is required",
        Expression = "x => !string.IsNullOrWhiteSpace(x.Name)",
        FullPathToModel = typeof(Person).FullName
    });
```

---

### Rule properties

| Property          | Description                                   |
|-------------------|-----------------------------------------------|
| RuleName          | Unique rule identifier                        |
| PropertyName      | Target model property                         |
| ErrorMessage      | Validation error message                      |
| Expression        | Validation logic as an expression string      |
| FullPathToModel   | Fully qualified model type name               |


---

### Consuming the validator

Inject and use the validator via the standard IValidator<T> interface:

```c#
using ReValidator;

public class MyClass
{
    private readonly IValidator<Person> _validator;

    public MyClass(IValidator<Person> validator)
    {
        _validator = validator;
    }
}
```

---

### Validation usage

Validate an input model as follows:

```c#
var validationResult = validator.Validate(model);

if (!validationResult.IsValid)
{
    _logger.LogInformation(
        "Validation failed: " +
        string.Join("; ",
            validationResult.Errors.Select(x =>
                $"{x.PropertyName}: {string.Join(", ", x.ErrorMessages)}"))
    );
}
```

---

### Performance considerations

- Validation expressions are compiled once and cached
- No reflection is used during validation execution
- Execution speed is comparable to statically coded validation rules
- Runtime reconfiguration does not require application restart

---

### Limitations

- Validation expressions must be valid C# expressions
- Only members available on the target model and registered helper types can be accessed
- Expression parsing errors surface during configuration, not execution

--- 

### Packages

- `ReValidator.Contracts` – Shared contracts and abstractions
- `ReValidator` – Core validation engine and DI integration

---

### Compatibility

- netstandard 2.1
- ASP.NET Core
- Console applications
- Background services

--- 

### License

This project is licensed under the MIT License.

--- 

 **Note:** `ReValidator` is a .NET-specific validation library and is not related to JavaScript or JSON Schema validators with similar names.
 