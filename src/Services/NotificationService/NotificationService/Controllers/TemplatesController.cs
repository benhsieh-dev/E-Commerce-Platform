using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.DTOs;
using NotificationService.Models;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly NotificationDbContext _context;
        private readonly ILogger<TemplatesController> _logger;

        public TemplatesController(NotificationDbContext context, ILogger<TemplatesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationTemplateResponseDto>>> GetTemplates(
            NotificationType? type = null,
            NotificationChannel? channel = null,
            bool activeOnly = true)
        {
            var query = _context.NotificationTemplates.AsQueryable();

            if (type.HasValue)
                query = query.Where(t => t.Type == type);

            if (channel.HasValue)
                query = query.Where(t => t.Channel == channel);

            if (activeOnly)
                query = query.Where(t => t.IsActive);

            var templates = await query
                .OrderBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapToResponseDto).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationTemplateResponseDto>> GetTemplate(Guid id)
        {
            var template = await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            return MapToResponseDto(template);
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<NotificationTemplateResponseDto>> GetTemplateByName(string name)
        {
            var template = await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Name == name);

            if (template == null)
            {
                return NotFound();
            }

            return MapToResponseDto(template);
        }

        [HttpPost]
        public async Task<ActionResult<NotificationTemplateResponseDto>> CreateTemplate(CreateTemplateDto dto)
        {
            // Check if template name already exists
            var existingTemplate = await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Name == dto.Name);

            if (existingTemplate != null)
            {
                return Conflict($"Template with name '{dto.Name}' already exists");
            }

            var template = new NotificationTemplate
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Type = dto.Type,
                Channel = dto.Channel,
                Subject = dto.Subject,
                BodyTemplate = dto.BodyTemplate,
                HtmlTemplate = dto.HtmlTemplate,
                SmsTemplate = dto.SmsTemplate,
                PushTemplate = dto.PushTemplate,
                Description = dto.Description,
                TemplateVariables = dto.TemplateVariables,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.NotificationTemplates.Add(template);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, 
                MapToResponseDto(template));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<NotificationTemplateResponseDto>> UpdateTemplate(Guid id, UpdateTemplateDto dto)
        {
            var template = await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            // Check if new name conflicts with existing template
            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != template.Name)
            {
                var existingTemplate = await _context.NotificationTemplates
                    .FirstOrDefaultAsync(t => t.Name == dto.Name && t.Id != id);

                if (existingTemplate != null)
                {
                    return Conflict($"Template with name '{dto.Name}' already exists");
                }

                template.Name = dto.Name;
            }

            if (!string.IsNullOrEmpty(dto.Subject))
                template.Subject = dto.Subject;

            if (!string.IsNullOrEmpty(dto.BodyTemplate))
                template.BodyTemplate = dto.BodyTemplate;

            if (dto.HtmlTemplate != null)
                template.HtmlTemplate = dto.HtmlTemplate;

            if (dto.SmsTemplate != null)
                template.SmsTemplate = dto.SmsTemplate;

            if (dto.PushTemplate != null)
                template.PushTemplate = dto.PushTemplate;

            if (dto.Description != null)
                template.Description = dto.Description;

            if (dto.IsActive.HasValue)
                template.IsActive = dto.IsActive.Value;

            if (dto.TemplateVariables != null)
                template.TemplateVariables = dto.TemplateVariables;

            template.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponseDto(template);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(Guid id)
        {
            var template = await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            // Check if template is being used by any notifications
            var hasNotifications = await _context.Notifications
                .AnyAsync(n => n.TemplateId == id);

            if (hasNotifications)
            {
                // Soft delete - deactivate instead of removing
                template.IsActive = false;
                template.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "Template deactivated as it's being used by existing notifications" });
            }

            _context.NotificationTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/activate")]
        public async Task<ActionResult<NotificationTemplateResponseDto>> ActivateTemplate(Guid id)
        {
            var template = await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            template.IsActive = true;
            template.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponseDto(template);
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult<NotificationTemplateResponseDto>> DeactivateTemplate(Guid id)
        {
            var template = await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            template.IsActive = false;
            template.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponseDto(template);
        }

        [HttpPost("{id}/preview")]
        public async Task<ActionResult<object>> PreviewTemplate(Guid id, [FromBody] Dictionary<string, object> templateData)
        {
            var template = await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            var processedSubject = ProcessTemplate(template.Subject, templateData);
            var processedBody = ProcessTemplate(template.BodyTemplate, templateData);
            var processedHtml = template.HtmlTemplate != null 
                ? ProcessTemplate(template.HtmlTemplate, templateData) 
                : null;
            var processedSms = template.SmsTemplate != null 
                ? ProcessTemplate(template.SmsTemplate, templateData) 
                : null;
            var processedPush = template.PushTemplate != null 
                ? ProcessTemplate(template.PushTemplate, templateData) 
                : null;

            return Ok(new
            {
                subject = processedSubject,
                body = processedBody,
                html = processedHtml,
                sms = processedSms,
                push = processedPush,
                templateData = templateData
            });
        }

        [HttpPost("validate")]
        public async Task<ActionResult<object>> ValidateTemplate(CreateTemplateDto dto)
        {
            var errors = new List<string>();

            // Validate template syntax (basic validation)
            try
            {
                var testData = new Dictionary<string, object>
                {
                    { "TestField", "TestValue" },
                    { "UserName", "John Doe" },
                    { "Date", DateTime.Now.ToString("yyyy-MM-dd") }
                };

                ProcessTemplate(dto.Subject, testData);
                ProcessTemplate(dto.BodyTemplate, testData);

                if (!string.IsNullOrEmpty(dto.HtmlTemplate))
                    ProcessTemplate(dto.HtmlTemplate, testData);

                if (!string.IsNullOrEmpty(dto.SmsTemplate))
                    ProcessTemplate(dto.SmsTemplate, testData);

                if (!string.IsNullOrEmpty(dto.PushTemplate))
                    ProcessTemplate(dto.PushTemplate, testData);
            }
            catch (Exception ex)
            {
                errors.Add($"Template processing error: {ex.Message}");
            }

            // Check for required fields based on channel
            switch (dto.Channel)
            {
                case NotificationChannel.Email:
                    if (string.IsNullOrEmpty(dto.Subject))
                        errors.Add("Subject is required for email templates");
                    break;

                case NotificationChannel.SMS:
                    if (string.IsNullOrEmpty(dto.SmsTemplate))
                        errors.Add("SMS template is required for SMS notifications");
                    if (dto.SmsTemplate?.Length > 160)
                        errors.Add("SMS template should be 160 characters or less");
                    break;

                case NotificationChannel.Push:
                    if (string.IsNullOrEmpty(dto.PushTemplate))
                        errors.Add("Push template is required for push notifications");
                    break;
            }

            return Ok(new
            {
                isValid = errors.Count == 0,
                errors = errors
            });
        }

        [HttpGet("types")]
        public ActionResult<object> GetNotificationTypes()
        {
            var types = Enum.GetValues<NotificationType>()
                .Select(t => new { value = (int)t, name = t.ToString() })
                .ToList();

            return Ok(types);
        }

        [HttpGet("channels")]
        public ActionResult<object> GetNotificationChannels()
        {
            var channels = Enum.GetValues<NotificationChannel>()
                .Select(c => new { value = (int)c, name = c.ToString() })
                .ToList();

            return Ok(channels);
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

        private NotificationTemplateResponseDto MapToResponseDto(NotificationTemplate template)
        {
            return new NotificationTemplateResponseDto
            {
                Id = template.Id,
                Name = template.Name,
                Type = template.Type,
                Channel = template.Channel,
                Subject = template.Subject,
                BodyTemplate = template.BodyTemplate,
                HtmlTemplate = template.HtmlTemplate,
                SmsTemplate = template.SmsTemplate,
                PushTemplate = template.PushTemplate,
                IsActive = template.IsActive,
                Description = template.Description,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                TemplateVariables = template.TemplateVariables
            };
        }
    }
}