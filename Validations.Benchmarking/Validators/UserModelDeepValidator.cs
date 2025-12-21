using FluentValidation;
using Validations.Benchmarking.Models;

namespace Validations.Benchmarking.Validators
{
    public class ProductModelValidator : AbstractValidator<ProductModel>
    {
        public ProductModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }

    public class OrderItemModelValidator : AbstractValidator<OrderItemModel>
    {
        public OrderItemModelValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.Product).SetValidator(new ProductModelValidator());
        }
    }

    public class OrderModelValidator : AbstractValidator<OrderModel>
    {
        public OrderModelValidator()
        {
            RuleFor(x => x.OrderNumber).NotEmpty();
            RuleForEach(x => x.Items).SetValidator(new OrderItemModelValidator());
        }
    }

    public class UserModelDeepValidator : AbstractValidator<UserModelDeep>
    {
        public UserModelDeepValidator()
        {
            RuleFor(x => x.Email)
                .Must(Helpers.Helpers.IsValidEmail)
                .WithMessage("Not valid email");
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleForEach(x => x.Orders).SetValidator(new OrderModelValidator());
        }
    }
}
