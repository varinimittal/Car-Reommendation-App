using CarRecommendationApi.Models;
using System.Text.Json;

namespace CarRecommendationApi.Services
{
    public class CarService : ICarService
    {
        private readonly string _jsonFilePath;
        private readonly ILogger<CarService> _logger;

        // Scoring weights
        private const double PriceWeight = 0.40;
        private const double MileageWeight = 0.30;
        private const double SafetyWeight = 0.30;

        public CarService(IConfiguration configuration, ILogger<CarService> logger)
        {
            _jsonFilePath = configuration["CarDataFilePath"] ?? "Data/cars.json";
            _logger = logger;
        }

        public async Task<List<CarRecommendation>> GetRecommendedCarsAsync(RecommendationRequest request)
        {
            var cars = await LoadCarsFromJsonAsync();
            var filteredCars = FilterCars(cars, request);

            if (!filteredCars.Any())
            {
                return new List<CarRecommendation>();
            }

            var recommendations = ScoreCars(filteredCars, request);
            return recommendations;
        }

        private async Task<List<Car>> LoadCarsFromJsonAsync()
        {
            try
            {
                if (!File.Exists(_jsonFilePath))
                {
                    _logger.LogWarning("Car data file not found at {FilePath}", _jsonFilePath);
                    return new List<Car>();
                }

                var jsonContent = await File.ReadAllTextAsync(_jsonFilePath);
                var cars = JsonSerializer.Deserialize<List<Car>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return cars ?? new List<Car>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car data from JSON file");
                throw;
            }
        }

        private List<Car> FilterCars(List<Car> cars, RecommendationRequest request)
        {
            var filtered = cars.Where(c => c.Price <= request.Budget);

            if (!string.IsNullOrWhiteSpace(request.FuelType))
            {
                filtered = filtered.Where(c => c.FuelType.Equals(request.FuelType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.BodyType))
            {
                filtered = filtered.Where(c => c.BodyType.Equals(request.BodyType, StringComparison.OrdinalIgnoreCase));
            }

            return filtered.ToList();
        }

        private List<CarRecommendation> ScoreCars(List<Car> cars, RecommendationRequest request)
        {
            // Get min/max values for normalization
            var minPrice = cars.Min(c => c.Price);
            var maxPrice = cars.Max(c => c.Price);
            var minMileage = cars.Min(c => c.Mileage);
            var maxMileage = cars.Max(c => c.Mileage);

            var scoredRecs = new List<(double Score, CarRecommendation Rec)>();

            foreach (var car in cars)
            {
                var scores = CalculateNormalizedScores(car, request, minPrice, maxPrice, minMileage, maxMileage);
                var totalScore = CalculateWeightedScore(scores);
                var explanation = GenerateExplanation(car, request, scores);

                scoredRecs.Add((
                    Math.Round(totalScore, 3),
                    new CarRecommendation
                    {
                        Name = $"{car.Make} {car.Model} {car.Variant}".Trim(),
                        Price = car.Price,
                        Mileage = car.Mileage,
                        SafetyRating = car.SafetyRating,
                        RecommendationReason = explanation
                    }
                ));
            }

            return scoredRecs
                .OrderByDescending(x => x.Score)
                .Take(5)
                .Select(x => x.Rec)
                .ToList();
        }

        private (double priceScore, double mileageScore, double safetyScore) CalculateNormalizedScores(
            Car car,
            RecommendationRequest request,
            decimal minPrice,
            decimal maxPrice,
            double minMileage,
            double maxMileage)
        {
            // Price Score: Cars closer to budget get higher scores
            // Normalize price to 0-1, where closer to maxBudget is better
            double priceScore = 0;
            if (maxPrice > minPrice)
            {
                var priceRange = maxPrice - minPrice;
                var budgetRange = request.Budget - minPrice;
                var carPriceFromMin = car.Price - minPrice;

                // Score based on how close to budget (higher price = better value utilization)
                priceScore = (double)(carPriceFromMin / budgetRange);
                priceScore = Math.Clamp(priceScore, 0, 1);
            }
            else
            {
                priceScore = 1.0;
            }

            // Mileage Score: Higher mileage is better (normalize to 0-1)
            double mileageScore = 0;
            if (maxMileage > minMileage)
            {
                mileageScore = (car.Mileage - minMileage) / (maxMileage - minMileage);
            }
            else if (car.Mileage > 0)
            {
                mileageScore = 1.0;
            }

            // Safety Score: Normalize 1-5 rating to 0-1
            double safetyScore = (car.SafetyRating - 1) / 4.0;

            return (priceScore, mileageScore, safetyScore);
        }

        private double CalculateWeightedScore((double price, double mileage, double safety) scores)
        {
            return (scores.price * PriceWeight) +
                   (scores.mileage * MileageWeight) +
                   (scores.safety * SafetyWeight);
        }

        private string GenerateExplanation(
            Car car,
            RecommendationRequest request,
            (double priceScore, double mileageScore, double safetyScore) scores)
        {
            var explanationParts = new List<string>();

            // Analyze mileage
            if (scores.mileageScore >= 0.7)
            {
                explanationParts.Add("excellent mileage");
            }
            else if (scores.mileageScore >= 0.4)
            {
                explanationParts.Add("good mileage");
            }

            // Analyze safety
            if (scores.safetyScore >= 0.75) // 4-5 star rating
            {
                explanationParts.Add("top safety rating");
            }
            else if (scores.safetyScore >= 0.5) // 3-4 star rating
            {
                explanationParts.Add("good safety");
            }

            // Analyze price
            var priceUtilization = (double)(car.Price / request.Budget);
            if (priceUtilization >= 0.9)
            {
                explanationParts.Add("maximizes your budget");
            }
            else if (priceUtilization >= 0.7)
            {
                explanationParts.Add("great value");
            }
            else if (priceUtilization <= 0.5)
            {
                explanationParts.Add("well under budget");
            }
            else
            {
                explanationParts.Add("within budget");
            }

            // Build final explanation
            if (explanationParts.Count == 0)
            {
                return "Meets your requirements";
            }

            var explanation = string.Join(" and ", explanationParts);
            return char.ToUpper(explanation[0]) + explanation.Substring(1);
        }
    }
}