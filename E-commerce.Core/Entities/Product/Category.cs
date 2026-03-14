namespace E_commerce.Core.Entities.Product;

public class Category : BaseEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<Product>  Products { get; set; } = new HashSet<Product>();
}
