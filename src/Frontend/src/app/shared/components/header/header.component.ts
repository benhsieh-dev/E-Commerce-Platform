import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatDividerModule } from '@angular/material/divider';

import { AuthService } from '@core/services/auth.service';
import { NotificationService } from '@core/services/notification.service';
import { User } from '@core/models/user.model';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatBadgeModule,
    MatDividerModule
  ],
  template: `
    <mat-toolbar class="header-toolbar">
      <div class="container">
        <div class="header-content">
          <!-- Logo -->
          <a routerLink="/" class="logo">
            <mat-icon>home</mat-icon>
            <span>AirBnB Platform</span>
          </a>

          <!-- Navigation -->
          <nav class="nav-links" *ngIf="!isMobile">
            <a routerLink="/search" routerLinkActive="active" class="nav-link">Search</a>
            <a routerLink="/host/properties" routerLinkActive="active" class="nav-link" *ngIf="currentUser">Become a Host</a>
          </nav>

          <!-- User Menu -->
          <div class="user-menu">
            <ng-container *ngIf="currentUser; else guestMenu">
              <!-- Notifications -->
              <button mat-icon-button 
                      [matMenuTriggerFor]="notificationMenu"
                      [matBadge]="unreadNotifications"
                      [matBadgeHidden]="unreadNotifications === 0"
                      matBadgeColor="warn">
                <mat-icon>notifications</mat-icon>
              </button>

              <!-- User Avatar Menu -->
              <button mat-button [matMenuTriggerFor]="userMenu" class="user-avatar-btn">
                <div class="user-avatar">
                  <img [src]="currentUser.profilePictureUrl || '/assets/default-avatar.png'" 
                       [alt]="currentUser.firstName"
                       class="avatar-img">
                </div>
                <span class="user-name">{{currentUser.firstName}}</span>
                <mat-icon>keyboard_arrow_down</mat-icon>
              </button>
            </ng-container>

            <!-- Guest Menu -->
            <ng-template #guestMenu>
              <a routerLink="/auth/login" mat-button class="auth-btn">Log in</a>
              <a routerLink="/auth/register" mat-raised-button color="primary" class="auth-btn">Sign up</a>
            </ng-template>
          </div>
        </div>
      </div>
    </mat-toolbar>

    <!-- User Menu -->
    <mat-menu #userMenu="matMenu" class="user-dropdown">
      <div class="user-info" mat-menu-item disabled>
        <div class="user-details">
          <strong>{{currentUser?.firstName}} {{currentUser?.lastName}}</strong>
          <small>{{currentUser?.email}}</small>
        </div>
      </div>
      <mat-divider></mat-divider>
      
      <a routerLink="/profile" mat-menu-item>
        <mat-icon>person</mat-icon>
        <span>Profile</span>
      </a>
      
      <ng-container *ngIf="isHost">
        <a routerLink="/host/dashboard" mat-menu-item>
          <mat-icon>dashboard</mat-icon>
          <span>Host Dashboard</span>
        </a>
        <a routerLink="/host/properties" mat-menu-item>
          <mat-icon>home_work</mat-icon>
          <span>My Properties</span>
        </a>
        <a routerLink="/host/bookings" mat-menu-item>
          <mat-icon>event</mat-icon>
          <span>Host Bookings</span>
        </a>
      </ng-container>

      <ng-container *ngIf="isGuest">
        <a routerLink="/guest/dashboard" mat-menu-item>
          <mat-icon>dashboard</mat-icon>
          <span>Guest Dashboard</span>
        </a>
        <a routerLink="/guest/bookings" mat-menu-item>
          <mat-icon>event</mat-icon>
          <span>My Trips</span>
        </a>
        <a routerLink="/guest/reviews" mat-menu-item>
          <mat-icon>star</mat-icon>
          <span>My Reviews</span>
        </a>
      </ng-container>

      <mat-divider></mat-divider>
      
      <button mat-menu-item (click)="logout()">
        <mat-icon>logout</mat-icon>
        <span>Log out</span>
      </button>
    </mat-menu>

    <!-- Notifications Menu -->
    <mat-menu #notificationMenu="matMenu" class="notification-dropdown">
      <div class="notification-header" mat-menu-item disabled>
        <strong>Notifications</strong>
        <button mat-button color="primary" (click)="markAllAsRead()" 
                [disabled]="unreadNotifications === 0">
          Mark all as read
        </button>
      </div>
      <mat-divider></mat-divider>
      
      <div class="notification-list">
        <ng-container *ngIf="notifications.length > 0; else noNotifications">
          <div *ngFor="let notification of notifications" 
               mat-menu-item 
               class="notification-item"
               [class.unread]="!notification.isRead"
               (click)="markAsRead(notification.id)">
            <div class="notification-content">
              <div class="notification-title">{{notification.title}}</div>
              <div class="notification-message">{{notification.content}}</div>
              <div class="notification-time">{{notification.createdAt | date:'short'}}</div>
            </div>
          </div>
        </ng-container>
        
        <ng-template #noNotifications>
          <div mat-menu-item disabled class="no-notifications">
            No notifications
          </div>
        </ng-template>
      </div>
      
      <mat-divider></mat-divider>
      <a routerLink="/notifications" mat-menu-item>
        <span>View all notifications</span>
      </a>
    </mat-menu>
  `,
  styles: [`
    .header-toolbar {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      z-index: 1000;
      background-color: white;
      border-bottom: 1px solid #e0e0e0;
      height: 80px;
    }

    .header-content {
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 100%;
    }

    .logo {
      display: flex;
      align-items: center;
      text-decoration: none;
      color: #ff5a5f;
      font-size: 20px;
      font-weight: 700;
      
      mat-icon {
        margin-right: 8px;
        font-size: 28px;
        width: 28px;
        height: 28px;
      }
    }

    .nav-links {
      display: flex;
      align-items: center;
      gap: 24px;
    }

    .nav-link {
      color: #484848;
      text-decoration: none;
      font-weight: 500;
      transition: color 0.3s ease;
      
      &:hover, &.active {
        color: #ff5a5f;
      }
    }

    .user-menu {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .auth-btn {
      font-weight: 500;
    }

    .user-avatar-btn {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 8px 16px;
      border-radius: 24px;
      border: 1px solid #e0e0e0;
      background: white;
      transition: box-shadow 0.3s ease;
      
      &:hover {
        box-shadow: 0 2px 8px rgba(0,0,0,0.12);
      }
    }

    .user-avatar {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      overflow: hidden;
    }

    .avatar-img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .user-name {
      font-weight: 500;
      color: #484848;
    }

    .user-dropdown {
      min-width: 240px;
    }

    .user-info {
      padding: 16px !important;
    }

    .user-details strong {
      display: block;
      color: #484848;
    }

    .user-details small {
      color: #767676;
    }

    .notification-dropdown {
      min-width: 320px;
      max-height: 400px;
    }

    .notification-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px !important;
    }

    .notification-list {
      max-height: 300px;
      overflow-y: auto;
    }

    .notification-item {
      white-space: normal !important;
      height: auto !important;
      padding: 12px 16px !important;
      border-left: 3px solid transparent;
      
      &.unread {
        background-color: #f5f5f5;
        border-left-color: #ff5a5f;
      }
    }

    .notification-content {
      width: 100%;
    }

    .notification-title {
      font-weight: 500;
      color: #484848;
      margin-bottom: 4px;
    }

    .notification-message {
      font-size: 14px;
      color: #767676;
      margin-bottom: 4px;
    }

    .notification-time {
      font-size: 12px;
      color: #b0b0b0;
    }

    .no-notifications {
      text-align: center;
      color: #767676;
      padding: 24px !important;
    }

    @media (max-width: 768px) {
      .nav-links {
        display: none;
      }
      
      .user-name {
        display: none;
      }
      
      .header-toolbar {
        height: 60px;
      }
    }
  `]
})
export class HeaderComponent implements OnInit {
  currentUser: User | null = null;
  notifications: any[] = [];
  unreadNotifications = 0;
  isMobile = false;

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      if (user) {
        this.loadNotifications();
      }
    });

    this.checkMobile();
    window.addEventListener('resize', () => this.checkMobile());
  }

  get isHost(): boolean {
    return this.authService.isHost;
  }

  get isGuest(): boolean {
    return this.authService.isGuest;
  }

  logout(): void {
    this.authService.logout();
  }

  private loadNotifications(): void {
    if (this.currentUser) {
      this.notificationService.getUserNotifications(this.currentUser.id, 0, 10)
        .subscribe(notifications => {
          this.notifications = notifications;
          this.unreadNotifications = notifications.filter(n => !n.isRead).length;
        });
    }
  }

  markAsRead(notificationId: string): void {
    this.notificationService.markAsRead([notificationId])
      .subscribe(() => {
        const notification = this.notifications.find(n => n.id === notificationId);
        if (notification) {
          notification.isRead = true;
          this.unreadNotifications = this.notifications.filter(n => !n.isRead).length;
        }
      });
  }

  markAllAsRead(): void {
    const unreadIds = this.notifications
      .filter(n => !n.isRead)
      .map(n => n.id);
    
    if (unreadIds.length > 0) {
      this.notificationService.markAsRead(unreadIds)
        .subscribe(() => {
          this.notifications.forEach(n => n.isRead = true);
          this.unreadNotifications = 0;
        });
    }
  }

  private checkMobile(): void {
    this.isMobile = window.innerWidth < 768;
  }
}