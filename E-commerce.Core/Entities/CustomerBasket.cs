namespace E_commerce.Core.Entities;

public class CustomerBasket
{
    public string Id { get; set; } = string.Empty;
    public string BuyerId { get; set; } = string.Empty;
    public string PaymentIntentId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public int? DeliveryMethodId { get; set; }
    public decimal ShippingPrice { get; set; }
    public List<BasketItem> BasketItems { get; set; } = new();

    public CustomerBasket()
    {
    }

    public CustomerBasket(string buyerId)
    {
        BuyerId = buyerId;
        Id = buyerId;
    }
}
