using FluentValidation;

namespace E_commerce.Core.Contracts.Category;

public class UpdateCategoryRequestValidator:AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x=>x.Id)
            .GreaterThan(0)
            .NotEmpty();

        RuleFor(x => x.Name)
          .NotEmpty()
          .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty();

    }
}
