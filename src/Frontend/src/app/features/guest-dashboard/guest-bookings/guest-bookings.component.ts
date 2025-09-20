import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-guest-bookings',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="guest-bookings-container">
      <h2>Guest Bookings</h2>
      <p>Guest bookings coming soon...</p>
    </div>
  `,
  styles: [`
    .guest-bookings-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class GuestBookingsComponent {
}