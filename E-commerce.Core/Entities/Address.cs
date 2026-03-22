using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Core.Entities;

public class Address : BaseEntity<int>
{
    public string RecipientName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
}