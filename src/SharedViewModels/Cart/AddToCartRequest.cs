namespace SharedViewModels.Cart;

public class AddToCartRequest
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; } = 1;
}