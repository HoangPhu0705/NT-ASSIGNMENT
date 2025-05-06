namespace SharedViewModels.Product
{
    public class CreateProductVariantRequest
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public List<CreateVariantAttributeRequest> Attributes { get; set; }
    }
}

