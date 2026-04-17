namespace E_commerce.Application.Contracts.Order;

public class OrderRequest
{
    public int DeliveryMethodId { get; set; }
    public AddressDto ShippingAddress { get; set; } = null!;
}
