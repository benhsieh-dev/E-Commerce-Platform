import { Routes } from '@angular/router';
import { authGuard } from '@core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/search',
    pathMatch: 'full'
  },
  {
    path: 'search',
    loadComponent: () => import('@features/property-search/property-search.component').then(m => m.PropertySearchComponent)
  },
  {
    path: 'property/:id',
    loadComponent: () => import('@features/property-detail/property-detail.component').then(m => m.PropertyDetailComponent)
  },
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () => import('@features/auth/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () => import('@features/auth/register/register.component').then(m => m.RegisterComponent)
      }
    ]
  },
  {
    path: 'booking',
    canActivate: [authGuard],
    children: [
      {
        path: 'create/:propertyId',
        loadComponent: () => import('@features/booking/booking-create/booking-create.component').then(m => m.BookingCreateComponent)
      },
      {
        path: ':id',
        loadComponent: () => import('@features/booking/booking-detail/booking-detail.component').then(m => m.BookingDetailComponent)
      }
    ]
  },
  {
    path: 'host',
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('@features/host-dashboard/host-dashboard.component').then(m => m.HostDashboardComponent)
      },
      {
        path: 'properties',
        loadComponent: () => import('@features/host-dashboard/property-management/property-management.component').then(m => m.PropertyManagementComponent)
      },
      {
        path: 'properties/create',
        loadComponent: () => import('@features/host-dashboard/property-create/property-create.component').then(m => m.PropertyCreateComponent)
      },
      {
        path: 'properties/edit/:id',
        loadComponent: () => import('@features/host-dashboard/property-edit/property-edit.component').then(m => m.PropertyEditComponent)
      },
      {
        path: 'bookings',
        loadComponent: () => import('@features/host-dashboard/host-bookings/host-bookings.component').then(m => m.HostBookingsComponent)
      }
    ]
  },
  {
    path: 'guest',
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('@features/guest-dashboard/guest-dashboard.component').then(m => m.GuestDashboardComponent)
      },
      {
        path: 'bookings',
        loadComponent: () => import('@features/guest-dashboard/guest-bookings/guest-bookings.component').then(m => m.GuestBookingsComponent)
      },
      {
        path: 'reviews',
        loadComponent: () => import('@features/guest-dashboard/guest-reviews/guest-reviews.component').then(m => m.GuestReviewsComponent)
      }
    ]
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () => import('@features/profile/profile.component').then(m => m.ProfileComponent)
  },
  {
    path: 'notifications',
    canActivate: [authGuard],
    loadComponent: () => import('@features/notifications/notifications.component').then(m => m.NotificationsComponent)
  },
  {
    path: '**',
    loadComponent: () => import('@shared/components/not-found/not-found.component').then(m => m.NotFoundComponent)
  }
];