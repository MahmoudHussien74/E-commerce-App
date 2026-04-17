using E_commerce.Application.Contracts.Product;
using FluentValidation;

namespace E_commerce.Application.Validators;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.NewPrice).GreaterThan(0);
        RuleFor(x => x.OldPrice).GreaterThanOrEqualTo(x => x.NewPrice);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Photo).NotNull().Must(x => x.Count > 0).WithMessage("At least one photo is required.");
    }
}
