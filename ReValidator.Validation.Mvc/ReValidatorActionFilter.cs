using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ReValidator.Validation.Mvc;
using ReValidator;
using Microsoft.AspNetCore.Http;

public sealed class ReValidatorActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor cad)
        {
            await next();
            return;
        }

        foreach (var parameter in cad.MethodInfo.GetParameters())
        {
            if (!parameter.IsDefined(typeof(ValidateAttribute), false))
            {
                continue;
            }

            if (!context.ActionArguments.TryGetValue(parameter.Name!, out var value))
            {
                continue;
            }

            var validatorType = typeof(IValidator<>)
                .MakeGenericType(parameter.ParameterType);

            var validator = context.HttpContext.RequestServices.GetService(validatorType) 
                ?? throw new InvalidOperationException($"No validator registered for {parameter.ParameterType}");

            var result = ValidatorDispatcher.Dispatch(validator, value);

            if (!result.IsValid)
            {
                context.Result = new ObjectResult(result.ToProblem())
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };

                return;
            }
        }

        await next();
    }
}
