namespace SharedViewModels.Product
{
    public class UpdateVariantAttributeRequest
    {
        public Guid? Id { get; set; }  // For existing attributes
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsDeleted { get; set; }  // To mark attributes for deletion
    }
    
}

