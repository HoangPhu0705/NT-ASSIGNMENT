using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ProductVariantAttribute
{
    [Key]
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid CategoryAttributeId { get; set; }
    
    public Product Product { get; set; }
    public CategoryAttribute CategoryAttribute { get; set; }
    public ICollection<VariantAttributeValue> Values { get; set; } = new List<VariantAttributeValue>();
}