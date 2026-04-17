namespace E_commerce.Application.Contracts.Basket;

public class CustomerBasketResponse
{
    public string Id { get; set; } = string.Empty;
    public string BuyerId { get; set; } = string.Empty;
    public string? PaymentIntentId { get; set; }
    public string? ClientSecret { get; set; }
    public int? DeliveryMethodId { get; set; }
    public decimal ShippingPrice { get; set; }
    public List<BasketItemResponse> Items { get; set; } = new();
}
