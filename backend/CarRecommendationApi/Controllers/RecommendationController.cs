using CarRecommendationApi.Models;
using CarRecommendationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRecommendationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly ILogger<RecommendationController> _logger;

        public RecommendationController(ICarService carService, ILogger<RecommendationController> logger)
        {
            _carService = carService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<List<CarRecommendation>>> GetRecommendations([FromBody] RecommendationRequest request)
        {
            try
            {
                if (request.Budget <= 0)
                {
                    return BadRequest("Budget must be greater than zero.");
                }

                var recommendations = await _carService.GetRecommendedCarsAsync(request);

                if (recommendations.Count == 0)
                {
                    return NotFound("No cars found matching the specified criteria.");
                }

                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting car recommendations");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}