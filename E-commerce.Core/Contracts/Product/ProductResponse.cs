using E_commerce.Core.Contracts.Category;
namespace E_commerce.Core.Contracts.Product;

public record ProductResponse(
    int Id,
    string Name,
    string Description,
    decimal NewPrice,
    decimal OldPrice,
    CategoryResponse Category,
    List<ProductImageResponse> Photos
);
