export interface RecommendationRequest {
  budget: number;
  fuelType: string;
  bodyType: string;
}

export interface CarRecommendation {
  name: string;
  price: number;
  mileage: number;
  safetyRating: number;
  recommendationReason: string;
}

export interface ApiError {
  status: number;
  message: string;
}
