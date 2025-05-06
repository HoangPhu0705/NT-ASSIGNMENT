namespace SharedViewModels.Cart;

public class CartDto
{
    public Guid Id { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    public int TotalItems => Items.Sum(i => i.Quantity);
}