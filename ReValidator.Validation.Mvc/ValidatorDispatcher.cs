using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace ReValidator.Validation.Mvc
{
    public static class ValidatorDispatcher
    {
        private static readonly ConcurrentDictionary<Type, Func<object, object, ValidationResult>> _cache
            = new();

        public static ValidationResult Dispatch(object validator, object value)
        {
            var modelType = value.GetType();

            var invoker = _cache.GetOrAdd(modelType, CreateInvoker);

            return invoker(validator, value);
        }

        private static Func<object, object, ValidationResult> CreateInvoker(Type modelType)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(modelType);

            var validatorParam = Expression.Parameter(typeof(object), "validator");
            var valueParam = Expression.Parameter(typeof(object), "value");

            var castValidator = Expression.Convert(validatorParam, validatorType);
            var castValue = Expression.Convert(valueParam, modelType);

            var call = Expression.Call(
                castValidator,
                validatorType.GetMethod("Validate")!,
                castValue);

            var lambda = Expression.Lambda<Func<object, object, ValidationResult>>(
                call,
                validatorParam,
                valueParam);

            return lambda.Compile();
        }
    }
}
