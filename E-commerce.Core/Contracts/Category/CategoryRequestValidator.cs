using FluentValidation;

namespace E_commerce.Core.Contracts.Category;

public class CategoryRequestValidator:AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty();
        
    }
}
