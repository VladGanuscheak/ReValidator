namespace ReValidator
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T model);

        void SetRule(ValidationDefinition<T> definition);

        void SetRule(ValidationExpression<T> expression);
    }
}
