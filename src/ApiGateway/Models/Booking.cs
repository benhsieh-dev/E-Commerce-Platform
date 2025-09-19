using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid GuestId { get; set; }
        public Guid HostId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfNights { get; set; }
        public decimal PropertyPricePerNight { get; set; }
        public decimal SubTotal { get; set; }
        public decimal CleaningFee { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public string? SpecialRequests { get; set; }
        public CancellationPolicy CancellationPolicy { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? CheckedInAt { get; set; }
        public DateTime? CheckedOutAt { get; set; }
        
        // Navigation properties
        public Property Property { get; set; } = null!;
        public User Guest { get; set; } = null!;
        public User Host { get; set; } = null!;
        public List<Payment> Payments { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }

    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        CheckedIn = 2,
        Completed = 3,
        Cancelled = 4,
        Expired = 5
    }

    public enum CancellationPolicy
    {
        Flexible = 0,
        Moderate = 1,
        Strict = 2,
        SuperStrict = 3
    }
}