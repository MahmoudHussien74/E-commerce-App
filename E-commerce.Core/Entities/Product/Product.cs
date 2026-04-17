namespace E_commerce.Core.Entities.Product;

public class Product:BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public int CategoryId { get; set; }
    public int StockQuantity { get; set; }
    public virtual Category  Category { get; set; } = null!;

    public List<Photo> Photos { get; set; } = new ();

}
