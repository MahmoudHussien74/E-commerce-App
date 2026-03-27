using E_commerce.Core.Contracts.Order;
using E_commerce.Core.Entities.Order;

namespace E_commerce.Core.Services;

public class OrderService(IUnitOfWork unitOfWork, IMapper mapper, IOrderLogic orderLogic) : IOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IOrderLogic _orderLogic = orderLogic;

    public async Task<Result<OrderResponse>> CreateOrdersAsync(OrderRequest orderRequest, string BuyerEmail)
    {
        var basketResult = await _unitOfWork.CustomerBasketRepository.GetBasketAsync(orderRequest.BasketId);
        if (basketResult.IsFailure)
            return Result.Failure<OrderResponse>(basketResult.Error);

        var productIds = basketResult.Value.basketItems.Select(x => x.Id).ToList();
        var productsTask = _unitOfWork.ProductRepository.GetByIdsAsync(productIds);


        var deliveryMethodTask = _unitOfWork.Repository<DeliveryMethod>()
                                  .GetByIdAsync(orderRequest.DeliveryMethodId);

        await Task.WhenAll(productsTask, deliveryMethodTask);

        var products = await productsTask;
        var deliveryMethod = await deliveryMethodTask;

        if (deliveryMethod is null)
            return Result.Failure<OrderResponse>(new Error("Order.DeliveryMethodNotFound", "Selected delivery method was not found.", 400));

        var address = _mapper.Map<ShippingAddress>(orderRequest.ShippingAddress);
        var orderBuilderResult = _orderLogic.BuildOrder(BuyerEmail, address, deliveryMethod, basketResult.Value, products);

        if (orderBuilderResult.IsFailure)
            return Result.Failure<OrderResponse>(orderBuilderResult.Error);

        var order = orderBuilderResult.Value;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            _unitOfWork.Repository<Orders>().Add(order);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            return Result.Failure<OrderResponse>(new Error("Order.CreationFailed", "Something went wrong while saving the order.", 500));
        }

        await _unitOfWork.CustomerBasketRepository.DeleteBasketAsync(orderRequest.BasketId);

        return Result.Success(_mapper.Map<OrderResponse>(order));
    }

    public async Task<Result<IReadOnlyList<OrderResponse>>> GetAllOrdersForUserAsync(string BuyerEmail)
    {
        var orders = await _unitOfWork.Repository<Orders>().GetListAsync(
            o => o.BuyerEmail == BuyerEmail,
            default,
            o => o.OrderItems,
            o => o.DeliveryMethod
        );

        var sortedOrders = orders.OrderByDescending(o => o.OrderDate).ToList();
        var response = _mapper.Map<IReadOnlyList<OrderResponse>>(sortedOrders);
        return Result.Success(response);
    }

    public async Task<Result<IReadOnlyList<DeliveryMethodResponse>>> GetDeliveryMethodAsync()
    {
        var methods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
        var response = _mapper.Map<IReadOnlyList<DeliveryMethodResponse>>(methods);
        return Result.Success(response);
    }

    public async Task<Result<OrderResponse>> GetOrderByIdAsync(int Id, string BuyerEmail)
    {
        var order = await _unitOfWork.Repository<Orders>().GetAsync(
            o => o.Id == Id && o.BuyerEmail == BuyerEmail,
            default,
            o => o.OrderItems,
            o => o.DeliveryMethod
        );

        if (order is null)
            return Result.Failure<OrderResponse>(new Error("Order.NotFound", "The requested order was not found.", 404));

        return Result.Success(_mapper.Map<OrderResponse>(order));
    }
}
