import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-property-management',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="property-management-container">
      <h2>Property Management</h2>
      <p>Property management coming soon...</p>
    </div>
  `,
  styles: [`
    .property-management-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class PropertyManagementComponent {
}