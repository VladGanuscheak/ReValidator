using FluentValidation;
using Validations.Benchmarking.Models;

namespace Validations.Benchmarking.Validators
{
    public class AddressModelValidator : AbstractValidator<AddressModel>
    {
        public AddressModelValidator()
        {
            RuleFor(x => x.Street).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
        }
    }
}
