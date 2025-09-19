using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? HtmlContent { get; set; }
        public NotificationStatus Status { get; set; }
        public NotificationPriority Priority { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientPhone { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? ReviewId { get; set; }
        public string? TrackingId { get; set; }
        public bool IsRead { get; set; }
        public bool IsArchived { get; set; }
        
        // Navigation properties
        public User User { get; set; } = null!;
        public Booking? Booking { get; set; }
        public Property? Property { get; set; }
    }

    public class NotificationPreference
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public NotificationType NotificationType { get; set; }
        public bool EmailEnabled { get; set; }
        public bool SmsEnabled { get; set; }
        public bool PushEnabled { get; set; }
        public bool InAppEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public User User { get; set; } = null!;
    }

    public enum NotificationType
    {
        BookingConfirmation = 0,
        BookingReminder = 1,
        BookingCancellation = 2,
        PaymentConfirmation = 3,
        PaymentFailed = 4,
        PaymentRefund = 5,
        ReviewRequest = 6,
        ReviewReceived = 7,
        PropertyListed = 8,
        PropertyApproved = 9,
        PropertyRejected = 10,
        HostPayout = 11,
        WelcomeMessage = 12,
        PasswordReset = 13,
        AccountVerification = 14,
        SecurityAlert = 15,
        MarketingPromo = 16,
        SystemMaintenance = 17
    }

    public enum NotificationChannel
    {
        Email = 0,
        SMS = 1,
        Push = 2,
        InApp = 3,
        WebSocket = 4
    }

    public enum NotificationStatus
    {
        Draft = 0,
        Scheduled = 1,
        Pending = 2,
        Sent = 3,
        Delivered = 4,
        Failed = 5,
        Cancelled = 6
    }

    public enum NotificationPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
    }
}