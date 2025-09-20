import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError, of } from 'rxjs';
import { map, catchError, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';

import { User, LoginRequest, RegisterRequest, AuthResponse } from '@core/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private tokenKey = 'token';
  private userKey = 'user';

  public currentUser$ = this.currentUserSubject.asObservable();
  public loading$ = this.loadingSubject.asObservable();

  constructor(
    private router: Router
  ) {}

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  public get isAuthenticated(): boolean {
    return !!this.currentUserValue && !!this.getToken();
  }

  public get isHost(): boolean {
    return this.currentUserValue?.role === 1; // UserRole.Host
  }

  public get isGuest(): boolean {
    return this.currentUserValue?.role === 0; // UserRole.Guest
  }

  public get isAdmin(): boolean {
    return this.currentUserValue?.role === 2; // UserRole.Admin
  }

  initializeAuth(): void {
    const token = this.getToken();
    const userData = localStorage.getItem(this.userKey);
    
    if (token && userData) {
      try {
        const user = JSON.parse(userData);
        this.currentUserSubject.next(user);
      } catch (error) {
        this.logout();
      }
    }
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    this.loadingSubject.next(true);
    
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  register(userData: RegisterRequest): Observable<User> {
    this.loadingSubject.next(true);
    
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  refreshToken(): Observable<string> {
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  updateProfile(userData: Partial<User>): Observable<User> {
    this.loadingSubject.next(true);
    
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  changePassword(currentPassword: string, newPassword: string): Observable<boolean> {
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  requestPasswordReset(email: string): Observable<boolean> {
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  resetPassword(token: string, newPassword: string): Observable<boolean> {
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  verifyEmail(token: string): Observable<boolean> {
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private getUserProfile(): Observable<User> {
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  private setSession(authResponse: AuthResponse): void {
    localStorage.setItem(this.tokenKey, authResponse.token);
    localStorage.setItem(this.userKey, JSON.stringify(authResponse.user));
    this.currentUserSubject.next(authResponse.user);
  }

  private handleError = (error: any) => {
    this.loadingSubject.next(false);
    console.error('Auth Service Error:', error);
    
    let errorMessage = 'An error occurred';
    if (error.graphQLErrors?.length) {
      errorMessage = error.graphQLErrors[0].message;
    } else if (error.networkError) {
      errorMessage = 'Network error occurred';
    }
    
    return throwError(() => new Error(errorMessage));
  };
}