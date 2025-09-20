import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { map } from 'rxjs/operators';

export interface Notification {
  id: string;
  userId: string;
  type: number;
  channel: number;
  title: string;
  content: string;
  status: number;
  priority: number;
  isRead: boolean;
  isArchived: boolean;
  createdAt: Date;
  updatedAt: Date;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private unreadCountSubject = new BehaviorSubject<number>(0);
  public unreadCount$ = this.unreadCountSubject.asObservable();

  constructor() {}

  initializeNotifications(userId: string): void {
    this.getUnreadCount(userId).subscribe(count => {
      this.unreadCountSubject.next(count);
    });
  }

  getUserNotifications(userId: string, skip = 0, take = 20): Observable<Notification[]> {
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  getUnreadCount(userId: string): Observable<number> {
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  markAsRead(notificationIds: string[]): Observable<boolean> {
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }

  deleteNotifications(notificationIds: string[]): Observable<boolean> {
    // TODO: Replace with actual GraphQL call when Apollo is configured
    return throwError(() => new Error('GraphQL not configured yet'));
  }
}