import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-property-detail',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="detail-container">
      <h2>Property Details</h2>
      <p>Property details coming soon...</p>
    </div>
  `,
  styles: [`
    .detail-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class PropertyDetailComponent {
}