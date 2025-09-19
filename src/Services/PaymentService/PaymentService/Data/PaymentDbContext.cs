using Microsoft.EntityFrameworkCore;
using PaymentService.Models;

namespace PaymentService.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.RefundAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PlatformFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ProcessingFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.HostAmount).HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.ExternalTransactionId).HasMaxLength(255);
                entity.Property(e => e.PaymentMethodId).HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.FailureReason).HasMaxLength(500);
                entity.Property(e => e.RefundReason).HasMaxLength(500);
                entity.Property(e => e.PropertyTitle).HasMaxLength(200);
                
                entity.HasIndex(e => e.BookingId);
                entity.HasIndex(e => e.PayerId);
                entity.HasIndex(e => e.PayeeId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ExternalTransactionId);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasMany(e => e.Transactions)
                    .WithOne(e => e.Payment)
                    .HasForeignKey(e => e.PaymentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ExternalTransactionId).HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.FailureReason).HasMaxLength(500);
                
                entity.HasIndex(e => e.PaymentId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ExternalTransactionId);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.StripePaymentMethodId).HasMaxLength(255);
                entity.Property(e => e.CardLast4).HasMaxLength(4);
                entity.Property(e => e.CardBrand).HasMaxLength(50);
                entity.Property(e => e.BankLast4).HasMaxLength(4);
                entity.Property(e => e.BankName).HasMaxLength(100);
                
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsDefault);
                entity.HasIndex(e => e.StripePaymentMethodId);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}