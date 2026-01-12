using Microsoft.AspNetCore.Mvc;
using ReValidator.Validation.Mvc;
using static ReValidator.Tests.ReValidatorMvcTests;

[ApiController]
[Route("persons")]
public sealed class PersonsController : ControllerBase
{
    [HttpPost("novalidation")]
    public IActionResult NoValidation(Person p)
    {
        return Ok();
    }

    [HttpPost("validate")]
    public IActionResult Validate([Validate] Person p)
    {
        return Ok();
    }
}
