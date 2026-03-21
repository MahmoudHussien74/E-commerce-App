namespace E_commerce.Core.Contracts.Basket;

public class CustomerBasketResponse
{
    public string Id { get; set; }
    public List<BasketItemResponse> BasketItems { get; set; } = new();
}
