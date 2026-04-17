using System.Text.Json;
namespace E_commerce.Infrastructure.Repositories;
internal sealed class CustomerBasketRepository : ICustomerBasketRepository
{
    private readonly IDatabase _database;

    public CustomerBasketRepository(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }
    public async Task<Result> DeleteBasketAsync(string buyerId, CancellationToken cancellationToken = default)
    {
        var result = await _database.KeyDeleteAsync(buyerId);

        return result ? Result.Success()
            : Result.Failure(BasketErrors.DeletionFailed);
    }

    public async Task<Result<CustomerBasket>> GetBasketAsync(string buyerId, CancellationToken cancellationToken = default)
    {
        var result = await _database.StringGetAsync(buyerId);
        
        if (result.IsNullOrEmpty)
            return Result.Failure<CustomerBasket>(BasketErrors.NotFound);

        var basket = JsonSerializer.Deserialize<CustomerBasket>(result!)!;
        return Result.Success(basket);
    }

    public async Task<Result<CustomerBasket>> UpdateBasketAsync(CustomerBasket basket, CancellationToken cancellationToken = default)
    {
        var created = await _database.StringSetAsync(
            basket.Id,
            JsonSerializer.Serialize(basket),
            TimeSpan.FromDays(3)
        );

        if (!created)
            return Result.Failure<CustomerBasket>(BasketErrors.UpdateFailed);

        return await GetBasketAsync(basket.Id);
    }
}