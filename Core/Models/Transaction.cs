using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public enum TransactionType
    {
        Fund, Airtime, Data, Cable, Electricity, Transfer, Reversal
    }

    public enum TransactionStatus
    {
        Pending, Success, Failed
    }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceBefore { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfter { get; set; }

        [Required]
        [MaxLength(100)]
        public string Reference { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("WalletId")]
        public Wallet? Wallet { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}