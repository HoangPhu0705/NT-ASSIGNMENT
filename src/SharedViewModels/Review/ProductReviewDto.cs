namespace SharedViewModels.Review
{
    public class ProductReviewsDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public Dictionary<string, int> RatingCounts { get; set; } = new();
        public List<ReviewDto> Reviews { get; set; } = new();
    }
}