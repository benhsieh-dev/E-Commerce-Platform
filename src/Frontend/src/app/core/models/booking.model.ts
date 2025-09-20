import { User } from './user.model';
import { Property } from './property.model';
import { Payment } from './payment.model';
import { Review } from './review.model';

export interface Booking {
  id: string;
  propertyId: string;
  guestId: string;
  hostId: string;
  checkInDate: Date;
  checkOutDate: Date;
  numberOfGuests: number;
  numberOfNights: number;
  propertyPricePerNight: number;
  subTotal: number;
  cleaningFee: number;
  serviceFee: number;
  taxAmount: number;
  totalAmount: number;
  status: BookingStatus;
  specialRequests?: string;
  cancellationPolicy: CancellationPolicy;
  cancelledAt?: Date;
  cancellationReason?: string;
  refundAmount: number;
  createdAt: Date;
  updatedAt: Date;
  checkedInAt?: Date;
  checkedOutAt?: Date;
  property?: Property;
  guest?: User;
  host?: User;
  payments?: Payment[];
  reviews?: Review[];
}

export enum BookingStatus {
  Pending = 0,
  Confirmed = 1,
  CheckedIn = 2,
  Completed = 3,
  Cancelled = 4,
  Expired = 5
}

export enum CancellationPolicy {
  Flexible = 0,
  Moderate = 1,
  Strict = 2,
  SuperStrict = 3
}

export interface CreateBookingInput {
  propertyId: string;
  guestId: string;
  checkInDate: Date;
  checkOutDate: Date;
  numberOfGuests: number;
  specialRequests?: string;
}

export interface UpdateBookingInput {
  checkInDate?: Date;
  checkOutDate?: Date;
  numberOfGuests?: number;
  specialRequests?: string;
}

export interface BookingSearchFilters {
  guestId?: string;
  hostId?: string;
  propertyId?: string;
  status?: BookingStatus;
  checkInDateFrom?: Date;
  checkInDateTo?: Date;
  checkOutDateFrom?: Date;
  checkOutDateTo?: Date;
  createdFrom?: Date;
  createdTo?: Date;
}

export interface BookingPriceBreakdown {
  pricePerNight: number;
  numberOfNights: number;
  subTotal: number;
  cleaningFee: number;
  serviceFee: number;
  taxAmount: number;
  totalAmount: number;
  weeklyDiscount?: number;
  monthlyDiscount?: number;
}