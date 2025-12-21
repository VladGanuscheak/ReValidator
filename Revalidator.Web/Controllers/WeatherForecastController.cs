using Microsoft.AspNetCore.Mvc;
using ReValidator;

namespace Revalidator.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger = logger;

    [HttpPost(Name = nameof(First_Test))]
    public IActionResult First_Test(
        [FromServices] IValidator<Person> validator, 
        [FromBody] Person person)
    {
        var validationResult = validator.Validate(person);
        
        if (validationResult.IsValid)
        {
            _logger.LogInformation($"The following validation did not pass: " +
                $"{validationResult.Errors.Select(x => $"{x.PropertyName}: {string.Join(", ", x.ErrorMessages)};")}");
        }

        var isValid = validationResult.IsValid;
        var errors = validationResult.Errors;

        return Ok(validationResult);
    }

    [HttpPut(Name = nameof(Second_Test))]
    public IActionResult Second_Test(
        [FromServices] IValidator<Person> validator,
        [FromBody] Person person)
    {
        return Ok(validator.Validate(person));
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
