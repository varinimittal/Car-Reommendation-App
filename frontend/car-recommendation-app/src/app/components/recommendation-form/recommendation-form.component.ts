import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CarService } from '../../services/car.service';
import { CarRecommendation } from '../../models/car.model';
import { CarCardComponent } from '../car-card/car-card.component';

@Component({
  selector: 'app-recommendation-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CarCardComponent],
  templateUrl: './recommendation-form.component.html',
  styleUrl: './recommendation-form.component.css'
})
export class RecommendationFormComponent {
  form: FormGroup;
  results: CarRecommendation[] = [];
  loading = false;
  submitted = false;
  errorMessage = '';

  fuelTypes = ['Petrol', 'Diesel', 'Electric', 'Hybrid', 'Plug-in Hybrid'];
  bodyTypes = ['Sedan', 'SUV', 'Hatchback', 'Coupe', 'Convertible', 'Pickup Truck', 'Minivan'];

  constructor(private readonly fb: FormBuilder, private readonly carService: CarService) {
    this.form = this.fb.group({
      budget: [null, [Validators.required, Validators.min(1)]],
      fuelType: ['', Validators.required],
      bodyType: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.results = [];

    this.carService.getRecommendations(this.form.value).subscribe({
      next: (data: CarRecommendation[]) => {
        this.results = data;
        this.loading = false;
        this.submitted = true;
      },
      error: () => {
        this.errorMessage = 'Failed to fetch recommendations. Please try again.';
        this.loading = false;
        this.submitted = true;
      }
    });
  }

  isInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control?.invalid && control.touched);
  }
}
