using FluentValidation;
using Validations.Benchmarking.Models;

namespace Validations.Benchmarking.Validators
{


    public class UserModelNestedValidator : AbstractValidator<UserModelNested>
    {
        public UserModelNestedValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();

            RuleForEach(x => x.Addresses)
                .SetValidator(new AddressModelValidator());
        }
    }
}
