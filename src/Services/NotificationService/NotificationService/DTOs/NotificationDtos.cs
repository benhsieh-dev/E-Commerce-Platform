using System.ComponentModel.DataAnnotations;
using NotificationService.Models;

namespace NotificationService.DTOs
{
    public class CreateNotificationDto
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public NotificationType Type { get; set; }
        
        [Required]
        public NotificationChannel Channel { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(4000)]
        public string Content { get; set; } = string.Empty;
        
        [StringLength(8000)]
        public string? HtmlContent { get; set; }
        
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
        
        [EmailAddress]
        public string? RecipientEmail { get; set; }
        
        [Phone]
        public string? RecipientPhone { get; set; }
        
        public string? RecipientPushToken { get; set; }
        
        public DateTime? ScheduledAt { get; set; }
        
        public Guid? BookingId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? ReviewId { get; set; }
        
        public Guid? TemplateId { get; set; }
        public Dictionary<string, object>? TemplateData { get; set; }
    }

    public class SendTemplateNotificationDto
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public Guid TemplateId { get; set; }
        
        [Required]
        public NotificationChannel Channel { get; set; }
        
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
        
        [EmailAddress]
        public string? RecipientEmail { get; set; }
        
        [Phone]
        public string? RecipientPhone { get; set; }
        
        public string? RecipientPushToken { get; set; }
        
        public DateTime? ScheduledAt { get; set; }
        
        [Required]
        public Dictionary<string, object> TemplateData { get; set; } = new();
        
        public Guid? BookingId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? ReviewId { get; set; }
    }

    public class NotificationResponseDto
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
        public List<NotificationDeliveryAttemptDto> DeliveryAttempts { get; set; } = new();
    }

    public class NotificationDeliveryAttemptDto
    {
        public Guid Id { get; set; }
        public NotificationChannel Channel { get; set; }
        public DeliveryStatus Status { get; set; }
        public DateTime AttemptedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ExternalId { get; set; }
    }

    public class NotificationListResponseDto
    {
        public List<NotificationResponseDto> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int UnreadCount { get; set; }
    }

    public class CreateTemplateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public NotificationType Type { get; set; }
        
        [Required]
        public NotificationChannel Channel { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        [StringLength(4000)]
        public string BodyTemplate { get; set; } = string.Empty;
        
        [StringLength(8000)]
        public string? HtmlTemplate { get; set; }
        
        [StringLength(1000)]
        public string? SmsTemplate { get; set; }
        
        [StringLength(1000)]
        public string? PushTemplate { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public string? TemplateVariables { get; set; }
    }

    public class UpdateTemplateDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
        
        [StringLength(200)]
        public string? Subject { get; set; }
        
        [StringLength(4000)]
        public string? BodyTemplate { get; set; }
        
        [StringLength(8000)]
        public string? HtmlTemplate { get; set; }
        
        [StringLength(1000)]
        public string? SmsTemplate { get; set; }
        
        [StringLength(1000)]
        public string? PushTemplate { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool? IsActive { get; set; }
        
        public string? TemplateVariables { get; set; }
    }

    public class NotificationTemplateResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string BodyTemplate { get; set; } = string.Empty;
        public string? HtmlTemplate { get; set; }
        public string? SmsTemplate { get; set; }
        public string? PushTemplate { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? TemplateVariables { get; set; }
    }

    public class NotificationPreferenceDto
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
    }

    public class UpdatePreferencesDto
    {
        [Required]
        public List<NotificationPreferenceUpdateDto> Preferences { get; set; } = new();
    }

    public class NotificationPreferenceUpdateDto
    {
        [Required]
        public NotificationType NotificationType { get; set; }
        
        public bool EmailEnabled { get; set; } = true;
        public bool SmsEnabled { get; set; } = false;
        public bool PushEnabled { get; set; } = true;
        public bool InAppEnabled { get; set; } = true;
    }

    public class NotificationStatsDto
    {
        public int TotalNotifications { get; set; }
        public int PendingNotifications { get; set; }
        public int SentNotifications { get; set; }
        public int DeliveredNotifications { get; set; }
        public int FailedNotifications { get; set; }
        public int UnreadNotifications { get; set; }
        public Dictionary<NotificationType, int> NotificationsByType { get; set; } = new();
        public Dictionary<NotificationChannel, int> NotificationsByChannel { get; set; } = new();
        public Dictionary<NotificationStatus, int> NotificationsByStatus { get; set; } = new();
        public double DeliveryRate { get; set; }
        public double OpenRate { get; set; }
    }

    public class BulkNotificationDto
    {
        [Required]
        public List<Guid> UserIds { get; set; } = new();
        
        [Required]
        public Guid TemplateId { get; set; }
        
        [Required]
        public NotificationChannel Channel { get; set; }
        
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
        
        public DateTime? ScheduledAt { get; set; }
        
        [Required]
        public Dictionary<string, object> TemplateData { get; set; } = new();
    }

    public class MarkAsReadDto
    {
        [Required]
        public List<Guid> NotificationIds { get; set; } = new();
    }
}