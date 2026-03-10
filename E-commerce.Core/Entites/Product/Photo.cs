using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Core.Entites.Product;

public class Photo : BaseEntity<int>
{
    public string ImageName { get; set; }
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public virtual Product Products { get; set; }
}
