#if NET7_0_OR_GREATER

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ReValidator.Validation.MinimalApi;

public sealed class ReValidatorFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext ctx,
        EndpointFilterDelegate next)
    {
        var model = ctx.Arguments.OfType<T>().FirstOrDefault();
        if (model is null)
        {
            return await next(ctx);
        }

        var validator = ctx.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
        {
            return await next(ctx);
        }

        var result = validator.Validate(model);

        if (!result.IsValid)
        {
            return Results.ValidationProblem(result.Errors.ToDictionary(x => x.PropertyName, x => x.ErrorMessages), statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return await next(ctx);
    }
}

public sealed class ReValidatorFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext ctx,
        EndpointFilterDelegate next)
    {
        var endpoint = ctx.HttpContext.GetEndpoint();
        if (endpoint is null)
        {
            return await next(ctx);
        }

        var routeHandler = endpoint.Metadata
            .OfType<RouteHandlerBuilder>()
            .FirstOrDefault();

        MethodInfo? methodInfo = null;

        if (endpoint is RouteEndpoint routeEndpoint)
        {
            methodInfo = routeEndpoint.RequestDelegate!.Method;
        }

        if (methodInfo is null)
        {
            return await next(ctx);
        }

        var parameters = methodInfo.GetParameters();

        for (int i = 0; i < parameters.Length && i < ctx.Arguments.Count; i++)
        {
            var parameter = parameters[i];
            var model = ctx.Arguments[i];

            if (model is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(model.GetType());
            var validator = ctx.HttpContext.RequestServices.GetService(validatorType);

            if (validator is null)
            {
                continue;
            }

            var validateMethod = validatorType.GetMethod("Validate", new[] { model.GetType() });
            if (validateMethod is null)
            {
                continue;
            }

            var result = (ValidationResult)validateMethod.Invoke(validator, new[] { model })!;
            if (!result.IsValid)
            {
                var errors = result.Errors.ToDictionary(x => x.PropertyName, x => x.ErrorMessages);

                return Results.ValidationProblem(errors, statusCode: StatusCodes.Status422UnprocessableEntity);
            }
        }

        return await next(ctx);
    }
}
#endif
