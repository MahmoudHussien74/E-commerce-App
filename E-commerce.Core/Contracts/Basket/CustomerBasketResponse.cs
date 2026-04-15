namespace E_commerce.Core.Contracts.Basket;

public class CustomerBasketResponse
{
    public string Id { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? ClientSecret { get; set; }
    public int? DeliveryMethodId { get; set; }
    public decimal ShippingPrice { get; set; }
    public List<BasketItemResponse> BasketItems { get; set; } = new();
}
