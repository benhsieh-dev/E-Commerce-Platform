import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-host-bookings',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="host-bookings-container">
      <h2>Host Bookings</h2>
      <p>Host bookings coming soon...</p>
    </div>
  `,
  styles: [`
    .host-bookings-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class HostBookingsComponent {
}