import { User } from './user.model';
import { Review } from './review.model';

export interface Property {
  id: string;
  hostId: string;
  title: string;
  description: string;
  propertyType: PropertyType;
  pricePerNight: number;
  maxGuests: number;
  bedrooms: number;
  bathrooms: number;
  address: string;
  city: string;
  state: string;
  country: string;
  postalCode: string;
  latitude: number;
  longitude: number;
  amenities: string[];
  images: string[];
  houseRules: string[];
  isInstantBook: boolean;
  minimumStay: number;
  maximumStay: number;
  weeklyDiscount?: number;
  monthlyDiscount?: number;
  status: PropertyStatus;
  createdAt: Date;
  updatedAt: Date;
  unavailableDates: Date[];
  averageRating: number;
  reviewCount: number;
  host?: User;
  reviews?: Review[];
}

export enum PropertyType {
  Apartment = 0,
  House = 1,
  Condo = 2,
  Villa = 3,
  Cabin = 4,
  Room = 5,
  Studio = 6,
  Loft = 7
}

export enum PropertyStatus {
  Draft = 0,
  Active = 1,
  Inactive = 2,
  Suspended = 3
}

export interface CreatePropertyInput {
  hostId: string;
  title: string;
  description: string;
  propertyType: PropertyType;
  pricePerNight: number;
  maxGuests: number;
  bedrooms: number;
  bathrooms: number;
  address: string;
  city: string;
  state: string;
  country: string;
  postalCode: string;
  latitude: number;
  longitude: number;
  amenities: string[];
  images: string[];
  houseRules: string[];
  isInstantBook: boolean;
  minimumStay: number;
  maximumStay: number;
  weeklyDiscount?: number;
  monthlyDiscount?: number;
}

export interface UpdatePropertyInput {
  title?: string;
  description?: string;
  propertyType?: PropertyType;
  pricePerNight?: number;
  maxGuests?: number;
  bedrooms?: number;
  bathrooms?: number;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  postalCode?: string;
  latitude?: number;
  longitude?: number;
  amenities?: string[];
  images?: string[];
  houseRules?: string[];
  isInstantBook?: boolean;
  minimumStay?: number;
  maximumStay?: number;
  weeklyDiscount?: number;
  monthlyDiscount?: number;
  status?: PropertyStatus;
}

export interface PropertySearchFilters {
  location?: string;
  checkIn?: Date;
  checkOut?: Date;
  guests?: number;
  minPrice?: number;
  maxPrice?: number;
  propertyType?: PropertyType;
  amenities?: string[];
  instantBookOnly?: boolean;
  latitude?: number;
  longitude?: number;
  radiusKm?: number;
}

export interface PropertySearchResult {
  properties: Property[];
  totalCount: number;
  hasMore: boolean;
}