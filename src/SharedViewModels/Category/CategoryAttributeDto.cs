namespace SharedViewModels.Category;

public class CategoryAttributeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsFilterable { get; set; }
    public List<string> Values { get; set; } = new();
}