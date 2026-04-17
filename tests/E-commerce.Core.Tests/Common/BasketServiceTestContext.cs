namespace E_commerce.Core.Tests.Common;

internal sealed class BasketServiceTestContext
{
    public Mock<IUnitOfWork> UnitOfWorkMock { get; } = new();
    public Mock<ICustomerBasketRepository> BasketRepositoryMock { get; } = new();
    public BasketService Sut { get; }

    public BasketServiceTestContext()
    {
        UnitOfWorkMock.SetupGet(x => x.CustomerBasketRepository).Returns(BasketRepositoryMock.Object);
        Sut = new BasketService(UnitOfWorkMock.Object, MapperFactory.Create());
    }
}
