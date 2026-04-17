using E_commerce.Application.Abstractions.Persistence;
using E_commerce.Application.Abstractions.Services;
using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Order;
using E_commerce.Application.Errors;
using E_commerce.Core.Entities.Order;

namespace E_commerce.Application.Services;

internal sealed class OrderService(IUnitOfWork unitOfWork) : IOrderService
{
    public async Task<Result<OrderResponse>> CreateOrderAsync(string buyerId, string buyerEmail, OrderRequest request, CancellationToken cancellationToken = default)
    {
        var basketResult = await unitOfWork.CustomerBasketRepository.GetBasketAsync(buyerId, cancellationToken);
        if (basketResult.IsFailure)
        {
            return Result.Failure<OrderResponse>(basketResult.Error);
        }

        var basket = basketResult.Value;
        var products = await unitOfWork.ProductRepository.GetByIdsAsync(basket.BasketItems.Select(x => x.Id), cancellationToken);
        var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(request.DeliveryMethodId, cancellationToken);
        if (deliveryMethod is null)
        {
            return Result.Failure<OrderResponse>(OrderErrors.DeliveryMethodNotFound);
        }

        var productMap = products.ToDictionary(x => x.Id);
        var orderItems = basket.BasketItems
            .Where(x => productMap.ContainsKey(x.Id))
            .Select(x =>
            {
                var product = productMap[x.Id];
                return new OrderItem(product.Id, product.Photos.FirstOrDefault()?.ImageName ?? string.Empty, product.Name, product.NewPrice, x.Qunatity);
            })
            .ToList();

        if (orderItems.Count == 0)
        {
            return Result.Failure<OrderResponse>(OrderErrors.EmptyBasket);
        }

        var existingOrder = await unitOfWork.Repository<Orders>().GetAsync(x => x.PaymentIntentId == basket.PaymentIntentId, cancellationToken, x => x.OrderItems);
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (existingOrder is not null)
            {
                await unitOfWork.Repository<Orders>().DeleteAsync(existingOrder, cancellationToken);
            }

            var order = new Orders
            {
                BuyerId = buyerId,
                BuyerEmail = buyerEmail,
                ShippingAddress = new ShippingAddress
                {
                    FirstName = request.ShippingAddress.FirstName,
                    LastName = request.ShippingAddress.LastName,
                    City = request.ShippingAddress.City,
                    State = request.ShippingAddress.State,
                    Street = request.ShippingAddress.Street,
                    ZipCode = request.ShippingAddress.ZipCode
                },
                DeliveryMethod = deliveryMethod,
                OrderItems = orderItems,
                SubTotal = orderItems.Sum(x => x.Price * x.Quntity),
                PaymentIntentId = basket.PaymentIntentId,
                Status = Status.Pending
            };

            unitOfWork.Repository<Orders>().Add(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CustomerBasketRepository.DeleteBasketAsync(buyerId, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(Map(order));
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result<IReadOnlyList<OrderResponse>>> GetAllOrdersForUserAsync(string buyerId, CancellationToken cancellationToken = default)
    {
        var orders = await unitOfWork.OrderRepository.GetAllOrdersForUserAsync(buyerId, cancellationToken);
        return Result.Success<IReadOnlyList<OrderResponse>>(orders.Select(Map).ToList());
    }

    public async Task<Result<OrderResponse>> GetOrderByIdAsync(int id, string buyerId, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.OrderRepository.GetOrderForUserAsync(buyerId, id, cancellationToken);
        return order is null
            ? Result.Failure<OrderResponse>(OrderErrors.NotFound)
            : Result.Success(Map(order));
    }

    public async Task<Result<OrderResponse>> UpdateOrderStatusAsync(int id, Status status, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.OrderRepository.GetOrderByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return Result.Failure<OrderResponse>(OrderErrors.NotFound);
        }

        order.Status = status;
        unitOfWork.OrderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(order));
    }

    private static OrderResponse Map(Orders order)
        => new()
        {
            Id = order.Id,
            BuyerId = order.BuyerId,
            BuyerEmail = order.BuyerEmail,
            OrderDate = order.OrderDate,
            ShippingAddress = new AddressDto
            {
                FirstName = order.ShippingAddress.FirstName,
                LastName = order.ShippingAddress.LastName,
                City = order.ShippingAddress.City,
                State = order.ShippingAddress.State,
                Street = order.ShippingAddress.Street,
                ZipCode = order.ShippingAddress.ZipCode
            },
            DeliveryMethod = order.DeliveryMethod.ShortName,
            ShippingPrice = order.DeliveryMethod.Price,
            SubTotal = order.SubTotal,
            Total = order.SubTotal + order.DeliveryMethod.Price,
            Status = order.Status.ToString(),
            OrderItems = order.OrderItems.Select(x => new OrderItemResponse
            {
                ProductItemId = x.ProductItemId,
                ProductName = x.ProductName,
                MainImage = x.MainImage,
                Price = x.Price,
                Quantity = x.Quntity
            }).ToList()
        };
}
