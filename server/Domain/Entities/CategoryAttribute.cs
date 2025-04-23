using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class CategoryAttribute
{
    [Key]
    public Guid Id { get; set; }
    public Guid? CategoryId { get; set; }
    public string Name { get; set; }
    public bool IsFilterable { get; set; } = true;
    
    public Category Category { get; set; }
    public ICollection<ProductVariantAttribute> ProductVariantAttributes { get; set; } = new List<ProductVariantAttribute>();
}