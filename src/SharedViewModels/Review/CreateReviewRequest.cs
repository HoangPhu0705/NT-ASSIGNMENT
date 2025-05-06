namespace SharedViewModels.Review;


public class CreateReviewRequest
{
    public Guid ProductId { get; set; }
    public int Rating { get; set; }
    public string ReviewText { get; set; }
}