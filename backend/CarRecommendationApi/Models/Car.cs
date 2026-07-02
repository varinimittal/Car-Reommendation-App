namespace CarRecommendationApi.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Variant { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Mileage { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public string BodyType { get; set; } = string.Empty;
        public int SafetyRating { get; set; }
    }
}