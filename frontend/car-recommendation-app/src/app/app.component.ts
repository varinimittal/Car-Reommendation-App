import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RecommendationFormComponent } from './components/recommendation-form/recommendation-form.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RecommendationFormComponent, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {}
