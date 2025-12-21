using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using ReValidator.Contracts;

namespace ReValidator
{
    public class ValidationExpression<T> : ValidationDefinition<T>
    {
        public ValidationExpression(
            string expression,
            string propertyName = "Model",
            string? ruleName = null, 
            string? errorMessage = null,
            ReValidatorOptions options = default!)
        {
            var parsingConfig = new ParsingConfig
            {
                CustomTypeProvider = new DefaultDynamicLinqCustomTypeProvider(
                    ParsingConfig.Default,
                    options.RegisteredTypes.ToList(),
                    cacheCustomTypes: true)
            };

            Rule = DynamicExpressionParser
                .ParseLambda<T, bool>(parsingConfig, false, expression)
                .Compile();

            RuleName = ruleName ?? "DynamicExpression";
            PropertyName = propertyName;
            ErrorMessage = errorMessage ?? $"Validation failed: {expression}";
        }
    }
}
