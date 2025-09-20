import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-guest-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="guest-dashboard-container">
      <h2>Guest Dashboard</h2>
      <p>Guest dashboard coming soon...</p>
    </div>
  `,
  styles: [`
    .guest-dashboard-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class GuestDashboardComponent {
}