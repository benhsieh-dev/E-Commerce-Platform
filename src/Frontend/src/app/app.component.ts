import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';

import { HeaderComponent } from '@shared/components/header/header.component';
import { FooterComponent } from '@shared/components/footer/footer.component';
import { LoadingSpinnerComponent } from '@shared/components/loading-spinner/loading-spinner.component';
import { AuthService } from '@core/services/auth.service';
import { NotificationService } from '@core/services/notification.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatBadgeModule,
    HeaderComponent,
    FooterComponent,
    LoadingSpinnerComponent
  ],
  template: `
    <div class="app-container">
      <app-header></app-header>
      
      <main class="main-content">
        <router-outlet></router-outlet>
      </main>
      
      <app-footer></app-footer>
      
      <app-loading-spinner 
        *ngIf="isLoading" 
        [overlay]="true">
      </app-loading-spinner>
    </div>
  `,
  styles: [`
    .app-container {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
    }

    .main-content {
      flex: 1;
      padding-top: 80px; // Account for fixed header
    }

    @media (max-width: 768px) {
      .main-content {
        padding-top: 60px;
      }
    }
  `]
})
export class AppComponent implements OnInit {
  isLoading = false;

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    // Initialize auth state
    this.authService.initializeAuth();
    
    // Subscribe to loading state
    this.authService.loading$.subscribe(loading => {
      this.isLoading = loading;
    });
    
    // Initialize notifications if user is authenticated
    this.authService.currentUser$.subscribe(user => {
      if (user) {
        this.notificationService.initializeNotifications(user.id);
      }
    });
  }
}