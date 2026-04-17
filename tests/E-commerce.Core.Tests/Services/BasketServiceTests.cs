using E_commerce.Core.Tests.Common;

namespace E_commerce.Core.Tests.Services;

public class BasketServiceTests
{
    private readonly BasketServiceTestContext _context = new();

    [Fact]
    public async Task GetBasketAsync_ShouldReturnMappedBasket_WhenBasketExists()
    {
        var basket = TestDataFactory.CreateBasket();
        _context.BasketRepositoryMock.Setup(x => x.GetBasketAsync("basket-123"))
            .ReturnsAsync(Result.Success(basket));

        var result = await _context.Sut.GetBasketAsync("basket-123");

        Assert.True(result.IsSuccess);

        Assert.Equal("basket-123", result.Value.Id);

        Assert.Single(result.Value.BasketItems);
    }

    [Fact]
    public async Task GetBasketAsync_ShouldReturnFailure_WhenRepositoryReturnsFailure()
    {
        var expectedError = BasketErrors.NotFound;
        _context.BasketRepositoryMock.Setup(x => x.GetBasketAsync("missing-basket"))
            .ReturnsAsync(Result.Failure<CustomerBasket>(expectedError));

        var result = await _context.Sut.GetBasketAsync("missing-basket");

        Assert.True(result.IsFailure);
        Assert.Equal(expectedError, result.Error);
    }

    [Fact]
    public async Task UpdateBasketAsync_ShouldReturnUpdatedBasket_WhenRequestIsValid()
    {
        var request = TestDataFactory.CreateBasketResponse();
        var updatedBasket = TestDataFactory.CreateBasket();
        _context.BasketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CustomerBasket>()))
            .ReturnsAsync(Result.Success(updatedBasket));

        var result = await _context.Sut.UpdateBasketAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(request.Id, result.Value.Id);
        Assert.Equal(request.BasketItems.Count, result.Value.BasketItems.Count);
    }

    [Fact]
    public async Task UpdateBasketAsync_ShouldMapEmptyItemsCollection_WhenBasketHasNoItems()
    {
        var request = TestDataFactory.CreateBasketResponse();
        request.BasketItems.Clear();

        _context.BasketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.Is<CustomerBasket>(basket =>
                basket.Id == request.Id &&
                basket.basketItems.Count == 0)))
            .ReturnsAsync(Result.Success(new CustomerBasket(request.Id)));

        var result = await _context.Sut.UpdateBasketAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.BasketItems);
    }

    [Fact]
    public async Task UpdateBasketAsync_ShouldReturnFailure_WhenRepositoryReturnsFailure()
    {
        var request = TestDataFactory.CreateBasketResponse();
        _context.BasketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CustomerBasket>()))
            .ReturnsAsync(Result.Failure<CustomerBasket>(BasketErrors.UpdateFailed));

        var result = await _context.Sut.UpdateBasketAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(BasketErrors.UpdateFailed, result.Error);
    }

    [Fact]
    public async Task UpdateBasketAsync_ShouldReturnFailure_WhenRepositoryThrowsException()
    {
        var request = TestDataFactory.CreateBasketResponse();
        _context.BasketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CustomerBasket>()))
            .ThrowsAsync(new InvalidOperationException("Redis unavailable"));

        var result = await _context.Sut.UpdateBasketAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal(BasketErrors.UpdateFailed, result.Error);
    }

    [Fact]
    public async Task DeleteBasketAsync_ShouldReturnSuccess_WhenRepositoryDeletesBasket()
    {
        _context.BasketRepositoryMock.Setup(x => x.DeleteBasketAsync("basket-123"))
            .ReturnsAsync(Result.Success());

        var result = await _context.Sut.DeleteBasketAsync("basket-123");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteBasketAsync_ShouldReturnRepositoryFailure_WhenDeleteFails()
    {
        _context.BasketRepositoryMock.Setup(x => x.DeleteBasketAsync("basket-123"))
            .ReturnsAsync(Result.Failure(BasketErrors.DeletionFailed));

        var result = await _context.Sut.DeleteBasketAsync("basket-123");

        Assert.True(result.IsFailure);
        Assert.Equal(BasketErrors.DeletionFailed, result.Error);
    }

    [Fact]
    public async Task DeleteBasketAsync_ShouldReturnDeletionFailed_WhenRepositoryThrowsException()
    {
        _context.BasketRepositoryMock.Setup(x => x.DeleteBasketAsync("basket-123"))
            .ThrowsAsync(new Exception("Storage timeout"));

        var result = await _context.Sut.DeleteBasketAsync("basket-123");

        Assert.True(result.IsFailure);

        Assert.Equal(BasketErrors.DeletionFailed, result.Error);
    }
}
