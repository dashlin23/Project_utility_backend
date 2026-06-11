using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Logic.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklist { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionPin> TransactionPins { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AirtimeTransaction> AirtimeTransactions { get; set; }
        public DbSet<DataTransaction> DataTransactions { get; set; }
        public DbSet<CableTvTransaction> CableTvTransactions { get; set; }
        public DbSet<ElectricityTransaction> ElectricityTransactions { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
            });


           
            modelBuilder.Entity<TokenBlacklist>(entity =>
            {
                entity.Property(t => t.Token).HasColumnType("nvarchar(max)");
                
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => p.Token).IsUnique();
            });

            modelBuilder.Entity<Wallet>()
                    .HasOne(w => w.User)
                    .WithOne()
                    .HasForeignKey<Wallet>(w => w.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Wallet>()
                .HasIndex(w => w.UserId)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.Reference)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TransactionPin>(entity =>
            {
                entity.HasOne(t => t.User)
                      .WithMany()
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => t.UserId).IsUnique();
            });
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<AirtimeTransaction>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithMany()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(a => a.Amount).HasColumnType("decimal(18,2)");
            });
            modelBuilder.Entity<DataTransaction>(entity =>
            {
                entity.HasOne(d => d.User)
                      .WithMany()
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(d => d.Amount).HasColumnType("decimal(18,2)");
            });
            modelBuilder.Entity<CableTvTransaction>(entity =>
            {
                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.Amount).HasColumnType("decimal(18,2)");
            });
            modelBuilder.Entity<ElectricityTransaction>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            });
        }
    }
}
