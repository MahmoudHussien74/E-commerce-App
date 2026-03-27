using E_commerce.Core.Entities;

namespace E_commerce.Core.Entities.Order;

public class DeliveryMethod : BaseEntity<int>
{
    public DeliveryMethod() { }

    public DeliveryMethod(string shortName, string deliveryTime, string description, decimal price)
    {
        ShortName = shortName;
        DeliveryTime = deliveryTime;
        Description = description;
        Price = price;
    }

    public string ShortName { get; set; } = string.Empty;
    public string DeliveryTime { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
