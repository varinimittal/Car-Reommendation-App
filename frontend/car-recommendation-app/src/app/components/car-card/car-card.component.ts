import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CarRecommendation } from '../../models/car.model';

@Component({
  selector: 'app-car-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './car-card.component.html',
  styleUrl: './car-card.component.css'
})
export class CarCardComponent {
  @Input() car!: CarRecommendation;
  @Input() isBest = false;
}
