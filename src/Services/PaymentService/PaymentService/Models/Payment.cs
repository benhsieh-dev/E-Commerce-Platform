using System.ComponentModel.DataAnnotations;

namespace PaymentService.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid BookingId { get; set; }
        
        [Required]
        public Guid PayerId { get; set; } // Guest making the payment
        
        [Required]
        public Guid PayeeId { get; set; } // Host receiving the payment
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public string Currency { get; set; } = "USD";
        
        [Required]
        public PaymentType PaymentType { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; }
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        
        public string? ExternalTransactionId { get; set; } // Stripe, PayPal, etc.
        
        public string? PaymentMethodId { get; set; } // Stripe payment method ID
        
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public DateTime? ProcessedAt { get; set; }
        
        public DateTime? RefundedAt { get; set; }
        
        public string? FailureReason { get; set; }
        
        public decimal? RefundAmount { get; set; }
        
        public string? RefundReason { get; set; }
        
        public List<PaymentTransaction> Transactions { get; set; } = new();
        
        // Fee breakdown
        public decimal? PlatformFee { get; set; }
        public decimal? ProcessingFee { get; set; }
        public decimal? HostAmount { get; set; } // Amount to be paid to host after fees
        
        // Booking details (cached for performance)
        public string PropertyTitle { get; set; } = string.Empty;
        public DateTime BookingCheckIn { get; set; }
        public DateTime BookingCheckOut { get; set; }
    }

    public class PaymentTransaction
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid PaymentId { get; set; }
        
        [Required]
        public TransactionType Type { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public TransactionStatus Status { get; set; }
        
        public string? ExternalTransactionId { get; set; }
        
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? ProcessedAt { get; set; }
        
        public string? FailureReason { get; set; }
        
        public Payment Payment { get; set; } = null!;
    }

    public class PaymentMethod
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public PaymentMethodType Type { get; set; }
        
        public string? StripePaymentMethodId { get; set; }
        
        // Card details (masked for security)
        public string? CardLast4 { get; set; }
        public string? CardBrand { get; set; }
        public int? CardExpMonth { get; set; }
        public int? CardExpYear { get; set; }
        
        // Bank account details (masked)
        public string? BankLast4 { get; set; }
        public string? BankName { get; set; }
        
        public bool IsDefault { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }

    public enum PaymentType
    {
        BookingPayment = 0,     // Full payment for booking
        DepositPayment = 1,     // Security deposit
        PartialPayment = 2,     // Partial/installment payment
        HostPayout = 3,         // Payment to host
        Refund = 4,             // Refund to guest
        SecurityDeposit = 5     // Refundable security deposit
    }

    public enum PaymentStatus
    {
        Pending = 0,            // Payment initiated but not processed
        Processing = 1,         // Payment being processed
        Completed = 2,          // Payment successful
        Failed = 3,             // Payment failed
        Cancelled = 4,          // Payment cancelled
        Refunded = 5,           // Payment refunded
        PartiallyRefunded = 6,  // Partial refund issued
        Disputed = 7,           // Payment disputed/chargeback
        OnHold = 8              // Payment on hold for review
    }

    public enum PaymentMethodType
    {
        CreditCard = 0,
        DebitCard = 1,
        BankTransfer = 2,
        PayPal = 3,
        ApplePay = 4,
        GooglePay = 5,
        BankAccount = 6
    }

    public enum TransactionType
    {
        Charge = 0,             // Charging the customer
        Refund = 1,             // Refunding the customer
        Transfer = 2,           // Transferring to host
        Fee = 3,                // Platform fee collection
        Chargeback = 4,         // Chargeback from bank
        Dispute = 5             // Dispute resolution
    }

    public enum TransactionStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4
    }
}