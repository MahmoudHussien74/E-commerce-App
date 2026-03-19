namespace E_commerce.Core.Entities.Product;

public class Category : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ICollection<Product>  Products { get; set; } = new HashSet<Product>();
}
