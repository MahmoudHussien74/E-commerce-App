namespace E_commerce.Core.Contracts.Product;

public record ProductImageResponse
{
    public string Url { get; init; } = string.Empty;
    public int ProductId { get; init; }
}