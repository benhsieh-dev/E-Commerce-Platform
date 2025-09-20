import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-guest-reviews',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="guest-reviews-container">
      <h2>Guest Reviews</h2>
      <p>Guest reviews coming soon...</p>
    </div>
  `,
  styles: [`
    .guest-reviews-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class GuestReviewsComponent {
}