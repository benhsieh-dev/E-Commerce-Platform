import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-property-edit',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="property-edit-container">
      <h2>Edit Property</h2>
      <p>Property edit form coming soon...</p>
    </div>
  `,
  styles: [`
    .property-edit-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class PropertyEditComponent {
}