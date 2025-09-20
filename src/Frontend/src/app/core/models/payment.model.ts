import { User } from './user.model';
import { Booking } from './booking.model';

export interface Payment {
  id: string;
  bookingId: string;
  payerId: string;
  payeeId?: string;
  type: PaymentType;
  method: PaymentMethod;
  amount: number;
  platformFee: number;
  processingFee: number;
  netAmount: number;
  currency: string;
  status: PaymentStatus;
  externalTransactionId?: string;
  paymentIntentId?: string;
  failureReason?: string;
  processedAt?: Date;
  createdAt: Date;
  updatedAt: Date;
  booking?: Booking;
  payer?: User;
  payee?: User;
}

export interface PaymentMethod {
  id: string;
  userId: string;
  type: PaymentMethodType;
  last4Digits: string;
  brand: string;
  expiryMonth: number;
  expiryYear: number;
  bankName?: string;
  bankAccountType?: string;
  isDefault: boolean;
  isActive: boolean;
  externalMethodId: string;
  createdAt: Date;
  updatedAt: Date;
  user?: User;
}

export enum PaymentType {
  BookingPayment = 0,
  DepositPayment = 1,
  HostPayout = 2,
  Refund = 3,
  ServiceFee = 4
}

export enum PaymentStatus {
  Pending = 0,
  Processing = 1,
  Completed = 2,
  Failed = 3,
  Cancelled = 4,
  Refunded = 5,
  PartiallyRefunded = 6
}

export enum PaymentMethodType {
  CreditCard = 0,
  DebitCard = 1,
  BankAccount = 2,
  PayPal = 3,
  ApplePay = 4,
  GooglePay = 5
}

export interface CreatePaymentInput {
  bookingId: string;
  payerId: string;
  type: PaymentType;
  amount: number;
  currency: string;
  paymentMethodId: string;
}

export interface CreatePaymentMethodInput {
  userId: string;
  type: PaymentMethodType;
  last4Digits: string;
  brand: string;
  expiryMonth: number;
  expiryYear: number;
  bankName?: string;
  bankAccountType?: string;
  isDefault: boolean;
  externalMethodId: string;
}

export interface PaymentIntent {
  id: string;
  clientSecret: string;
  amount: number;
  currency: string;
  status: string;
}