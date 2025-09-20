import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-booking-create',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="booking-create-container">
      <h2>Create Booking</h2>
      <p>Booking creation form coming soon...</p>
    </div>
  `,
  styles: [`
    .booking-create-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class BookingCreateComponent {
}