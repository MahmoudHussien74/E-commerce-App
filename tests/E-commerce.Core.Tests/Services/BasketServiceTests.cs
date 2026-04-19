using E_commerce.Application.Abstractions.Persistence;
using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Basket;
using E_commerce.Application.Errors;
using E_commerce.Application.Services;
using E_commerce.Core.Entities;
using Moq;
using Xunit;

namespace E_commerce.Core.Tests.Services;

public class BasketServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICustomerBasketRepository> _basketRepoMock;
    private readonly BasketService _sut;

    public BasketServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _basketRepoMock = new Mock<ICustomerBasketRepository>();
        
        _unitOfWorkMock.Setup(u => u.CustomerBasketRepository).Returns(_basketRepoMock.Object);
        _sut = new BasketService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetBasketAsync_WhenBasketExists_ShouldReturnSuccessWithItems()
    {
        // Arrange
        var buyerId = "test-buyer-id";
        var basket = new CustomerBasket(buyerId)
        {
            BasketItems = [ new BasketItem { Id = 1, Name = "Laptop", Qunatity = 1, Price = 1500, Category = "Electronics" } ]
        };

        _basketRepoMock.Setup(x => x.GetBasketAsync(buyerId, default))
            .ReturnsAsync(Result.Success(basket));

        // Act
        var result = await _sut.GetBasketAsync(buyerId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Items);
        Assert.Equal("Laptop", result.Value.Items[0].Name);
    }

    [Fact]
    public async Task GetBasketAsync_WhenBasketNotFound_ShouldReturnFailure()
    {
        // Arrange
        var buyerId = "non-existent-buyer";
        _basketRepoMock.Setup(x => x.GetBasketAsync(buyerId, default))
            .ReturnsAsync(Result.Failure<CustomerBasket>(BasketErrors.NotFound));

        // Act
        var result = await _sut.GetBasketAsync(buyerId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BasketErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task UpdateBasketAsync_WhenValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var buyerId = "test-buyer-id";
        var request = new BasketUpdateRequest
        {
            DeliveryMethodId = 1,
            ShippingPrice = 10,
            Items = new List<BasketItemRequest>
            {
                new BasketItemRequest { Id = 2, Name = "Mouse", Quantity = 2, Price = 25, Category = "Accessories" }
            }
        };

        var updatedBasket = new CustomerBasket(buyerId) { BasketItems = [ new BasketItem { Id = 2, Name = "Mouse" } ] };
        _basketRepoMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CustomerBasket>(), default))
            .ReturnsAsync(Result.Success(updatedBasket));

        // Act
        var result = await _sut.UpdateBasketAsync(buyerId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(buyerId, result.Value.BuyerId);
    }
}
