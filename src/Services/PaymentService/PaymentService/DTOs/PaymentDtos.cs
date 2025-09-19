using System.ComponentModel.DataAnnotations;
using PaymentService.Models;

namespace PaymentService.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public Guid BookingId { get; set; }
        
        [Required]
        public Guid PayerId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [StringLength(3)]
        public string Currency { get; set; } = "USD";
        
        [Required]
        public PaymentType PaymentType { get; set; }
        
        [Required]
        public Guid PaymentMethodId { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class ProcessPaymentDto
    {
        [Required]
        public Guid PaymentId { get; set; }
        
        public string? PaymentIntentId { get; set; }
    }

    public class RefundPaymentDto
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid PayerId { get; set; }
        public Guid PayeeId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public PaymentType PaymentType { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public string? ExternalTransactionId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        public string? FailureReason { get; set; }
        public decimal? RefundAmount { get; set; }
        public string? RefundReason { get; set; }
        public decimal? PlatformFee { get; set; }
        public decimal? ProcessingFee { get; set; }
        public decimal? HostAmount { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public DateTime BookingCheckIn { get; set; }
        public DateTime BookingCheckOut { get; set; }
        public List<PaymentTransactionDto> Transactions { get; set; } = new();
    }

    public class PaymentTransactionDto
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public string? ExternalTransactionId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? FailureReason { get; set; }
    }

    public class PaymentListResponseDto
    {
        public List<PaymentResponseDto> Payments { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class CreatePaymentMethodDto
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public PaymentMethodType Type { get; set; }
        
        public string? StripePaymentMethodId { get; set; }
        
        public bool IsDefault { get; set; } = false;
    }

    public class PaymentMethodResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public PaymentMethodType Type { get; set; }
        public string? CardLast4 { get; set; }
        public string? CardBrand { get; set; }
        public int? CardExpMonth { get; set; }
        public int? CardExpYear { get; set; }
        public string? BankLast4 { get; set; }
        public string? BankName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PaymentSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalRefunds { get; set; }
        public decimal PlatformFees { get; set; }
        public decimal HostPayouts { get; set; }
        public int TotalTransactions { get; set; }
        public int SuccessfulPayments { get; set; }
        public int FailedPayments { get; set; }
        public int RefundCount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class PaymentIntentDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
    }

    public class HostPayoutDto
    {
        [Required]
        public Guid HostId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public List<Guid>? BookingIds { get; set; }
    }

    public class PaymentSearchDto
    {
        public Guid? BookingId { get; set; }
        public Guid? PayerId { get; set; }
        public Guid? PayeeId { get; set; }
        public PaymentStatus? Status { get; set; }
        public PaymentType? PaymentType { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}