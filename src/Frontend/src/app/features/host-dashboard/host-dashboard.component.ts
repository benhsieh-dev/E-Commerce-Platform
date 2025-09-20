import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-host-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="host-dashboard-container">
      <h2>Host Dashboard</h2>
      <p>Host dashboard coming soon...</p>
    </div>
  `,
  styles: [`
    .host-dashboard-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class HostDashboardComponent {
}