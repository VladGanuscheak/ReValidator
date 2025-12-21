using System;
using System.Linq;

namespace ReValidator
{
    public class Validator<T> : IValidator<T>
    {
        private static readonly object _lock = new object();

        public static ValidationDefinition<T>[] Rules { get; private set; } 
            = Array.Empty<ValidationDefinition<T>>();

        public void SetRule(ValidationDefinition<T> definition)
        {
            lock (_lock)
            {
                Rules = Rules.Where(x => !(x.RuleName == definition.RuleName && x.PropertyName == definition.PropertyName))
                    .Append(definition)
                    .ToArray();
            }
        }

        public void SetRule(ValidationExpression<T> expression)
        {
            SetRule((ValidationDefinition<T>)expression);
        }

        public ValidationResult Validate(T model)
        {
            return new ValidationResult
            {
                Errors = Rules
                    .Where(r => !r.Rule(model))
                    .GroupBy(r => r.PropertyName)
                    .Select(r => new ValidationErrorMessage
                    {
                        PropertyName = r.Key,
                        ErrorMessages = r.Select(x => x.ErrorMessage.Replace("{PropertyName}", x.PropertyName)).ToArray()
                    })
                    .ToArray()
            };
        }
    }
}
