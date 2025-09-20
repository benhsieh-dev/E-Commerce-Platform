import { User } from './user.model';
import { Booking } from './booking.model';
import { Property } from './property.model';

export interface Review {
  id: string;
  bookingId: string;
  reviewerId: string;
  revieweeId: string;
  type: ReviewType;
  overallRating: number;
  cleanlinessRating?: number;
  accuracyRating?: number;
  checkInRating?: number;
  communicationRating?: number;
  locationRating?: number;
  valueRating?: number;
  guestCommunicationRating?: number;
  guestCleanlinessRating?: number;
  guestRespectRating?: number;
  comment: string;
  status: ReviewStatus;
  createdAt: Date;
  updatedAt: Date;
  publishedAt?: Date;
  isAnonymous: boolean;
  responseComment?: string;
  responseDate?: Date;
  helpfulVotes: number;
  totalVotes: number;
  isFlagged: boolean;
  flagReason?: string;
  flaggedAt?: Date;
  flaggedByUserId?: string;
  moderationStatus: ModerationStatus;
  moderationNotes?: string;
  moderatedAt?: Date;
  moderatedByUserId?: string;
  booking?: Booking;
  reviewer?: User;
  reviewee?: User;
  property?: Property;
}

export enum ReviewType {
  PropertyReview = 0,
  GuestReview = 1
}

export enum ReviewStatus {
  Draft = 0,
  Published = 1,
  Hidden = 2,
  Removed = 3
}

export enum ModerationStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
  Flagged = 3,
  UnderReview = 4
}

export interface CreateReviewInput {
  bookingId: string;
  reviewerId: string;
  revieweeId: string;
  type: ReviewType;
  overallRating: number;
  cleanlinessRating?: number;
  accuracyRating?: number;
  checkInRating?: number;
  communicationRating?: number;
  locationRating?: number;
  valueRating?: number;
  guestCommunicationRating?: number;
  guestCleanlinessRating?: number;
  guestRespectRating?: number;
  comment: string;
  isAnonymous: boolean;
}

export interface UpdateReviewInput {
  overallRating?: number;
  cleanlinessRating?: number;
  accuracyRating?: number;
  checkInRating?: number;
  communicationRating?: number;
  locationRating?: number;
  valueRating?: number;
  guestCommunicationRating?: number;
  guestCleanlinessRating?: number;
  guestRespectRating?: number;
  comment?: string;
  isAnonymous?: boolean;
}

export interface ReviewSearchFilters {
  propertyId?: string;
  reviewerId?: string;
  revieweeId?: string;
  type?: ReviewType;
  status?: ReviewStatus;
  minRating?: number;
  maxRating?: number;
  createdFrom?: Date;
  createdTo?: Date;
  isFlagged?: boolean;
  moderationStatus?: ModerationStatus;
}