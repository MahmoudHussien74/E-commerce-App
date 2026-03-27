using FluentValidation;

namespace E_commerce.Core.Contracts.Order;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator()
    {
        RuleFor(x => x.BasketId)
        .NotEmpty();
        RuleFor(x => x.DeliveryMethodId)
        .NotEmpty()
        .GreaterThan(0);
        RuleFor(x => x.ShippingAddress)
        .NotNull()
        .SetValidator(new AddressDtoValidator());
    }
}

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.FirstName)
        .NotEmpty();
        RuleFor(x => x.LastName)
        .NotEmpty();
        RuleFor(x => x.City)
        .NotEmpty();
        RuleFor(x => x.ZipCode)
        .NotEmpty();
        RuleFor(x => x.Street)
        .NotEmpty();
        RuleFor(x => x.State)
        .NotEmpty();
    }
}
