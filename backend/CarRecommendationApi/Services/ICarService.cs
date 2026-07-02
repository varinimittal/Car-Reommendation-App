using CarRecommendationApi.Models;

namespace CarRecommendationApi.Services
{
    public interface ICarService
    {
        Task<List<CarRecommendation>> GetRecommendedCarsAsync(RecommendationRequest request);
    }
}