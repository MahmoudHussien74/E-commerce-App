namespace E_commerce.Core.Entities.Order;

public class Orders : BaseEntity<int>
{
    public string BuyerEmail { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public ShippingAddress ShippingAddress { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public IReadOnlyList<OrderItem> OrderItems { get; set; } = [];
    public Status Status { get; set; }
}
