using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Core.Entities.Product;

public class Photo : BaseEntity<int>
{
    public string ImageName { get; set; }
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; }
}
