using E_commerce.Core.Tests.Common;

namespace E_commerce.Core.Tests.Services;

public class OrderServiceTests
{
    private readonly OrderServiceTestContext _context = new();

    [Fact]
    public async Task CreateOrdersAsync_ShouldCreateOrderAndDeleteBasket_WhenRequestIsValid()
    {
        var arrangement = _context.ArrangeSuccessfulCreation();

        var result = await _context.Sut.CreateOrdersAsync(arrangement.Request, OrderServiceTestContext.BuyerEmail);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderServiceTestContext.BuyerEmail, result.Value.BuyerEmail);
        Assert.Equal(325, result.Value.Total);
        _context.UnitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _context.OrdersRepositoryMock.Verify(x => x.Add(arrangement.BuiltOrder), Times.Once);
        _context.UnitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _context.BasketRepositoryMock.Verify(x => x.DeleteBasketAsync(arrangement.Request.BasketId), Times.Once);
    }

    [Fact]
    public async Task CreateOrdersAsync_ShouldReturnFailure_WhenBasketDoesNotExist()
    {
        var request = TestDataFactory.CreateOrderRequest();
        var expectedError = BasketErrors.NotFound;
        _context.BasketRepositoryMock.Setup(x => x.GetBasketAsync(request.BasketId))
            .ReturnsAsync(Result.Failure<CustomerBasket>(expectedError));

        var result = await _context.Sut.CreateOrdersAsync(request, OrderServiceTestContext.BuyerEmail);

        Assert.True(result.IsFailure);
        Assert.Equal(expectedError, result.Error);
        _context.ProductRepositoryMock.Verify(x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrdersAsync_ShouldReturnFailure_WhenDeliveryMethodIsMissing()
    {
        var request = TestDataFactory.CreateOrderRequest();
        var basket = TestDataFactory.CreateBasket();
        var products = TestDataFactory.CreateProducts();

        _context.BasketRepositoryMock.Setup(x => x.GetBasketAsync(request.BasketId))
            .ReturnsAsync(Result.Success(basket));
        _context.ProductRepositoryMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(products);
        _context.DeliveryMethodRepositoryMock.Setup(x => x.GetByIdAsync(request.DeliveryMethodId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryMethod?)null);

        var result = await _context.Sut.CreateOrdersAsync(request, OrderServiceTestContext.BuyerEmail);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.DeliveryMethodNotFound", result.Error.Code);
        _context.OrderLogicMock.Verify(x => x.BuildOrder(It.IsAny<string>(), It.IsAny<ShippingAddress>(), It.IsAny<DeliveryMethod>(), It.IsAny<CustomerBasket>(), It.IsAny<List<Product>>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrdersAsync_ShouldReturnFailure_WhenOrderLogicRejectsBasket()
    {
        var arrangement = _context.ArrangeSuccessfulCreation();
        var expectedError = new Error("Order.InvalidBasket", "Basket contains invalid items.", 400);
        _context.OrderLogicMock.Setup(x => x.BuildOrder(
                OrderServiceTestContext.BuyerEmail,
                It.IsAny<ShippingAddress>(),
                arrangement.DeliveryMethod,
                arrangement.Basket,
                arrangement.Products))
            .Returns(Result.Failure<Orders>(expectedError));

        var result = await _context.Sut.CreateOrdersAsync(arrangement.Request, OrderServiceTestContext.BuyerEmail);

        Assert.True(result.IsFailure);
        Assert.Equal(expectedError, result.Error);
        _context.UnitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateOrdersAsync_ShouldDeleteExistingOrderBeforeCreatingNewOne_WhenPaymentIntentAlreadyExists()
    {
        var existingOrder = TestDataFactory.CreateOrder(OrderServiceTestContext.BuyerEmail);
        var replacementOrder = TestDataFactory.CreateOrder(OrderServiceTestContext.BuyerEmail);
        var arrangement = _context.ArrangeSuccessfulCreation(existingOrder: existingOrder, builtOrder: replacementOrder);

        var result = await _context.Sut.CreateOrdersAsync(arrangement.Request, OrderServiceTestContext.BuyerEmail);

        Assert.True(result.IsSuccess);
        _context.OrdersRepositoryMock.Verify(x => x.DeleteAsync(existingOrder, It.IsAny<CancellationToken>()), Times.Once);
        _context.OrdersRepositoryMock.Verify(x => x.Add(replacementOrder), Times.Once);
    }

    [Fact]
    public async Task CreateOrdersAsync_ShouldRollbackAndReturnFailure_WhenCommitThrowsException()
    {
        var arrangement = _context.ArrangeSuccessfulCreation();
        _context.UnitOfWorkMock.Setup(x => x.CommitAsync()).ThrowsAsync(new Exception("SQL failure"));

        var result = await _context.Sut.CreateOrdersAsync(arrangement.Request, OrderServiceTestContext.BuyerEmail);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.CreationFailed", result.Error.Code);
        _context.UnitOfWorkMock.Verify(x => x.RollbackAsync(), Times.Once);
        _context.BasketRepositoryMock.Verify(x => x.DeleteBasketAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetAllOrdersForUserAsync_ShouldReturnProjectedOrders_WhenOrdersExist()
    {
        var expectedOrders = new List<OrderResponse>
        {
            new()
            {
                Id = 55,
                BuyerEmail = "buyer@shop.com",
                DeliveryMethod = "Express",
                ShippingPrice = 25,
                SubTotal = 300,
                Total = 325,
                Status = "Pending",
                ShippingAddress = TestDataFactory.CreateOrderRequest().ShippingAddress
            }
        };

        _context.OrderRepositoryMock.Setup(x => x.GetAllOrdersForUserProjectedAsync(OrderServiceTestContext.BuyerEmail))
            .ReturnsAsync(expectedOrders);

        var result = await _context.Sut.GetAllOrdersForUserAsync(OrderServiceTestContext.BuyerEmail);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal(55, result.Value[0].Id);
    }

    [Fact]
    public async Task GetAllOrdersForUserAsync_ShouldReturnEmptyCollection_WhenUserHasNoOrders()
    {
        _context.OrderRepositoryMock.Setup(x => x.GetAllOrdersForUserProjectedAsync(OrderServiceTestContext.BuyerEmail))
            .ReturnsAsync([]);

        var result = await _context.Sut.GetAllOrdersForUserAsync(OrderServiceTestContext.BuyerEmail);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetDeliveryMethodAsync_ShouldReturnAvailableMethods_WhenMethodsExist()
    {
        var expectedMethods = new List<DeliveryMethodResponse>
        {
            new()
            {
                Id = 7,
                ShortName = "Express",
                Description = "Fast delivery",
                DeliveryTime = "2 Days",
                Price = 25
            }
        };

        _context.OrderRepositoryMock.Setup(x => x.GetDeliveryMethodsProjectedAsync())
            .ReturnsAsync(expectedMethods);

        var result = await _context.Sut.GetDeliveryMethodAsync();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Express", result.Value[0].ShortName);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExistsForBuyer()
    {
        var expectedOrder = new OrderResponse
        {
            Id = 55,
            BuyerEmail = OrderServiceTestContext.BuyerEmail,
            DeliveryMethod = "Express",
            ShippingPrice = 25,
            SubTotal = 300,
            Total = 325,
            Status = "Pending",
            ShippingAddress = TestDataFactory.CreateOrderRequest().ShippingAddress
        };

        _context.OrderRepositoryMock.Setup(x => x.GetOrdersForUserProjectedAsync(OrderServiceTestContext.BuyerEmail, 55))
            .ReturnsAsync(expectedOrder);

        var result = await _context.Sut.GetOrderByIdAsync(55, OrderServiceTestContext.BuyerEmail);

        Assert.True(result.IsSuccess);
        Assert.Equal(55, result.Value.Id);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnFailure_WhenOrderDoesNotExist()
    {
        _context.OrderRepositoryMock.Setup(x => x.GetOrdersForUserProjectedAsync(OrderServiceTestContext.BuyerEmail, 404))
            .ReturnsAsync((OrderResponse)null!);

        var result = await _context.Sut.GetOrderByIdAsync(404, OrderServiceTestContext.BuyerEmail);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.NotFound", result.Error.Code);
    }
}
