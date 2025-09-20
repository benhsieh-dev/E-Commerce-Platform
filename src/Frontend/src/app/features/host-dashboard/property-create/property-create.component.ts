import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-property-create',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="property-create-container">
      <h2>Create Property</h2>
      <p>Property creation form coming soon...</p>
    </div>
  `,
  styles: [`
    .property-create-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class PropertyCreateComponent {
}