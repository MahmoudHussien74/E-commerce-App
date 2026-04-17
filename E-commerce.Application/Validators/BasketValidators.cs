using E_commerce.Application.Contracts.Basket;
using FluentValidation;
namespace E_commerce.Application.Validators;
public class BasketItemRequestValidator : AbstractValidator<BasketItemRequest>
{
    public BasketItemRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}
public class BasketUpdateRequestValidator : AbstractValidator<BasketUpdateRequest>
{
    public BasketUpdateRequestValidator()
    {
        RuleForEach(x => x.Items).SetValidator(new BasketItemRequestValidator());
    }
}
