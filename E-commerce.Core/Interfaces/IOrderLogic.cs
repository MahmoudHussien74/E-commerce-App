using E_commerce.Core.Common;
using E_commerce.Core.Entities;
using E_commerce.Core.Entities.Order;
using E_commerce.Core.Entities.Product;
using System.Collections.Generic;

namespace E_commerce.Core.Interfaces;

public interface IOrderLogic
{
    Result<Orders> BuildOrder(string buyerEmail, ShippingAddress address, DeliveryMethod deliveryMethod, CustomerBasket basket, List<Product> products);
}
