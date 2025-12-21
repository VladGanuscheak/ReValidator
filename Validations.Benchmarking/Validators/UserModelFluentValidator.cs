using FluentValidation;
using Validations.Benchmarking.Models;

namespace Validations.Benchmarking.Validators
{
    public class UserModelFluentValidator : AbstractValidator<UserModel>
    {
        public UserModelFluentValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotEmpty();
        }
    }
}
