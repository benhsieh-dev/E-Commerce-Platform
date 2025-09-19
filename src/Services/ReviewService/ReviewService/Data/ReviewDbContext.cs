using Microsoft.EntityFrameworkCore;
using ReviewService.Models;

namespace ReviewService.Data
{
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options)
        {
        }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewHelpfulness> ReviewHelpfulness { get; set; }
        public DbSet<ReviewFlag> ReviewFlags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.ReviewText).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Response).HasMaxLength(1000);
                entity.Property(e => e.ModerationNotes).HasMaxLength(500);
                
                entity.Property(e => e.PropertyTitle).HasMaxLength(200);
                entity.Property(e => e.ReviewerName).HasMaxLength(100);
                entity.Property(e => e.RevieweeName).HasMaxLength(100);
                
                entity.HasIndex(e => e.BookingId).IsUnique();
                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.ReviewerId);
                entity.HasIndex(e => e.RevieweeId);
                entity.HasIndex(e => e.ReviewType);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.OverallRating);
                entity.HasIndex(e => e.CreatedAt);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasMany(e => e.Helpfulness)
                    .WithOne(e => e.Review)
                    .HasForeignKey(e => e.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(e => e.Flags)
                    .WithOne(e => e.Review)
                    .HasForeignKey(e => e.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ReviewHelpfulness>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.HasIndex(e => new { e.ReviewId, e.UserId }).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsHelpful);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<ReviewFlag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Resolution).HasMaxLength(500);
                
                entity.HasIndex(e => e.ReviewId);
                entity.HasIndex(e => e.FlaggerId);
                entity.HasIndex(e => e.Reason);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}