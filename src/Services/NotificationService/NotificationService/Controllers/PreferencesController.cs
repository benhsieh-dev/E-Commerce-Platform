using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.DTOs;
using NotificationService.Models;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly NotificationDbContext _context;
        private readonly ILogger<PreferencesController> _logger;

        public PreferencesController(NotificationDbContext context, ILogger<PreferencesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<NotificationPreferenceDto>>> GetUserPreferences(Guid userId)
        {
            var preferences = await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.NotificationType)
                .ToListAsync();

            // If no preferences exist, create default ones
            if (!preferences.Any())
            {
                preferences = await CreateDefaultPreferences(userId);
            }

            return preferences.Select(MapToDto).ToList();
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<List<NotificationPreferenceDto>>> UpdateUserPreferences(
            Guid userId, 
            UpdatePreferencesDto dto)
        {
            var existingPreferences = await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var updatedPreferences = new List<NotificationPreference>();

            foreach (var preferenceUpdate in dto.Preferences)
            {
                var existing = existingPreferences
                    .FirstOrDefault(p => p.NotificationType == preferenceUpdate.NotificationType);

                if (existing != null)
                {
                    // Update existing preference
                    existing.EmailEnabled = preferenceUpdate.EmailEnabled;
                    existing.SmsEnabled = preferenceUpdate.SmsEnabled;
                    existing.PushEnabled = preferenceUpdate.PushEnabled;
                    existing.InAppEnabled = preferenceUpdate.InAppEnabled;
                    existing.UpdatedAt = DateTime.UtcNow;
                    updatedPreferences.Add(existing);
                }
                else
                {
                    // Create new preference
                    var newPreference = new NotificationPreference
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        NotificationType = preferenceUpdate.NotificationType,
                        EmailEnabled = preferenceUpdate.EmailEnabled,
                        SmsEnabled = preferenceUpdate.SmsEnabled,
                        PushEnabled = preferenceUpdate.PushEnabled,
                        InAppEnabled = preferenceUpdate.InAppEnabled,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.NotificationPreferences.Add(newPreference);
                    updatedPreferences.Add(newPreference);
                }
            }

            await _context.SaveChangesAsync();

            return updatedPreferences.Select(MapToDto).ToList();
        }

        [HttpPut("{userId}/type/{notificationType}")]
        public async Task<ActionResult<NotificationPreferenceDto>> UpdateTypePreference(
            Guid userId, 
            NotificationType notificationType,
            NotificationPreferenceUpdateDto dto)
        {
            var preference = await _context.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId && p.NotificationType == notificationType);

            if (preference == null)
            {
                // Create new preference
                preference = new NotificationPreference
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    NotificationType = notificationType,
                    EmailEnabled = dto.EmailEnabled,
                    SmsEnabled = dto.SmsEnabled,
                    PushEnabled = dto.PushEnabled,
                    InAppEnabled = dto.InAppEnabled,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.NotificationPreferences.Add(preference);
            }
            else
            {
                // Update existing preference
                preference.EmailEnabled = dto.EmailEnabled;
                preference.SmsEnabled = dto.SmsEnabled;
                preference.PushEnabled = dto.PushEnabled;
                preference.InAppEnabled = dto.InAppEnabled;
                preference.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return MapToDto(preference);
        }

        [HttpPost("{userId}/reset")]
        public async Task<ActionResult<List<NotificationPreferenceDto>>> ResetToDefaults(Guid userId)
        {
            // Remove existing preferences
            var existingPreferences = await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync();

            _context.NotificationPreferences.RemoveRange(existingPreferences);
            await _context.SaveChangesAsync();

            // Create default preferences
            var defaultPreferences = await CreateDefaultPreferences(userId);

            return defaultPreferences.Select(MapToDto).ToList();
        }

        [HttpGet("{userId}/channel/{channel}")]
        public async Task<ActionResult<List<NotificationPreferenceDto>>> GetChannelPreferences(
            Guid userId, 
            NotificationChannel channel)
        {
            var preferences = await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (!preferences.Any())
            {
                preferences = await CreateDefaultPreferences(userId);
            }

            // Filter preferences that have the specified channel enabled
            var filteredPreferences = preferences.Where(p =>
            {
                return channel switch
                {
                    NotificationChannel.Email => p.EmailEnabled,
                    NotificationChannel.SMS => p.SmsEnabled,
                    NotificationChannel.Push => p.PushEnabled,
                    NotificationChannel.InApp => p.InAppEnabled,
                    _ => false
                };
            }).ToList();

            return filteredPreferences.Select(MapToDto).ToList();
        }

        [HttpPost("{userId}/disable-all")]
        public async Task<IActionResult> DisableAllNotifications(Guid userId)
        {
            var preferences = await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync();

            foreach (var preference in preferences)
            {
                preference.EmailEnabled = false;
                preference.SmsEnabled = false;
                preference.PushEnabled = false;
                preference.InAppEnabled = false;
                preference.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{userId}/enable-all")]
        public async Task<IActionResult> EnableAllNotifications(Guid userId)
        {
            var preferences = await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (!preferences.Any())
            {
                preferences = await CreateDefaultPreferences(userId);
            }

            foreach (var preference in preferences)
            {
                preference.EmailEnabled = true;
                preference.SmsEnabled = false; // Keep SMS disabled by default for privacy
                preference.PushEnabled = true;
                preference.InAppEnabled = true;
                preference.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("defaults")]
        public ActionResult<List<NotificationPreferenceDto>> GetDefaultPreferences()
        {
            var defaultPreferences = GetDefaultPreferenceSettings();
            return Ok(defaultPreferences.Select(p => MapToDto(p)).ToList());
        }

        private async Task<List<NotificationPreference>> CreateDefaultPreferences(Guid userId)
        {
            var defaultPreferences = GetDefaultPreferenceSettings()
                .Select(p => new NotificationPreference
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    NotificationType = p.NotificationType,
                    EmailEnabled = p.EmailEnabled,
                    SmsEnabled = p.SmsEnabled,
                    PushEnabled = p.PushEnabled,
                    InAppEnabled = p.InAppEnabled,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                })
                .ToList();

            _context.NotificationPreferences.AddRange(defaultPreferences);
            await _context.SaveChangesAsync();

            return defaultPreferences;
        }

        private List<NotificationPreference> GetDefaultPreferenceSettings()
        {
            return new List<NotificationPreference>
            {
                new() { NotificationType = NotificationType.BookingConfirmation, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.BookingReminder, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.BookingCancellation, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.PaymentConfirmation, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.PaymentFailed, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.PaymentRefund, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.ReviewRequest, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.ReviewReceived, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.PropertyListed, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.PropertyApproved, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.PropertyRejected, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.HostPayout, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.WelcomeMessage, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.PasswordReset, EmailEnabled = true, SmsEnabled = true, PushEnabled = false, InAppEnabled = false },
                new() { NotificationType = NotificationType.AccountVerification, EmailEnabled = true, SmsEnabled = true, PushEnabled = false, InAppEnabled = false },
                new() { NotificationType = NotificationType.SecurityAlert, EmailEnabled = true, SmsEnabled = true, PushEnabled = true, InAppEnabled = true },
                new() { NotificationType = NotificationType.MarketingPromo, EmailEnabled = false, SmsEnabled = false, PushEnabled = false, InAppEnabled = false },
                new() { NotificationType = NotificationType.SystemMaintenance, EmailEnabled = true, SmsEnabled = false, PushEnabled = true, InAppEnabled = true }
            };
        }

        private NotificationPreferenceDto MapToDto(NotificationPreference preference)
        {
            return new NotificationPreferenceDto
            {
                Id = preference.Id,
                UserId = preference.UserId,
                NotificationType = preference.NotificationType,
                EmailEnabled = preference.EmailEnabled,
                SmsEnabled = preference.SmsEnabled,
                PushEnabled = preference.PushEnabled,
                InAppEnabled = preference.InAppEnabled,
                CreatedAt = preference.CreatedAt,
                UpdatedAt = preference.UpdatedAt
            };
        }
    }
}