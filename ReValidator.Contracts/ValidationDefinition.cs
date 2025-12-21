using System;
namespace ReValidator
{
    public class ValidationDefinition<T>
    {
        public string RuleName { get; set; } = string.Empty;

        public string PropertyName { get; set; } = "Model";

        public Func<T, bool> Rule { get; set; } = default!;

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
