import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="register-container">
      <h2>Register</h2>
      <p>Registration form coming soon...</p>
    </div>
  `,
  styles: [`
    .register-container {
      padding: 2rem;
      text-align: center;
    }
  `]
})
export class RegisterComponent {
}