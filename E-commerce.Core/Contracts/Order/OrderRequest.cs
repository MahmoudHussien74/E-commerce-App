namespace E_commerce.Core.Contracts.Order;

public class OrderRequest
{
    public string BasketId { get; set; } = string.Empty;
    public int DeliveryMethodId { get; set; }
    public AddressDto ShippingAddress { get; set; } = null!;
}
