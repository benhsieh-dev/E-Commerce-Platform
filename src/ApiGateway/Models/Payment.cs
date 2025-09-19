using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid PayerId { get; set; }
        public Guid? PayeeId { get; set; }
        public PaymentType Type { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal ProcessingFee { get; set; }
        public decimal NetAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public PaymentStatus Status { get; set; }
        public string? ExternalTransactionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? FailureReason { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public Booking Booking { get; set; } = null!;
        public User Payer { get; set; } = null!;
        public User? Payee { get; set; }
    }

    public class PaymentMethod
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public PaymentMethodType Type { get; set; }
        public string Last4Digits { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountType { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public string ExternalMethodId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public User User { get; set; } = null!;
    }

    public enum PaymentType
    {
        BookingPayment = 0,
        DepositPayment = 1,
        HostPayout = 2,
        Refund = 3,
        ServiceFee = 4
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
        Refunded = 5,
        PartiallyRefunded = 6
    }

    public enum PaymentMethodType
    {
        CreditCard = 0,
        DebitCard = 1,
        BankAccount = 2,
        PayPal = 3,
        ApplePay = 4,
        GooglePay = 5
    }
}