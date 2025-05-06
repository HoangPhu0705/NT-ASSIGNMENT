namespace SharedViewModels.Review;

public class UpdateReviewRequest
{
    public Guid ReviewId { get; set; }
    public int Rating { get; set; }
    public string ReviewText { get; set; }
}