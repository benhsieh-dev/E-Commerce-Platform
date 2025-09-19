using NotificationService.Models;

namespace NotificationService.Services
{
    public interface INotificationDeliveryService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, string? htmlBody = null);
        Task<bool> SendSmsAsync(string to, string message);
        Task<bool> SendPushNotificationAsync(string token, string title, string message, Dictionary<string, object>? data = null);
        Task<bool> SendInAppNotificationAsync(Guid userId, string title, string content);
    }

    public class EmailDeliveryService : IEmailDeliveryService
    {
        private readonly ILogger<EmailDeliveryService> _logger;
        private readonly IConfiguration _configuration;

        public EmailDeliveryService(ILogger<EmailDeliveryService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, string? htmlBody = null)
        {
            try
            {
                // In a real implementation, this would use SendGrid, AWS SES, etc.
                await Task.Delay(100); // Simulate API call
                
                _logger.LogInformation("Email sent to {To} with subject {Subject}", to, subject);
                
                // Simulate 95% success rate
                var success = Random.Shared.Next(1, 101) <= 95;
                
                if (!success)
                {
                    _logger.LogWarning("Email delivery failed for {To}", to);
                }
                
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", to);
                return false;
            }
        }
    }

    public class SmsDeliveryService : ISmsDeliveryService
    {
        private readonly ILogger<SmsDeliveryService> _logger;
        private readonly IConfiguration _configuration;

        public SmsDeliveryService(ILogger<SmsDeliveryService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendSmsAsync(string to, string message)
        {
            try
            {
                // In a real implementation, this would use Twilio, AWS SNS, etc.
                await Task.Delay(200); // Simulate API call
                
                _logger.LogInformation("SMS sent to {To}: {Message}", to, message[..Math.Min(50, message.Length)]);
                
                // Simulate 90% success rate (SMS typically has lower delivery rates)
                var success = Random.Shared.Next(1, 101) <= 90;
                
                if (!success)
                {
                    _logger.LogWarning("SMS delivery failed for {To}", to);
                }
                
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS to {To}", to);
                return false;
            }
        }
    }

    public class PushNotificationService : IPushNotificationService
    {
        private readonly ILogger<PushNotificationService> _logger;
        private readonly IConfiguration _configuration;

        public PushNotificationService(ILogger<PushNotificationService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendPushNotificationAsync(string token, string title, string message, Dictionary<string, object>? data = null)
        {
            try
            {
                // In a real implementation, this would use Firebase FCM, Apple APNS, etc.
                await Task.Delay(150); // Simulate API call
                
                _logger.LogInformation("Push notification sent to token {Token}: {Title}", 
                    token[..Math.Min(10, token.Length)] + "...", title);
                
                // Simulate 85% success rate (push notifications can fail due to uninstalled apps, etc.)
                var success = Random.Shared.Next(1, 101) <= 85;
                
                if (!success)
                {
                    _logger.LogWarning("Push notification delivery failed for token {Token}", token);
                }
                
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending push notification to token {Token}", token);
                return false;
            }
        }
    }

    public class InAppNotificationService : IInAppNotificationService
    {
        private readonly ILogger<InAppNotificationService> _logger;

        public InAppNotificationService(ILogger<InAppNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendInAppNotificationAsync(Guid userId, string title, string content)
        {
            try
            {
                // In a real implementation, this would use SignalR, WebSockets, etc.
                await Task.Delay(50); // Simulate WebSocket send
                
                _logger.LogInformation("In-app notification sent to user {UserId}: {Title}", userId, title);
                
                // In-app notifications typically have very high success rates
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending in-app notification to user {UserId}", userId);
                return false;
            }
        }
    }

    // Composite service that handles all delivery channels
    public class NotificationDeliveryService : INotificationDeliveryService
    {
        private readonly IEmailDeliveryService _emailService;
        private readonly ISmsDeliveryService _smsService;
        private readonly IPushNotificationService _pushService;
        private readonly IInAppNotificationService _inAppService;
        private readonly ILogger<NotificationDeliveryService> _logger;

        public NotificationDeliveryService(
            IEmailDeliveryService emailService,
            ISmsDeliveryService smsService,
            IPushNotificationService pushService,
            IInAppNotificationService inAppService,
            ILogger<NotificationDeliveryService> logger)
        {
            _emailService = emailService;
            _smsService = smsService;
            _pushService = pushService;
            _inAppService = inAppService;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, string? htmlBody = null)
        {
            return await _emailService.SendEmailAsync(to, subject, body, htmlBody);
        }

        public async Task<bool> SendSmsAsync(string to, string message)
        {
            return await _smsService.SendSmsAsync(to, message);
        }

        public async Task<bool> SendPushNotificationAsync(string token, string title, string message, Dictionary<string, object>? data = null)
        {
            return await _pushService.SendPushNotificationAsync(token, title, message, data);
        }

        public async Task<bool> SendInAppNotificationAsync(Guid userId, string title, string content)
        {
            return await _inAppService.SendInAppNotificationAsync(userId, title, content);
        }
    }

    // Individual service interfaces for dependency injection
    public interface IEmailDeliveryService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, string? htmlBody = null);
    }

    public interface ISmsDeliveryService
    {
        Task<bool> SendSmsAsync(string to, string message);
    }

    public interface IPushNotificationService
    {
        Task<bool> SendPushNotificationAsync(string token, string title, string message, Dictionary<string, object>? data = null);
    }

    public interface IInAppNotificationService
    {
        Task<bool> SendInAppNotificationAsync(Guid userId, string title, string content);
    }
}