import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthService } from '@core/services/auth.service';
import { LoginRequest } from '@core/models/user.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="login-container">
      <div class="login-content">
        <mat-card class="login-card">
          <mat-card-header>
            <mat-card-title>Welcome back</mat-card-title>
            <mat-card-subtitle>Sign in to your account</mat-card-subtitle>
          </mat-card-header>

          <mat-card-content>
            <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="login-form">
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Email</mat-label>
                <input matInput 
                       type="email" 
                       formControlName="email" 
                       placeholder="Enter your email"
                       autocomplete="email">
                <mat-icon matSuffix>email</mat-icon>
                <mat-error *ngIf="loginForm.get('email')?.hasError('required')">
                  Email is required
                </mat-error>
                <mat-error *ngIf="loginForm.get('email')?.hasError('email')">
                  Please enter a valid email
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Password</mat-label>
                <input matInput 
                       [type]="hidePassword ? 'password' : 'text'" 
                       formControlName="password"
                       placeholder="Enter your password"
                       autocomplete="current-password">
                <button mat-icon-button 
                        matSuffix 
                        (click)="hidePassword = !hidePassword" 
                        type="button"
                        [attr.aria-label]="'Hide password'" 
                        [attr.aria-pressed]="hidePassword">
                  <mat-icon>{{hidePassword ? 'visibility_off' : 'visibility'}}</mat-icon>
                </button>
                <mat-error *ngIf="loginForm.get('password')?.hasError('required')">
                  Password is required
                </mat-error>
                <mat-error *ngIf="loginForm.get('password')?.hasError('minlength')">
                  Password must be at least 6 characters
                </mat-error>
              </mat-form-field>

              <div class="form-actions">
                <button mat-raised-button 
                        color="primary" 
                        type="submit" 
                        [disabled]="loginForm.invalid || loading"
                        class="full-width login-button">
                  <mat-spinner *ngIf="loading" diameter="20"></mat-spinner>
                  <span *ngIf="!loading">Sign In</span>
                </button>
              </div>

              <div class="form-links">
                <a routerLink="/auth/forgot-password" class="forgot-link">
                  Forgot your password?
                </a>
              </div>
            </form>
          </mat-card-content>

          <mat-card-actions class="card-actions">
            <div class="register-prompt">
              <span>Don't have an account?</span>
              <a routerLink="/auth/register" class="register-link">Sign up</a>
            </div>
          </mat-card-actions>
        </mat-card>

        <div class="demo-accounts" *ngIf="!isProduction">
          <h3>Demo Accounts</h3>
          <div class="demo-buttons">
            <button mat-button (click)="loginAsDemo('guest')" class="demo-btn">
              <mat-icon>person</mat-icon>
              Login as Guest
            </button>
            <button mat-button (click)="loginAsDemo('host')" class="demo-btn">
              <mat-icon>home</mat-icon>
              Login as Host
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: calc(100vh - 160px);
      padding: 20px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }

    .login-content {
      width: 100%;
      max-width: 400px;
    }

    .login-card {
      padding: 32px;
      border-radius: 16px;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
    }

    .login-form {
      display: flex;
      flex-direction: column;
      gap: 20px;
      margin-top: 24px;
    }

    .full-width {
      width: 100%;
    }

    .form-actions {
      margin-top: 8px;
    }

    .login-button {
      height: 48px;
      font-size: 16px;
      font-weight: 500;
    }

    .form-links {
      text-align: center;
      margin-top: 16px;
    }

    .forgot-link {
      color: #ff5a5f;
      text-decoration: none;
      font-size: 14px;
      
      &:hover {
        text-decoration: underline;
      }
    }

    .card-actions {
      padding: 24px 0 0 0;
      justify-content: center;
      border-top: 1px solid #e0e0e0;
      margin-top: 24px;
    }

    .register-prompt {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 14px;
    }

    .register-prompt span {
      color: #767676;
    }

    .register-link {
      color: #ff5a5f;
      text-decoration: none;
      font-weight: 500;
      
      &:hover {
        text-decoration: underline;
      }
    }

    .demo-accounts {
      margin-top: 24px;
      padding: 20px;
      background: rgba(255, 255, 255, 0.9);
      border-radius: 12px;
      text-align: center;
    }

    .demo-accounts h3 {
      margin: 0 0 16px 0;
      color: #484848;
      font-size: 16px;
    }

    .demo-buttons {
      display: flex;
      gap: 12px;
    }

    .demo-btn {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      padding: 12px;
      font-size: 12px;
    }

    mat-card-header {
      text-align: center;
      margin-bottom: 8px;
    }

    mat-card-title {
      font-size: 28px;
      font-weight: 600;
      color: #484848;
    }

    mat-card-subtitle {
      font-size: 16px;
      color: #767676;
      margin-top: 8px;
    }

    @media (max-width: 480px) {
      .login-container {
        padding: 16px;
      }

      .login-card {
        padding: 24px;
      }

      .demo-buttons {
        flex-direction: column;
      }
    }
  `]
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  hidePassword = true;
  loading = false;
  returnUrl = '/';
  isProduction = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    // Get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    
    // Check if user is already logged in
    if (this.authService.isAuthenticated) {
      this.router.navigate([this.returnUrl]);
    }

    // Set production flag
    this.isProduction = false; // Set to true in production
  }

  onSubmit(): void {
    if (this.loginForm.valid && !this.loading) {
      this.loading = true;
      const credentials: LoginRequest = this.loginForm.value;

      this.authService.login(credentials).subscribe({
        next: (response) => {
          this.loading = false;
          this.snackBar.open('Login successful!', 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });

          // Redirect based on user role
          if (response.user.role === 1) { // Host
            this.router.navigate(['/host/dashboard']);
          } else {
            this.router.navigate([this.returnUrl]);
          }
        },
        error: (error) => {
          this.loading = false;
          this.snackBar.open(
            error.message || 'Login failed. Please try again.',
            'Close',
            {
              duration: 5000,
              panelClass: ['error-snackbar']
            }
          );
        }
      });
    }
  }

  loginAsDemo(type: 'guest' | 'host'): void {
    const demoCredentials = {
      guest: { email: 'guest@demo.com', password: 'password123' },
      host: { email: 'host@demo.com', password: 'password123' }
    };

    this.loginForm.patchValue(demoCredentials[type]);
    this.onSubmit();
  }
}