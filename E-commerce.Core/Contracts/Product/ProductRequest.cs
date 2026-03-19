using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Core.Contracts.Product;

public record ProductRequest(
    string Name,
    string Description,
    decimal NewPrice,
    decimal OldPrice,
    int CategoryId,
    IFormFileCollection Photo 
);
public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(200).WithMessage("Description cannot exceed 200 characters");

        RuleFor(x => x.NewPrice)
            .GreaterThan(0).WithMessage("New price must be greater than 0");

        RuleFor(x => x.OldPrice)
            .GreaterThan(0).WithMessage("Old price must be greater than 0");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid category is required");

        RuleFor(x => x.Photo)
            .NotEmpty().WithMessage("At least one photo is required");
    }
}