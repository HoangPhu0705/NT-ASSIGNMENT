using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class VariantAttributeValue
{
    [Key]
    public Guid Id { get; set; }
    public Guid ProductVariantAttributeId { get; set; }
    public Guid ProductVariantId { get; set; }
    
    public string Name { get; set; }
    public string Value { get; set; }
    
    public ProductVariantAttribute ProductVariantAttribute { get; set; }
    public ProductVariant ProductVariant { get; set; }
}