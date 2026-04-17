namespace E_commerce.Application.Contracts.Order;

public class DeliveryMethodRequest
{
    public string ShortName { get; set; } = string.Empty;
    public string DeliveryTime { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
