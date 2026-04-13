namespace E_commerce.Core.Entities;

public class CustomerBasket
{
    public string Id { get; set; }
    public string PaymentIntentId { get; set; }
    public string ClientSecret { get; set; }
    public List<BasketItem> basketItems { get; set; } = new();
    public CustomerBasket()
    {
        
    }
    public CustomerBasket(string id)
    {
        Id = id; 
    }
  
}
