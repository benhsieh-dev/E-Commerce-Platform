import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="profile-container">
      <h2>Profile</h2>
      <p>Profile page coming soon...</p>
    </div>
  `,
  styles: [`
    .profile-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class ProfileComponent {
}