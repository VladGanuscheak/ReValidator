namespace ReValidator.Validation.Mvc
{
    public static class ValidationResultExtensions
    {
        public static ReValidationProblem ToProblem(this ValidationResult result)
        {
            return new ReValidationProblem
            {
                Errors = result.Errors.ToDictionary(x => x.PropertyName, x => x.ErrorMessages)
            };
        }
    }
}
