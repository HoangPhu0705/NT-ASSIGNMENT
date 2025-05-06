namespace SharedViewModels.Product
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public List<VariantAttributeDto> Attributes { get; set; } = new List<VariantAttributeDto>();
    }
    
}


