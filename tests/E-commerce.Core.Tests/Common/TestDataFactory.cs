namespace E_commerce.Core.Tests.Common;

internal static class TestDataFactory
{
    public static OrderRequest CreateOrderRequest() => new()
    {
        BasketId = "basket-123",
        DeliveryMethodId = 7,
        ShippingAddress = new AddressDto
        {
            FirstName = "John",
            LastName = "Doe",
            City = "Cairo",
            State = "Cairo",
            Street = "12 Nile Street",
            ZipCode = "11511"
        }
    };

    public static CustomerBasket CreateBasket(string? paymentIntentId = "pi_123") => new("basket-123")
    {
        PaymentIntentId = paymentIntentId ?? string.Empty,
        ClientSecret = "secret_123",
        DeliveryMethodId = 7,
        ShippingPrice = 25,
        basketItems =
        [
            new BasketItem
            {
                Id = 1,
                Name = "Keyboard",
                Price = 150,
                Qunatity = 2,
                Category = "Accessories"
            }
        ]
    };

    public static List<Product> CreateProducts() =>
    [
        new Product
        {
            Id = 1,
            Name = "Keyboard",
            Description = "Mechanical keyboard",
            NewPrice = 150,
            OldPrice = 175,
            StockQuantity = 5,
            CategoryId = 10
        }
    ];

    public static DeliveryMethod CreateDeliveryMethod(int id = 7) => new("Express", "2 Days", "Fast delivery", 25)
    {
        Id = id
    };

    public static Orders CreateOrder(string buyerEmail, string? paymentIntentId = "pi_123") => new()
    {
        Id = 55,
        BuyerEmail = buyerEmail,
        PaymentIntentId = paymentIntentId ?? string.Empty,
        SubTotal = 300,
        DeliveryMethod = CreateDeliveryMethod(),
        ShippingAddress = new ShippingAddress
        {
            FirstName = "John",
            LastName = "Doe",
            City = "Cairo",
            State = "Cairo",
            Street = "12 Nile Street",
            ZipCode = "11511"
        },
        OrderItems =
        [
            new OrderItem
            {
                ProductItemId = 1,
                ProductName = "Keyboard",
                Price = 150,
                Quntity = 2,
                MainImage = "keyboard.jpg"
            }
        ],
        Status = Status.Pending
    };

    public static CustomerBasketResponse CreateBasketResponse() => new()
    {
        Id = "basket-123",
        PaymentIntentId = "pi_123",
        ClientSecret = "secret_123",
        DeliveryMethodId = 7,
        ShippingPrice = 25,
        BasketItems =
        [
            new BasketItemResponse
            {
                Id = 1,
                Name = "Keyboard",
                Price = 150,
                Qunatity = 2,
                Category = "Accessories"
            }
        ]
    };
}
