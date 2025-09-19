using System.ComponentModel.DataAnnotations;
using BookingService.Models;

namespace BookingService.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public Guid PropertyId { get; set; }
        
        [Required]
        public Guid GuestId { get; set; }
        
        [Required]
        public DateTime CheckInDate { get; set; }
        
        [Required]
        public DateTime CheckOutDate { get; set; }
        
        [Required]
        [Range(1, 50)]
        public int NumberOfGuests { get; set; }
        
        [StringLength(1000)]
        public string? SpecialRequests { get; set; }
        
        public CancellationPolicy CancellationPolicy { get; set; } = CancellationPolicy.Moderate;
    }

    public class UpdateBookingDto
    {
        public DateTime? CheckInDate { get; set; }
        
        public DateTime? CheckOutDate { get; set; }
        
        [Range(1, 50)]
        public int? NumberOfGuests { get; set; }
        
        [StringLength(1000)]
        public string? SpecialRequests { get; set; }
    }

    public class BookingResponseDto
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid GuestId { get; set; }
        public Guid HostId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? CleaningFee { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public CancellationPolicy CancellationPolicy { get; set; }
        public int Nights { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public string PropertyCity { get; set; } = string.Empty;
        public string PropertyCountry { get; set; } = string.Empty;
        public List<BookingStatusHistoryDto> StatusHistory { get; set; } = new();
    }

    public class BookingStatusHistoryDto
    {
        public Guid Id { get; set; }
        public BookingStatus FromStatus { get; set; }
        public BookingStatus ToStatus { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BookingListResponseDto
    {
        public List<BookingResponseDto> Bookings { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class ConfirmBookingDto
    {
        [StringLength(500)]
        public string? Message { get; set; }
    }

    public class CancelBookingDto
    {
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    public class CheckInDto
    {
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class CheckOutDto
    {
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class BookingSearchDto
    {
        public Guid? PropertyId { get; set; }
        public Guid? GuestId { get; set; }
        public Guid? HostId { get; set; }
        public BookingStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime? CheckInFrom { get; set; }
        public DateTime? CheckInTo { get; set; }
        public DateTime? CheckOutFrom { get; set; }
        public DateTime? CheckOutTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class BookingPriceCalculationDto
    {
        public Guid PropertyId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
    }

    public class BookingPriceResponseDto
    {
        public decimal BasePrice { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? CleaningFee { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public int Nights { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal? DiscountApplied { get; set; }
        public string? DiscountReason { get; set; }
    }
}