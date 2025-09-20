import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-property-search',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="search-container">
      <h2>Property Search</h2>
      <p>Search functionality coming soon...</p>
    </div>
  `,
  styles: [`
    .search-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class PropertySearchComponent {
}