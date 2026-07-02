namespace CarRecommendationApi.Models
{
    public class RecommendationRequest
    {
        public decimal Budget { get; set; }
        public string? FuelType { get; set; }
        public string? BodyType { get; set; }
    }
}