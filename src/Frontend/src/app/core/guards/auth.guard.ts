import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@core/services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated) {
    return true;
  }

  // Store the attempted URL for redirecting after login
  router.navigate(['/auth/login'], { 
    queryParams: { returnUrl: state.url } 
  });
  
  return false;
};

export const guestOnlyGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated) {
    return true;
  }

  // Redirect authenticated users to dashboard
  if (authService.isHost) {
    router.navigate(['/host/dashboard']);
  } else {
    router.navigate(['/guest/dashboard']);
  }
  
  return false;
};

export const hostGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated && authService.isHost) {
    return true;
  }

  if (!authService.isAuthenticated) {
    router.navigate(['/auth/login'], { 
      queryParams: { returnUrl: state.url } 
    });
  } else {
    // Authenticated but not a host
    router.navigate(['/guest/dashboard']);
  }
  
  return false;
};

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated && authService.isAdmin) {
    return true;
  }

  if (!authService.isAuthenticated) {
    router.navigate(['/auth/login'], { 
      queryParams: { returnUrl: state.url } 
    });
  } else {
    // Authenticated but not admin
    router.navigate(['/']);
  }
  
  return false;
};