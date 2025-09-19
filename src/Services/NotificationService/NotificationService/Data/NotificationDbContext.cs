using Microsoft.EntityFrameworkCore;
using NotificationService.Models;

namespace NotificationService.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationDeliveryAttempt> NotificationDeliveryAttempts { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(4000);
                entity.Property(e => e.HtmlContent).HasMaxLength(8000);
                entity.Property(e => e.RecipientEmail).HasMaxLength(254);
                entity.Property(e => e.RecipientPhone).HasMaxLength(20);
                entity.Property(e => e.RecipientPushToken).HasMaxLength(500);
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                entity.Property(e => e.TemplateData).HasMaxLength(4000);
                entity.Property(e => e.TrackingId).HasMaxLength(100);
                
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Channel);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.ScheduledAt);
                entity.HasIndex(e => e.BookingId);
                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.TrackingId);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasMany(e => e.DeliveryAttempts)
                    .WithOne(e => e.Notification)
                    .HasForeignKey(e => e.NotificationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NotificationDeliveryAttempt>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                entity.Property(e => e.ExternalId).HasMaxLength(255);
                entity.Property(e => e.Response).HasMaxLength(2000);
                
                entity.HasIndex(e => e.NotificationId);
                entity.HasIndex(e => e.Channel);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.AttemptedAt);
                entity.HasIndex(e => e.ExternalId);
                
                entity.Property(e => e.AttemptedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<NotificationTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
                entity.Property(e => e.BodyTemplate).IsRequired().HasMaxLength(4000);
                entity.Property(e => e.HtmlTemplate).HasMaxLength(8000);
                entity.Property(e => e.SmsTemplate).HasMaxLength(1000);
                entity.Property(e => e.PushTemplate).HasMaxLength(1000);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.TemplateVariables).HasMaxLength(2000);
                
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Channel);
                entity.HasIndex(e => e.IsActive);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<NotificationPreference>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.HasIndex(e => new { e.UserId, e.NotificationType }).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.NotificationType);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Seed default notification templates
            SeedNotificationTemplates(modelBuilder);
        }

        private static void SeedNotificationTemplates(ModelBuilder modelBuilder)
        {
            var templates = new[]
            {
                new NotificationTemplate
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "BookingConfirmationEmail",
                    Type = NotificationType.BookingConfirmation,
                    Channel = NotificationChannel.Email,
                    Subject = "Booking Confirmed - {{PropertyTitle}}",
                    BodyTemplate = "Dear {{GuestName}},\n\nYour booking at {{PropertyTitle}} has been confirmed!\n\nCheck-in: {{CheckInDate}}\nCheck-out: {{CheckOutDate}}\nGuests: {{GuestCount}}\nTotal: {{TotalAmount}}\n\nThank you for choosing our platform!",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new NotificationTemplate
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "BookingReminderEmail",
                    Type = NotificationType.BookingReminder,
                    Channel = NotificationChannel.Email,
                    Subject = "Upcoming Stay - {{PropertyTitle}}",
                    BodyTemplate = "Hi {{GuestName}},\n\nJust a friendly reminder that your stay at {{PropertyTitle}} is coming up!\n\nCheck-in: {{CheckInDate}} at {{CheckInTime}}\nAddress: {{PropertyAddress}}\n\nWe're excited to host you!",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new NotificationTemplate
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "PaymentConfirmationEmail",
                    Type = NotificationType.PaymentConfirmation,
                    Channel = NotificationChannel.Email,
                    Subject = "Payment Confirmed - {{Amount}}",
                    BodyTemplate = "Dear {{UserName}},\n\nYour payment of {{Amount}} has been successfully processed.\n\nTransaction ID: {{TransactionId}}\nBooking: {{PropertyTitle}}\n\nThank you!",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            modelBuilder.Entity<NotificationTemplate>().HasData(templates);
        }
    }
}