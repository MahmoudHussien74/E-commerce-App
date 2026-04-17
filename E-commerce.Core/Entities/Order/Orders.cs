namespace E_commerce.Core.Entities.Order;

public class Orders : BaseEntity<int>
{
    public string BuyerId { get; set; } = string.Empty;
    public string BuyerEmail { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string PaymentIntentId { get; set; } = string.Empty;
    public ShippingAddress ShippingAddress { get; set; } = null!;
    public DeliveryMethod DeliveryMethod { get; set; } = null!;
    public IReadOnlyList<OrderItem> OrderItems { get; set; } = [];
    public Status Status { get; set; }
}
