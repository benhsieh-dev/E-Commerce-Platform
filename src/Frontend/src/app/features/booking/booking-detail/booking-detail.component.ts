import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-booking-detail',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="booking-detail-container">
      <h2>Booking Details</h2>
      <p>Booking details coming soon...</p>
    </div>
  `,
  styles: [`
    .booking-detail-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class BookingDetailComponent {
}