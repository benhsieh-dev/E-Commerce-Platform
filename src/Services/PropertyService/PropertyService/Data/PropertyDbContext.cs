using Microsoft.EntityFrameworkCore;
using PropertyService.Models;

namespace PropertyService.Data
{
    public class PropertyDbContext : DbContext
    {
        public PropertyDbContext(DbContextOptions<PropertyDbContext> options) : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
                
                entity.Property(e => e.PricePerNight).HasColumnType("decimal(18,2)");
                entity.Property(e => e.WeeklyDiscount).HasColumnType("decimal(5,2)");
                entity.Property(e => e.MonthlyDiscount).HasColumnType("decimal(5,2)");
                
                entity.Property(e => e.Images)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .HasMaxLength(2000);
                
                entity.Property(e => e.Amenities)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .HasMaxLength(1000);
                
                entity.Property(e => e.UnavailableDates)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<List<DateRange>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<DateRange>())
                    .HasMaxLength(4000);
                
                entity.HasIndex(e => e.HostId);
                entity.HasIndex(e => new { e.City, e.Country });
                entity.HasIndex(e => e.PropertyType);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}