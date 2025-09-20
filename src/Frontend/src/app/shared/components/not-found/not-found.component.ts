import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="not-found-container">
      <div class="not-found-content">
        <div class="error-icon">
          <mat-icon>error_outline</mat-icon>
        </div>
        
        <h1>404</h1>
        <h2>Page Not Found</h2>
        <p>The page you're looking for doesn't exist or has been moved.</p>
        
        <div class="actions">
          <a routerLink="/" mat-raised-button color="primary">
            <mat-icon>home</mat-icon>
            Go Home
          </a>
          <button mat-button (click)="goBack()">
            <mat-icon>arrow_back</mat-icon>
            Go Back
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .not-found-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 60vh;
      padding: 40px 20px;
    }

    .not-found-content {
      text-align: center;
      max-width: 500px;
    }

    .error-icon {
      margin-bottom: 24px;
      
      mat-icon {
        font-size: 80px;
        width: 80px;
        height: 80px;
        color: #ff5a5f;
      }
    }

    h1 {
      font-size: 72px;
      font-weight: 700;
      color: #ff5a5f;
      margin: 0 0 16px 0;
    }

    h2 {
      font-size: 32px;
      font-weight: 600;
      color: #484848;
      margin: 0 0 16px 0;
    }

    p {
      font-size: 16px;
      color: #767676;
      margin: 0 0 32px 0;
      line-height: 1.5;
    }

    .actions {
      display: flex;
      gap: 16px;
      justify-content: center;
      flex-wrap: wrap;
    }

    .actions button,
    .actions a {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    @media (max-width: 768px) {
      h1 {
        font-size: 56px;
      }

      h2 {
        font-size: 24px;
      }

      .actions {
        flex-direction: column;
        align-items: center;
      }

      .actions button,
      .actions a {
        width: 200px;
        justify-content: center;
      }
    }
  `]
})
export class NotFoundComponent {
  goBack(): void {
    window.history.back();
  }
}