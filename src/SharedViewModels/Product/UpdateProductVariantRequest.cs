namespace SharedViewModels.Product
{
    public class UpdateProductVariantRequest
    {
        public Guid? Id { get; set; }  // For existing variants
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public List<UpdateVariantAttributeRequest> Attributes { get; set; }
        public bool IsDeleted { get; set; }  // To mark variants for deletion
    }

}