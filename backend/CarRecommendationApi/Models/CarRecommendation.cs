namespace CarRecommendationApi.Models
{
    public class CarRecommendation
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Mileage { get; set; }
        public int SafetyRating { get; set; }
        public string RecommendationReason { get; set; } = string.Empty;
    }
}