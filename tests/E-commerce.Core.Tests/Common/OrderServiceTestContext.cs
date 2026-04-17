namespace E_commerce.Core.Tests.Common;

internal sealed class OrderServiceTestContext
{
    public const string BuyerEmail = "buyer@shop.com";

    public Mock<IUnitOfWork> UnitOfWorkMock { get; } = new();
    public Mock<ICustomerBasketRepository> BasketRepositoryMock { get; } = new();
    public Mock<IProductRepository> ProductRepositoryMock { get; } = new();
    public Mock<IOrderRepository> OrderRepositoryMock { get; } = new();
    public Mock<IGenericRepository<Orders>> OrdersRepositoryMock { get; } = new();
    public Mock<IGenericRepository<DeliveryMethod>> DeliveryMethodRepositoryMock { get; } = new();
    public Mock<IOrderLogic> OrderLogicMock { get; } = new();
    public OrderService Sut { get; }

    public OrderServiceTestContext()
    {
        UnitOfWorkMock.SetupGet(x => x.CustomerBasketRepository).Returns(BasketRepositoryMock.Object);
        UnitOfWorkMock.SetupGet(x => x.ProductRepository).Returns(ProductRepositoryMock.Object);
        UnitOfWorkMock.SetupGet(x => x.OrderRepository).Returns(OrderRepositoryMock.Object);
        UnitOfWorkMock.Setup(x => x.Repository<Orders>()).Returns(OrdersRepositoryMock.Object);
        UnitOfWorkMock.Setup(x => x.Repository<DeliveryMethod>()).Returns(DeliveryMethodRepositoryMock.Object);

        Sut = new OrderService(UnitOfWorkMock.Object, MapperFactory.Create(), OrderLogicMock.Object);
    }

    public OrderCreationArrangement ArrangeSuccessfulCreation(
        OrderRequest? request = null,
        CustomerBasket? basket = null,
        List<Product>? products = null,
        DeliveryMethod? deliveryMethod = null,
        Orders? builtOrder = null,
        Orders? existingOrder = null)
    {
        request ??= TestDataFactory.CreateOrderRequest();
        basket ??= TestDataFactory.CreateBasket();
        products ??= TestDataFactory.CreateProducts();
        deliveryMethod ??= TestDataFactory.CreateDeliveryMethod(request.DeliveryMethodId);
        builtOrder ??= TestDataFactory.CreateOrder(BuyerEmail, basket.PaymentIntentId);

        BasketRepositoryMock.Setup(x => x.GetBasketAsync(request.BasketId))
            .ReturnsAsync(Result.Success(basket));
        ProductRepositoryMock.Setup(x => x.GetByIdsAsync(It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(basket.basketItems.Select(item => item.Id)))))
            .ReturnsAsync(products);
        DeliveryMethodRepositoryMock.Setup(x => x.GetByIdAsync(request.DeliveryMethodId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryMethod);
        OrderLogicMock.Setup(x => x.BuildOrder(BuyerEmail, It.IsAny<ShippingAddress>(), deliveryMethod, basket, products))
            .Returns(Result.Success(builtOrder));
        OrdersRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Orders, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Orders, object>>[]>()))
            .ReturnsAsync(existingOrder);
        BasketRepositoryMock.Setup(x => x.DeleteBasketAsync(request.BasketId))
            .ReturnsAsync(Result.Success());

        return new OrderCreationArrangement(request, basket, products, deliveryMethod, builtOrder);
    }
}

internal sealed record OrderCreationArrangement(
    OrderRequest Request,
    CustomerBasket Basket,
    List<Product> Products,
    DeliveryMethod DeliveryMethod,
    Orders BuiltOrder);
