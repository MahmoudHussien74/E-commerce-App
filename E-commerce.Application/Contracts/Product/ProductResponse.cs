using E_commerce.Application.Contracts.Category;

namespace E_commerce.Application.Contracts.Product;

public record ProductResponse(
    int Id,
    string Name,
    string Description,
    decimal NewPrice,
    decimal OldPrice,
    CategoryResponse Category,
    IReadOnlyList<ProductImageResponse> Photos);
