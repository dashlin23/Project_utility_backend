using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Logic.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logic.Services
{
    public class WalletService : IWalletService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WalletService> _logger;

        public WalletService(AppDbContext context, ILogger<WalletService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WalletBalanceDto> GetBalanceAsync(int userId)
        {
            var wallet = await GetWalletByUserIdAsync(userId);
            return new WalletBalanceDto
            {
                WalletId = wallet.Id,
                Balance = wallet.Balance,
                Currency = "NGN",
                LastUpdated = wallet.UpdatedAt
            };
        }

        public async Task<FundWalletResponseDto> FundWalletAsync(int userId, FundWalletDto dto)
        {
            var wallet = await GetWalletByUserIdAsync(userId);

            var balanceBefore = wallet.Balance;
            var reference = GenerateReference("FUND");

            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                UserId = userId,
                Type = TransactionType.Fund,
                Amount = dto.Amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceBefore + dto.Amount,
                Reference = reference,
                Description = dto.Description ?? $"Wallet funded with ₦{dto.Amount:N2}",
                Status = TransactionStatus.Success
            };

            wallet.Balance += dto.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Wallet funded: UserId={UserId}, Amount={Amount}, Ref={Ref}",
                userId, dto.Amount, reference);

            return new FundWalletResponseDto
            {
                Reference = reference,
                AmountFunded = dto.Amount,
                NewBalance = wallet.Balance,
                Currency = "NGN",
                TransactionDate = transaction.CreatedAt
            };
        }

        public async Task<TransactionHistoryResponseDto> GetTransactionHistoryAsync(int userId, TransactionFilterDto filter)
        {
            var wallet = await GetWalletByUserIdAsync(userId);

            var query = _context.Transactions
                .Where(t => t.WalletId == wallet.Id)
                .AsQueryable();

            if (filter.Type.HasValue)
                query = query.Where(t => t.Type == filter.Type.Value);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.From.HasValue)
                query = query.Where(t => t.CreatedAt >= filter.From.Value);

            if (filter.To.HasValue)
                query = query.Where(t => t.CreatedAt <= filter.To.Value);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var transactions = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Reference = t.Reference,
                    Type = t.Type.ToString(),
                    Amount = t.Amount,
                    BalanceBefore = t.BalanceBefore,
                    BalanceAfter = t.BalanceAfter,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Currency = "NGN",
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return new TransactionHistoryResponseDto
            {
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                Transactions = transactions
            };
        }

        public async Task DebitWalletAsync(int userId, decimal amount, string transactionType, string description, string reference)
        {
            var wallet = await GetWalletByUserIdAsync(userId);

            if (wallet.Balance < amount)
                throw new InvalidOperationException("Insufficient wallet balance.");

            if (!Enum.TryParse<TransactionType>(transactionType, true, out var txType))
                throw new ArgumentException($"Invalid transaction type: {transactionType}");

            var balanceBefore = wallet.Balance;

            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                UserId = userId,
                Type = txType,
                Amount = amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceBefore - amount,
                Reference = reference,
                Description = description,
                Status = TransactionStatus.Success
            };

            wallet.Balance -= amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasSufficientBalanceAsync(int userId, decimal amount)
        {
            var wallet = await GetWalletByUserIdAsync(userId);
            return wallet.Balance >= amount;
        }

        private async Task<Wallet> GetWalletByUserIdAsync(int userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId)
                ?? throw new KeyNotFoundException("Wallet not found for this user.");
        }

        private static string GenerateReference(string prefix)
            => $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}