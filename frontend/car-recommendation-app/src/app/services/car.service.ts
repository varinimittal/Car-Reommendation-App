import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { CarRecommendation, RecommendationRequest, ApiError } from '../models/car.model';

@Injectable({ providedIn: 'root' })
export class CarService {
  private readonly apiUrl = 'https://localhost:7240/api/Recommendation';

  constructor(private readonly http: HttpClient) {}

  getRecommendations(request: RecommendationRequest): Observable<CarRecommendation[]> {
    return this.http
      .post<CarRecommendation[]>(this.apiUrl, request)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let apiError: ApiError;

    if (error.status === 0) {
      // Network / client-side error
      apiError = {
        status: 0,
        message: 'Unable to reach the server. Please check your connection.',
      };
    } else {
      // Server returned a non-2xx status
      apiError = {
        status: error.status,
        message:
          error.error?.message ??
          `Server error ${error.status}: ${error.statusText}`,
      };
    }

    return throwError(() => apiError);
  }
}
