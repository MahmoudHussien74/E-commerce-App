using E_commerce.Core.Entities.Order;
namespace E_commerce.Core.Services;
public class OrderLogic : IOrderLogic
{
    public Result<Orders> BuildOrder(string buyerEmail, ShippingAddress address, DeliveryMethod deliveryMethod, CustomerBasket basket, List<Product> products)
    {
        var productDict = products.ToDictionary(p => p.Id);
        var orderItems = new List<OrderItem>();

        foreach (var item in basket.basketItems)
        {
            if (productDict.TryGetValue(item.Id, out var product))
            {
                orderItems.Add(new OrderItem(
                    product.Id,
                    product.Photos.FirstOrDefault()?.ImageName ?? string.Empty,
                    product.Name,
                    product.NewPrice,
                    item.Qunatity
                ));
            }
        }
        if (!orderItems.Any())
            return Result.Failure<Orders>(error: new Error("Order.NoValidItems", "The basket does not contain any valid products.", 400));

        var subTotal = orderItems.Sum(item => item.Price * item.Quntity);
        var order = new Orders
        {
            BuyerEmail = buyerEmail,
            ShippingAddress = address,
            DeliveryMethod = deliveryMethod,
            OrderItems = orderItems,
            SubTotal = subTotal,
            Status = Status.Pending,
            PaymentIntentId = basket.PaymentIntentId
        };
        return Result.Success(order);
    }
}
