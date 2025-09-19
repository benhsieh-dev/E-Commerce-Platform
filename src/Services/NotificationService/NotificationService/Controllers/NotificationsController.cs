using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.DTOs;
using NotificationService.Models;
using NotificationService.Services;
using System.Text.Json;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationDbContext _context;
        private readonly INotificationDeliveryService _deliveryService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            NotificationDbContext context,
            INotificationDeliveryService deliveryService,
            ILogger<NotificationsController> logger)
        {
            _context = context;
            _deliveryService = deliveryService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<NotificationResponseDto>> CreateNotification(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Type = dto.Type,
                Channel = dto.Channel,
                Title = dto.Title,
                Content = dto.Content,
                HtmlContent = dto.HtmlContent,
                Priority = dto.Priority,
                RecipientEmail = dto.RecipientEmail,
                RecipientPhone = dto.RecipientPhone,
                RecipientPushToken = dto.RecipientPushToken,
                ScheduledAt = dto.ScheduledAt,
                BookingId = dto.BookingId,
                PropertyId = dto.PropertyId,
                PaymentId = dto.PaymentId,
                ReviewId = dto.ReviewId,
                TemplateId = dto.TemplateId,
                TemplateData = dto.TemplateData != null ? JsonSerializer.Serialize(dto.TemplateData) : null,
                Status = dto.ScheduledAt.HasValue && dto.ScheduledAt > DateTime.UtcNow 
                    ? NotificationStatus.Scheduled 
                    : NotificationStatus.Pending,
                TrackingId = Guid.NewGuid().ToString("N")[..8],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // If not scheduled, send immediately
            if (!dto.ScheduledAt.HasValue || dto.ScheduledAt <= DateTime.UtcNow)
            {
                await ProcessNotificationDelivery(notification);
            }

            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, 
                MapToResponseDto(notification));
        }

        [HttpPost("template")]
        public async Task<ActionResult<NotificationResponseDto>> SendTemplateNotification(SendTemplateNotificationDto dto)
        {
            var template = await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Id == dto.TemplateId && t.IsActive);

            if (template == null)
            {
                return NotFound("Template not found or inactive");
            }

            var processedContent = ProcessTemplate(template.BodyTemplate, dto.TemplateData);
            var processedSubject = ProcessTemplate(template.Subject, dto.TemplateData);
            var processedHtml = template.HtmlTemplate != null 
                ? ProcessTemplate(template.HtmlTemplate, dto.TemplateData) 
                : null;

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Type = template.Type,
                Channel = dto.Channel,
                Title = processedSubject,
                Content = processedContent,
                HtmlContent = processedHtml,
                Priority = dto.Priority,
                RecipientEmail = dto.RecipientEmail,
                RecipientPhone = dto.RecipientPhone,
                RecipientPushToken = dto.RecipientPushToken,
                ScheduledAt = dto.ScheduledAt,
                BookingId = dto.BookingId,
                PropertyId = dto.PropertyId,
                PaymentId = dto.PaymentId,
                ReviewId = dto.ReviewId,
                TemplateId = dto.TemplateId,
                TemplateData = JsonSerializer.Serialize(dto.TemplateData),
                Status = dto.ScheduledAt.HasValue && dto.ScheduledAt > DateTime.UtcNow 
                    ? NotificationStatus.Scheduled 
                    : NotificationStatus.Pending,
                TrackingId = Guid.NewGuid().ToString("N")[..8],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            if (!dto.ScheduledAt.HasValue || dto.ScheduledAt <= DateTime.UtcNow)
            {
                await ProcessNotificationDelivery(notification);
            }

            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, 
                MapToResponseDto(notification));
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<List<NotificationResponseDto>>> SendBulkNotifications(BulkNotificationDto dto)
        {
            var template = await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Id == dto.TemplateId && t.IsActive);

            if (template == null)
            {
                return NotFound("Template not found or inactive");
            }

            var notifications = new List<Notification>();

            foreach (var userId in dto.UserIds)
            {
                var processedContent = ProcessTemplate(template.BodyTemplate, dto.TemplateData);
                var processedSubject = ProcessTemplate(template.Subject, dto.TemplateData);

                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = template.Type,
                    Channel = dto.Channel,
                    Title = processedSubject,
                    Content = processedContent,
                    Priority = dto.Priority,
                    ScheduledAt = dto.ScheduledAt,
                    TemplateId = dto.TemplateId,
                    TemplateData = JsonSerializer.Serialize(dto.TemplateData),
                    Status = dto.ScheduledAt.HasValue && dto.ScheduledAt > DateTime.UtcNow 
                        ? NotificationStatus.Scheduled 
                        : NotificationStatus.Pending,
                    TrackingId = Guid.NewGuid().ToString("N")[..8],
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                notifications.Add(notification);
            }

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            // Process immediate notifications
            var immediateNotifications = notifications
                .Where(n => !n.ScheduledAt.HasValue || n.ScheduledAt <= DateTime.UtcNow);

            foreach (var notification in immediateNotifications)
            {
                _ = Task.Run(() => ProcessNotificationDelivery(notification));
            }

            return Ok(notifications.Select(MapToResponseDto).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationResponseDto>> GetNotification(Guid id)
        {
            var notification = await _context.Notifications
                .Include(n => n.DeliveryAttempts)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            return MapToResponseDto(notification);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<NotificationListResponseDto>> GetUserNotifications(
            Guid userId, 
            int page = 1, 
            int pageSize = 20,
            NotificationChannel? channel = null,
            NotificationStatus? status = null,
            bool unreadOnly = false)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId);

            if (channel.HasValue)
                query = query.Where(n => n.Channel == channel);

            if (status.HasValue)
                query = query.Where(n => n.Status == status);

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            var totalCount = await query.CountAsync();
            var unreadCount = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            var notifications = await query
                .Include(n => n.DeliveryAttempts)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new NotificationListResponseDto
            {
                Notifications = notifications.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                UnreadCount = unreadCount
            };
        }

        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkNotificationsAsRead(MarkAsReadDto dto)
        {
            var notifications = await _context.Notifications
                .Where(n => dto.NotificationIds.Contains(n.Id))
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/retry")]
        public async Task<ActionResult<NotificationResponseDto>> RetryNotification(Guid id)
        {
            var notification = await _context.Notifications
                .Include(n => n.DeliveryAttempts)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            if (notification.Status == NotificationStatus.Delivered)
            {
                return BadRequest("Notification already delivered");
            }

            if (notification.RetryCount >= notification.MaxRetries)
            {
                return BadRequest("Maximum retry attempts exceeded");
            }

            notification.Status = NotificationStatus.Pending;
            notification.ErrorMessage = null;
            notification.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await ProcessNotificationDelivery(notification);

            return MapToResponseDto(notification);
        }

        [HttpGet("stats")]
        public async Task<ActionResult<NotificationStatsDto>> GetNotificationStats(
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            fromDate ??= DateTime.UtcNow.AddDays(-30);
            toDate ??= DateTime.UtcNow;

            var query = _context.Notifications
                .Where(n => n.CreatedAt >= fromDate && n.CreatedAt <= toDate);

            var totalNotifications = await query.CountAsync();
            var pendingNotifications = await query.CountAsync(n => n.Status == NotificationStatus.Pending);
            var sentNotifications = await query.CountAsync(n => n.Status == NotificationStatus.Sent);
            var deliveredNotifications = await query.CountAsync(n => n.Status == NotificationStatus.Delivered);
            var failedNotifications = await query.CountAsync(n => n.Status == NotificationStatus.Failed);
            var unreadNotifications = await query.CountAsync(n => !n.IsRead);

            var notificationsByType = await query
                .GroupBy(n => n.Type)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            var notificationsByChannel = await query
                .GroupBy(n => n.Channel)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            var notificationsByStatus = await query
                .GroupBy(n => n.Status)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            var deliveryRate = totalNotifications > 0 
                ? (double)deliveredNotifications / totalNotifications * 100 
                : 0;

            var openRate = deliveredNotifications > 0 
                ? (double)await query.CountAsync(n => n.ReadAt.HasValue) / deliveredNotifications * 100 
                : 0;

            return new NotificationStatsDto
            {
                TotalNotifications = totalNotifications,
                PendingNotifications = pendingNotifications,
                SentNotifications = sentNotifications,
                DeliveredNotifications = deliveredNotifications,
                FailedNotifications = failedNotifications,
                UnreadNotifications = unreadNotifications,
                NotificationsByType = notificationsByType,
                NotificationsByChannel = notificationsByChannel,
                NotificationsByStatus = notificationsByStatus,
                DeliveryRate = deliveryRate,
                OpenRate = openRate
            };
        }

        private async Task ProcessNotificationDelivery(Notification notification)
        {
            try
            {
                notification.Status = NotificationStatus.Pending;
                await _context.SaveChangesAsync();

                var attempt = new NotificationDeliveryAttempt
                {
                    Id = Guid.NewGuid(),
                    NotificationId = notification.Id,
                    Channel = notification.Channel,
                    Status = DeliveryStatus.Pending,
                    AttemptedAt = DateTime.UtcNow
                };

                bool success = false;

                switch (notification.Channel)
                {
                    case NotificationChannel.Email:
                        if (!string.IsNullOrEmpty(notification.RecipientEmail))
                        {
                            success = await _deliveryService.SendEmailAsync(
                                notification.RecipientEmail,
                                notification.Title,
                                notification.Content,
                                notification.HtmlContent);
                        }
                        break;

                    case NotificationChannel.SMS:
                        if (!string.IsNullOrEmpty(notification.RecipientPhone))
                        {
                            success = await _deliveryService.SendSmsAsync(
                                notification.RecipientPhone,
                                notification.Content);
                        }
                        break;

                    case NotificationChannel.Push:
                        if (!string.IsNullOrEmpty(notification.RecipientPushToken))
                        {
                            success = await _deliveryService.SendPushNotificationAsync(
                                notification.RecipientPushToken,
                                notification.Title,
                                notification.Content);
                        }
                        break;

                    case NotificationChannel.InApp:
                        success = await _deliveryService.SendInAppNotificationAsync(
                            notification.UserId,
                            notification.Title,
                            notification.Content);
                        break;
                }

                if (success)
                {
                    notification.Status = NotificationStatus.Sent;
                    notification.SentAt = DateTime.UtcNow;
                    attempt.Status = DeliveryStatus.Sent;
                    attempt.DeliveredAt = DateTime.UtcNow;
                }
                else
                {
                    notification.RetryCount++;
                    if (notification.RetryCount >= notification.MaxRetries)
                    {
                        notification.Status = NotificationStatus.Failed;
                    }
                    attempt.Status = DeliveryStatus.Failed;
                    attempt.ErrorMessage = "Delivery failed";
                }

                notification.UpdatedAt = DateTime.UtcNow;
                _context.NotificationDeliveryAttempts.Add(attempt);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notification delivery for {NotificationId}", notification.Id);
                notification.Status = NotificationStatus.Failed;
                notification.ErrorMessage = ex.Message;
                notification.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private string ProcessTemplate(string template, Dictionary<string, object> data)
        {
            var result = template;
            foreach (var kvp in data)
            {
                result = result.Replace($"{{{{{kvp.Key}}}}}", kvp.Value?.ToString() ?? "");
            }
            return result;
        }

        private NotificationResponseDto MapToResponseDto(Notification notification)
        {
            return new NotificationResponseDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Type = notification.Type,
                Channel = notification.Channel,
                Title = notification.Title,
                Content = notification.Content,
                HtmlContent = notification.HtmlContent,
                Status = notification.Status,
                Priority = notification.Priority,
                RecipientEmail = notification.RecipientEmail,
                RecipientPhone = notification.RecipientPhone,
                ScheduledAt = notification.ScheduledAt,
                SentAt = notification.SentAt,
                DeliveredAt = notification.DeliveredAt,
                ReadAt = notification.ReadAt,
                CreatedAt = notification.CreatedAt,
                UpdatedAt = notification.UpdatedAt,
                ErrorMessage = notification.ErrorMessage,
                RetryCount = notification.RetryCount,
                BookingId = notification.BookingId,
                PropertyId = notification.PropertyId,
                PaymentId = notification.PaymentId,
                ReviewId = notification.ReviewId,
                TrackingId = notification.TrackingId,
                IsRead = notification.IsRead,
                IsArchived = notification.IsArchived,
                DeliveryAttempts = notification.DeliveryAttempts.Select(da => new NotificationDeliveryAttemptDto
                {
                    Id = da.Id,
                    Channel = da.Channel,
                    Status = da.Status,
                    AttemptedAt = da.AttemptedAt,
                    DeliveredAt = da.DeliveredAt,
                    ErrorMessage = da.ErrorMessage,
                    ExternalId = da.ExternalId
                }).ToList()
            };
        }
    }
}