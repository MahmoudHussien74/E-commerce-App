using System;
using System.Collections.Generic;

namespace E_commerce.Core.Contracts.Order;

public class OrderResponse
{
    public int Id { get; set; }
    public string BuyerEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public AddressDto ShippingAddress { get; set; } = null!;
    public string DeliveryMethod { get; set; } = string.Empty;
    public decimal ShippingPrice { get; set; }
    public IReadOnlyList<OrderItemResponse> OrderItems { get; set; } = [];
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class OrderItemResponse
{
    public int ProductItemId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string MainImage { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quntity { get; set; }
}
