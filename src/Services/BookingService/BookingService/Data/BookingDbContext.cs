using Microsoft.EntityFrameworkCore;
using BookingService.Models;

namespace BookingService.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingStatusHistory> BookingStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ServiceFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CleaningFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.PropertyTitle).HasMaxLength(200);
                entity.Property(e => e.PropertyAddress).HasMaxLength(500);
                entity.Property(e => e.PropertyCity).HasMaxLength(100);
                entity.Property(e => e.PropertyCountry).HasMaxLength(100);
                
                entity.Property(e => e.SpecialRequests).HasMaxLength(1000);
                entity.Property(e => e.CancellationReason).HasMaxLength(500);
                
                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.GuestId);
                entity.HasIndex(e => e.HostId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.CheckInDate, e.CheckOutDate });
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasMany(e => e.StatusHistory)
                    .WithOne(e => e.Booking)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BookingStatusHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasIndex(e => e.BookingId);
                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }
}