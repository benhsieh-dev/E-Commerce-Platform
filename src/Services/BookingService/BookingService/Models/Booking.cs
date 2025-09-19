using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid PropertyId { get; set; }
        
        [Required]
        public Guid GuestId { get; set; }
        
        [Required]
        public Guid HostId { get; set; }
        
        [Required]
        public DateTime CheckInDate { get; set; }
        
        [Required]
        public DateTime CheckOutDate { get; set; }
        
        [Required]
        [Range(1, 50)]
        public int NumberOfGuests { get; set; }
        
        [Required]
        public decimal TotalPrice { get; set; }
        
        public decimal? ServiceFee { get; set; }
        
        public decimal? CleaningFee { get; set; }
        
        public decimal? TaxAmount { get; set; }
        
        public decimal FinalAmount => TotalPrice + (ServiceFee ?? 0) + (CleaningFee ?? 0) + (TaxAmount ?? 0);
        
        [Required]
        public BookingStatus Status { get; set; }
        
        public PaymentStatus PaymentStatus { get; set; }
        
        public string? SpecialRequests { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public DateTime? ConfirmedAt { get; set; }
        
        public DateTime? CancelledAt { get; set; }
        
        public string? CancellationReason { get; set; }
        
        public CancellationPolicy CancellationPolicy { get; set; }
        
        public List<BookingStatusHistory> StatusHistory { get; set; } = new();
        
        public int Nights => (CheckOutDate.Date - CheckInDate.Date).Days;
        
        // Property details (cached for performance)
        public string PropertyTitle { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public string PropertyCity { get; set; } = string.Empty;
        public string PropertyCountry { get; set; } = string.Empty;
    }

    public class BookingStatusHistory
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid BookingId { get; set; }
        
        [Required]
        public BookingStatus FromStatus { get; set; }
        
        [Required]
        public BookingStatus ToStatus { get; set; }
        
        public string? Reason { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public Booking Booking { get; set; } = null!;
    }

    public enum BookingStatus
    {
        Pending = 0,        // Waiting for host confirmation
        Confirmed = 1,      // Host confirmed the booking
        CheckedIn = 2,      // Guest has checked in
        CheckedOut = 3,     // Guest has checked out
        Completed = 4,      // Booking completed successfully
        Cancelled = 5,      // Booking cancelled
        Declined = 6        // Host declined the booking
    }

    public enum PaymentStatus
    {
        Pending = 0,        // Payment not processed
        PartialPaid = 1,    // Deposit/partial payment made
        Paid = 2,           // Full payment completed
        Refunded = 3,       // Payment refunded
        PartialRefund = 4   // Partial refund issued
    }

    public enum CancellationPolicy
    {
        Flexible = 0,       // Full refund 1 day before
        Moderate = 1,       // Full refund 5 days before
        Strict = 2,         // 50% refund up to 1 week before
        SuperStrict = 3     // No refund after booking
    }
}