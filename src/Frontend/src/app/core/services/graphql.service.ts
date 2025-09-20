import { Injectable } from '@angular/core';
import { Apollo, gql } from 'apollo-angular';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

import { 
  Property, 
  PropertySearchFilters, 
  PropertySearchResult,
  CreatePropertyInput,
  UpdatePropertyInput 
} from '@core/models/property.model';
import { 
  Booking, 
  BookingSearchFilters, 
  CreateBookingInput, 
  UpdateBookingInput,
  BookingStatus 
} from '@core/models/booking.model';
import { 
  Payment, 
  PaymentMethod, 
  CreatePaymentInput, 
  CreatePaymentMethodInput 
} from '@core/models/payment.model';
import { 
  Review, 
  ReviewSearchFilters, 
  CreateReviewInput, 
  UpdateReviewInput 
} from '@core/models/review.model';
import { User } from '@core/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class GraphQLService {
  constructor(private apollo: Apollo) {}

  // Property Queries
  searchProperties(filters?: PropertySearchFilters, skip = 0, take = 20): Observable<Property[]> {
    return this.apollo.query<{ searchProperties: Property[] }>({
      query: SEARCH_PROPERTIES_QUERY,
      variables: {
        location: filters?.location,
        checkIn: filters?.checkIn,
        checkOut: filters?.checkOut,
        guests: filters?.guests,
        minPrice: filters?.minPrice,
        maxPrice: filters?.maxPrice,
        skip,
        take
      }
    }).pipe(
      map(result => result.data.searchProperties || [])
    );
  }

  getProperty(id: string): Observable<Property> {
    return this.apollo.query<{ property: Property }>({
      query: GET_PROPERTY_QUERY,
      variables: { id }
    }).pipe(
      map(result => result.data.property)
    );
  }

  getPropertiesByHost(hostId: string, skip = 0, take = 20): Observable<Property[]> {
    return this.apollo.query<{ propertiesByHost: Property[] }>({
      query: GET_PROPERTIES_BY_HOST_QUERY,
      variables: { hostId, skip, take }
    }).pipe(
      map(result => result.data.propertiesByHost || [])
    );
  }

  checkPropertyAvailability(propertyId: string, checkIn: Date, checkOut: Date): Observable<boolean> {
    return this.apollo.query<{ checkPropertyAvailability: boolean }>({
      query: CHECK_PROPERTY_AVAILABILITY_QUERY,
      variables: { propertyId, checkIn, checkOut }
    }).pipe(
      map(result => result.data.checkPropertyAvailability)
    );
  }

  // Property Mutations
  createProperty(property: CreatePropertyInput): Observable<Property> {
    return this.apollo.mutate<{ createProperty: Property }>({
      mutation: CREATE_PROPERTY_MUTATION,
      variables: { property }
    }).pipe(
      map(result => result.data!.createProperty)
    );
  }

  updateProperty(id: string, property: UpdatePropertyInput): Observable<Property> {
    return this.apollo.mutate<{ updateProperty: Property }>({
      mutation: UPDATE_PROPERTY_MUTATION,
      variables: { id, property }
    }).pipe(
      map(result => result.data!.updateProperty)
    );
  }

  deleteProperty(id: string): Observable<boolean> {
    return this.apollo.mutate<{ deleteProperty: boolean }>({
      mutation: DELETE_PROPERTY_MUTATION,
      variables: { id }
    }).pipe(
      map(result => result.data!.deleteProperty)
    );
  }

  // Booking Queries
  getBooking(id: string): Observable<Booking> {
    return this.apollo.query<{ booking: Booking }>({
      query: GET_BOOKING_QUERY,
      variables: { id }
    }).pipe(
      map(result => result.data.booking)
    );
  }

  getBookingsByGuest(guestId: string, skip = 0, take = 20): Observable<Booking[]> {
    return this.apollo.query<{ bookingsByGuest: Booking[] }>({
      query: GET_BOOKINGS_BY_GUEST_QUERY,
      variables: { guestId, skip, take }
    }).pipe(
      map(result => result.data.bookingsByGuest || [])
    );
  }

  getBookingsByHost(hostId: string, skip = 0, take = 20): Observable<Booking[]> {
    return this.apollo.query<{ bookingsByHost: Booking[] }>({
      query: GET_BOOKINGS_BY_HOST_QUERY,
      variables: { hostId, skip, take }
    }).pipe(
      map(result => result.data.bookingsByHost || [])
    );
  }

  getBookingsByProperty(propertyId: string, skip = 0, take = 20): Observable<Booking[]> {
    return this.apollo.query<{ bookingsByProperty: Booking[] }>({
      query: GET_BOOKINGS_BY_PROPERTY_QUERY,
      variables: { propertyId, skip, take }
    }).pipe(
      map(result => result.data.bookingsByProperty || [])
    );
  }

  // Booking Mutations
  createBooking(booking: CreateBookingInput): Observable<Booking> {
    return this.apollo.mutate<{ createBooking: Booking }>({
      mutation: CREATE_BOOKING_MUTATION,
      variables: { booking }
    }).pipe(
      map(result => result.data!.createBooking)
    );
  }

  updateBooking(id: string, booking: UpdateBookingInput): Observable<Booking> {
    return this.apollo.mutate<{ updateBooking: Booking }>({
      mutation: UPDATE_BOOKING_MUTATION,
      variables: { id, booking }
    }).pipe(
      map(result => result.data!.updateBooking)
    );
  }

  updateBookingStatus(id: string, status: BookingStatus): Observable<Booking> {
    return this.apollo.mutate<{ updateBookingStatus: Booking }>({
      mutation: UPDATE_BOOKING_STATUS_MUTATION,
      variables: { id, status }
    }).pipe(
      map(result => result.data!.updateBookingStatus)
    );
  }

  cancelBooking(id: string, reason: string): Observable<Booking> {
    return this.apollo.mutate<{ cancelBooking: Booking }>({
      mutation: CANCEL_BOOKING_MUTATION,
      variables: { id, reason }
    }).pipe(
      map(result => result.data!.cancelBooking)
    );
  }

  checkInBooking(id: string): Observable<boolean> {
    return this.apollo.mutate<{ checkInBooking: boolean }>({
      mutation: CHECK_IN_BOOKING_MUTATION,
      variables: { id }
    }).pipe(
      map(result => result.data!.checkInBooking)
    );
  }

  checkOutBooking(id: string): Observable<boolean> {
    return this.apollo.mutate<{ checkOutBooking: boolean }>({
      mutation: CHECK_OUT_BOOKING_MUTATION,
      variables: { id }
    }).pipe(
      map(result => result.data!.checkOutBooking)
    );
  }

  // Payment Methods
  getUserPaymentMethods(userId: string): Observable<PaymentMethod[]> {
    return this.apollo.query<{ userPaymentMethods: PaymentMethod[] }>({
      query: GET_USER_PAYMENT_METHODS_QUERY,
      variables: { userId }
    }).pipe(
      map(result => result.data.userPaymentMethods || [])
    );
  }

  createPaymentMethod(paymentMethod: CreatePaymentMethodInput): Observable<PaymentMethod> {
    return this.apollo.mutate<{ createPaymentMethod: PaymentMethod }>({
      mutation: CREATE_PAYMENT_METHOD_MUTATION,
      variables: { paymentMethod }
    }).pipe(
      map(result => result.data!.createPaymentMethod)
    );
  }

  deletePaymentMethod(id: string): Observable<boolean> {
    return this.apollo.mutate<{ deletePaymentMethod: boolean }>({
      mutation: DELETE_PAYMENT_METHOD_MUTATION,
      variables: { id }
    }).pipe(
      map(result => result.data!.deletePaymentMethod)
    );
  }

  // Reviews
  getReviewsByProperty(propertyId: string, skip = 0, take = 20): Observable<Review[]> {
    return this.apollo.query<{ reviewsByProperty: Review[] }>({
      query: GET_REVIEWS_BY_PROPERTY_QUERY,
      variables: { propertyId, skip, take }
    }).pipe(
      map(result => result.data.reviewsByProperty || [])
    );
  }

  createReview(review: CreateReviewInput): Observable<Review> {
    return this.apollo.mutate<{ createReview: Review }>({
      mutation: CREATE_REVIEW_MUTATION,
      variables: { review }
    }).pipe(
      map(result => result.data!.createReview)
    );
  }

  updateReview(id: string, review: UpdateReviewInput): Observable<Review> {
    return this.apollo.mutate<{ updateReview: Review }>({
      mutation: UPDATE_REVIEW_MUTATION,
      variables: { id, review }
    }).pipe(
      map(result => result.data!.updateReview)
    );
  }
}

// GraphQL Queries and Mutations
const SEARCH_PROPERTIES_QUERY = gql`
  query SearchProperties($location: String, $checkIn: DateTime, $checkOut: DateTime, $guests: Int, $minPrice: Decimal, $maxPrice: Decimal, $skip: Int, $take: Int) {
    searchProperties(location: $location, checkIn: $checkIn, checkOut: $checkOut, guests: $guests, minPrice: $minPrice, maxPrice: $maxPrice, skip: $skip, take: $take) {
      id
      title
      description
      propertyType
      pricePerNight
      maxGuests
      bedrooms
      bathrooms
      city
      state
      country
      images
      isInstantBook
      averageRating
      reviewCount
      host {
        id
        firstName
        lastName
        profilePictureUrl
      }
    }
  }
`;

const GET_PROPERTY_QUERY = gql`
  query GetProperty($id: ID!) {
    property(id: $id) {
      id
      hostId
      title
      description
      propertyType
      pricePerNight
      maxGuests
      bedrooms
      bathrooms
      address
      city
      state
      country
      postalCode
      latitude
      longitude
      amenities
      images
      houseRules
      isInstantBook
      minimumStay
      maximumStay
      weeklyDiscount
      monthlyDiscount
      status
      createdAt
      updatedAt
      unavailableDates
      averageRating
      reviewCount
      host {
        id
        firstName
        lastName
        email
        phone
        profilePictureUrl
        isEmailVerified
        isPhoneVerified
        createdAt
      }
      reviews {
        id
        type
        overallRating
        cleanlinessRating
        accuracyRating
        checkInRating
        communicationRating
        locationRating
        valueRating
        comment
        status
        createdAt
        publishedAt
        isAnonymous
        reviewer {
          firstName
          lastName
          profilePictureUrl
        }
      }
    }
  }
`;

const GET_PROPERTIES_BY_HOST_QUERY = gql`
  query GetPropertiesByHost($hostId: ID!, $skip: Int, $take: Int) {
    propertiesByHost(hostId: $hostId, skip: $skip, take: $take) {
      id
      title
      description
      propertyType
      pricePerNight
      maxGuests
      city
      state
      images
      status
      averageRating
      reviewCount
      createdAt
    }
  }
`;

const CHECK_PROPERTY_AVAILABILITY_QUERY = gql`
  query CheckPropertyAvailability($propertyId: ID!, $checkIn: DateTime!, $checkOut: DateTime!) {
    checkPropertyAvailability(propertyId: $propertyId, checkIn: $checkIn, checkOut: $checkOut)
  }
`;

const CREATE_PROPERTY_MUTATION = gql`
  mutation CreateProperty($property: PropertyInput!) {
    createProperty(property: $property) {
      id
      title
      description
      pricePerNight
      city
      state
      status
    }
  }
`;

const UPDATE_PROPERTY_MUTATION = gql`
  mutation UpdateProperty($id: ID!, $property: PropertyInput!) {
    updateProperty(id: $id, property: $property) {
      id
      title
      description
      pricePerNight
      city
      state
      status
      updatedAt
    }
  }
`;

const DELETE_PROPERTY_MUTATION = gql`
  mutation DeleteProperty($id: ID!) {
    deleteProperty(id: $id)
  }
`;

const GET_BOOKING_QUERY = gql`
  query GetBooking($id: ID!) {
    booking(id: $id) {
      id
      propertyId
      guestId
      hostId
      checkInDate
      checkOutDate
      numberOfGuests
      numberOfNights
      propertyPricePerNight
      subTotal
      cleaningFee
      serviceFee
      taxAmount
      totalAmount
      status
      specialRequests
      cancellationPolicy
      cancelledAt
      cancellationReason
      refundAmount
      createdAt
      updatedAt
      checkedInAt
      checkedOutAt
      property {
        id
        title
        address
        city
        state
        images
        host {
          firstName
          lastName
          phone
          email
        }
      }
      guest {
        id
        firstName
        lastName
        email
        phone
        profilePictureUrl
      }
      host {
        id
        firstName
        lastName
        email
        phone
        profilePictureUrl
      }
    }
  }
`;

const GET_BOOKINGS_BY_GUEST_QUERY = gql`
  query GetBookingsByGuest($guestId: ID!, $skip: Int, $take: Int) {
    bookingsByGuest(guestId: $guestId, skip: $skip, take: $take) {
      id
      checkInDate
      checkOutDate
      numberOfGuests
      totalAmount
      status
      createdAt
      property {
        id
        title
        city
        images
        host {
          firstName
          lastName
        }
      }
    }
  }
`;

const GET_BOOKINGS_BY_HOST_QUERY = gql`
  query GetBookingsByHost($hostId: ID!, $skip: Int, $take: Int) {
    bookingsByHost(hostId: $hostId, skip: $skip, take: $take) {
      id
      checkInDate
      checkOutDate
      numberOfGuests
      totalAmount
      status
      createdAt
      guest {
        id
        firstName
        lastName
        email
        phone
        profilePictureUrl
      }
      property {
        id
        title
      }
    }
  }
`;

const GET_BOOKINGS_BY_PROPERTY_QUERY = gql`
  query GetBookingsByProperty($propertyId: ID!, $skip: Int, $take: Int) {
    bookingsByProperty(propertyId: $propertyId, skip: $skip, take: $take) {
      id
      checkInDate
      checkOutDate
      numberOfGuests
      totalAmount
      status
      createdAt
      guest {
        firstName
        lastName
        email
        phone
      }
    }
  }
`;

const CREATE_BOOKING_MUTATION = gql`
  mutation CreateBooking($booking: BookingInput!) {
    createBooking(booking: $booking) {
      id
      checkInDate
      checkOutDate
      numberOfGuests
      totalAmount
      status
      property {
        title
      }
      guest {
        firstName
        lastName
      }
    }
  }
`;

const UPDATE_BOOKING_MUTATION = gql`
  mutation UpdateBooking($id: ID!, $booking: BookingInput!) {
    updateBooking(id: $id, booking: $booking) {
      id
      checkInDate
      checkOutDate
      numberOfGuests
      specialRequests
      updatedAt
    }
  }
`;

const UPDATE_BOOKING_STATUS_MUTATION = gql`
  mutation UpdateBookingStatus($id: ID!, $status: BookingStatus!) {
    updateBookingStatus(id: $id, status: $status) {
      id
      status
      updatedAt
    }
  }
`;

const CANCEL_BOOKING_MUTATION = gql`
  mutation CancelBooking($id: ID!, $reason: String!) {
    cancelBooking(id: $id, reason: $reason) {
      id
      status
      cancelledAt
      cancellationReason
      refundAmount
    }
  }
`;

const CHECK_IN_BOOKING_MUTATION = gql`
  mutation CheckInBooking($id: ID!) {
    checkInBooking(id: $id)
  }
`;

const CHECK_OUT_BOOKING_MUTATION = gql`
  mutation CheckOutBooking($id: ID!) {
    checkOutBooking(id: $id)
  }
`;

const GET_USER_PAYMENT_METHODS_QUERY = gql`
  query GetUserPaymentMethods($userId: ID!) {
    userPaymentMethods(userId: $userId) {
      id
      type
      last4Digits
      brand
      expiryMonth
      expiryYear
      isDefault
      isActive
      createdAt
    }
  }
`;

const CREATE_PAYMENT_METHOD_MUTATION = gql`
  mutation CreatePaymentMethod($paymentMethod: PaymentMethodInput!) {
    createPaymentMethod(paymentMethod: $paymentMethod) {
      id
      type
      last4Digits
      brand
      isDefault
      isActive
    }
  }
`;

const DELETE_PAYMENT_METHOD_MUTATION = gql`
  mutation DeletePaymentMethod($id: ID!) {
    deletePaymentMethod(id: $id)
  }
`;

const GET_REVIEWS_BY_PROPERTY_QUERY = gql`
  query GetReviewsByProperty($propertyId: ID!, $skip: Int, $take: Int) {
    reviewsByProperty(propertyId: $propertyId, skip: $skip, take: $take) {
      id
      type
      overallRating
      cleanlinessRating
      accuracyRating
      checkInRating
      communicationRating
      locationRating
      valueRating
      comment
      status
      createdAt
      publishedAt
      isAnonymous
      reviewer {
        firstName
        lastName
        profilePictureUrl
      }
    }
  }
`;

const CREATE_REVIEW_MUTATION = gql`
  mutation CreateReview($review: ReviewInput!) {
    createReview(review: $review) {
      id
      type
      overallRating
      comment
      status
      createdAt
    }
  }
`;

const UPDATE_REVIEW_MUTATION = gql`
  mutation UpdateReview($id: ID!, $review: ReviewInput!) {
    updateReview(id: $id, review: $review) {
      id
      overallRating
      comment
      updatedAt
    }
  }
`;