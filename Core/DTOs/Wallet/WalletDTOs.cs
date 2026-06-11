using System.ComponentModel.DataAnnotations;
using Core.Models;

namespace Core.DTOs
{
    public class WalletBalanceDto
    {
        public int WalletId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "NGN";
        public DateTime LastUpdated { get; set; }
    }

    public class FundWalletDto
    {
        [Required]
        [Range(100, 1000000, ErrorMessage = "Amount must be between ₦100 and ₦1,000,000")]
        public decimal Amount { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }
    }

    public class FundWalletResponseDto
    {
        public string Reference { get; set; } = string.Empty;
        public decimal AmountFunded { get; set; }
        public decimal NewBalance { get; set; }
        public string Currency { get; set; } = "NGN";
        public DateTime TransactionDate { get; set; }
    }

    public class TransactionDto
    {
        public int Id { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Currency { get; set; } = "NGN";
        public DateTime CreatedAt { get; set; }
    }

    public class TransactionHistoryResponseDto
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<TransactionDto> Transactions { get; set; } = new();
    }

    public class TransactionFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public TransactionType? Type { get; set; }
        public TransactionStatus? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}