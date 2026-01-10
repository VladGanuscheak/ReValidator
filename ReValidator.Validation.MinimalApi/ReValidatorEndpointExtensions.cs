#if NET7_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ReValidator.Validation.MinimalApi
{
    public static class ReValidatorEndpointExtensions
    {
        public static RouteHandlerBuilder AddReValidator<T>(
            this RouteHandlerBuilder builder)
        {
            builder.AddEndpointFilter<ReValidatorFilter<T>>();
            return builder;
        }
    }
}
#endif
