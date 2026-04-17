namespace E_commerce.Application.Contracts.Basket;

public class BasketUpdateRequest
{
    public int? DeliveryMethodId { get; set; }
    public decimal ShippingPrice { get; set; }
    public List<BasketItemRequest> Items { get; set; } = new();
}
