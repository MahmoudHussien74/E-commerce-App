using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Contracts.Product;

public record ProductRequest(
    string Name,
    string Description,
    decimal NewPrice,
    decimal OldPrice,
    int CategoryId,
    IFormFileCollection Photo);
