using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public NotificationType Type { get; set; }
        
        [Required]
        public NotificationChannel Channel { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public string? HtmlContent { get; set; }
        
        [Required]
        public NotificationStatus Status { get; set; }
        
        [Required]
        public NotificationPriority Priority { get; set; }
        
        public string? RecipientEmail { get; set; }
        
        public string? RecipientPhone { get; set; }
        
        public string? RecipientPushToken { get; set; }
        
        public DateTime? ScheduledAt { get; set; }
        
        public DateTime? SentAt { get; set; }
        
        public DateTime? DeliveredAt { get; set; }
        
        public DateTime? ReadAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string? ErrorMessage { get; set; }
        
        public int RetryCount { get; set; } = 0;
        
        public int MaxRetries { get; set; } = 3;
        
        // Related entity information
        public Guid? BookingId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? ReviewId { get; set; }
        
        // Template and personalization
        public Guid? TemplateId { get; set; }
        public string? TemplateData { get; set; } // JSON data for template rendering
        
        // Tracking and analytics
        public string? TrackingId { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        
        public List<NotificationDeliveryAttempt> DeliveryAttempts { get; set; } = new();
    }

    public class NotificationDeliveryAttempt
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid NotificationId { get; set; }
        
        [Required]
        public NotificationChannel Channel { get; set; }
        
        [Required]
        public DeliveryStatus Status { get; set; }
        
        public DateTime AttemptedAt { get; set; }
        
        public DateTime? DeliveredAt { get; set; }
        
        public string? ErrorMessage { get; set; }
        
        public string? ExternalId { get; set; } // External service tracking ID
        
        public string? Response { get; set; } // Response from external service
        
        public Notification Notification { get; set; } = null!;
    }

    public class NotificationTemplate
    {
        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public NotificationType Type { get; set; }
        
        [Required]
        public NotificationChannel Channel { get; set; }
        
        [Required]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        public string BodyTemplate { get; set; } = string.Empty;
        
        public string? HtmlTemplate { get; set; }
        
        public string? SmsTemplate { get; set; }
        
        public string? PushTemplate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Template variables documentation
        public string? TemplateVariables { get; set; } // JSON schema
    }

    public class NotificationPreference
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public NotificationType NotificationType { get; set; }
        
        public bool EmailEnabled { get; set; } = true;
        
        public bool SmsEnabled { get; set; } = false;
        
        public bool PushEnabled { get; set; } = true;
        
        public bool InAppEnabled { get; set; } = true;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
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

    public enum DeliveryStatus
    {
        Pending = 0,
        Sent = 1,
        Delivered = 2,
        Failed = 3,
        Bounced = 4,
        Opened = 5,
        Clicked = 6
    }
}