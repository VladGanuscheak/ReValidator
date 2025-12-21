using Microsoft.Extensions.DependencyInjection;
using ReValidator.Contracts;
using ReValidator.SetUp;
using System;
using System.Linq;

namespace ReValidator
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddReValidator(this IServiceCollection services,
            Action<ReValidatorOptions>? configure = null)
        {
            var options = new ReValidatorOptions();
            configure?.Invoke(options);

            services.AddSingleton(options);

            services.AddTransient(typeof(IValidator<>), typeof(Validator<>));

            return services;
        }

        public static IServiceProvider ApplyReconfiguration(
            this IServiceProvider serviceProvider,
            DynamicReconfiguration config)
        {
            if (string.IsNullOrWhiteSpace(config.FullPathToModel))
                throw new ArgumentNullException(nameof(config.FullPathToModel));

            if (string.IsNullOrWhiteSpace(config.Expression))
                throw new ArgumentNullException(nameof(config.Expression));

            var type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(config.FullPathToModel))
                .FirstOrDefault(t => t != null)
                ?? throw new InvalidOperationException("Type not found");

            var options = serviceProvider.GetRequiredService<ReValidatorOptions>();

            var validationExpressionType = typeof(ValidationExpression<>).MakeGenericType(type);

            var validationExpression = Activator.CreateInstance(
                validationExpressionType,
                config.Expression!,
                config.PropertyName ?? "Model",
                config.RuleName ?? "DynamicExpression",
                config.ErrorMessage ?? $"Validation failed: {config.Expression}",
                options
            );

            var validatorType = typeof(IValidator<>).MakeGenericType(type);
            var validator = serviceProvider.GetRequiredService(validatorType);

            var setRuleMethod = validatorType.GetMethod(
                "SetRule",
                new[] { validationExpressionType });

            setRuleMethod!.Invoke(validator, new[] { validationExpression });

            return serviceProvider;
        }
    }
}
