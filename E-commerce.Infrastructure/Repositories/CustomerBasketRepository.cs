using E_commerce.Core.Common;
using E_commerce.Core.Entities;
using E_commerce.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System.Text.Json;

namespace E_commerce.Infrastructure.Repositories;

public class CustomerBasketRepository : ICustomerBasketRepository
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _redis;

    public CustomerBasketRepository(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }
    public async Task<Result> DeleteBasketAsync(string id)
    {
        var result = await _database.KeyDeleteAsync(id);

        return result ? Result.Success()
            : Result.Failure(new Error("","",StatusCodes.Status400BadRequest));
    }

    public async Task<CustomerBasket> GetBasketAsync(string id)
    {
        var result = await _database.StringGetAsync(id);
        if (!string.IsNullOrEmpty(result))
        {

            return  JsonSerializer.Deserialize<CustomerBasket>(result!)!;
        }
        return null;
    }

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var created = await _database.StringSetAsync(
            basket.Id,
            JsonSerializer.Serialize(basket),
            TimeSpan.FromDays(3)
        );

        if (!created)
            return null;

        return await GetBasketAsync(basket.Id);
    }
}
