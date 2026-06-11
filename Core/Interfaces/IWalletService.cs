using Core.DTOs;

namespace Core.Interfaces
{
    public interface IWalletService
    {
        Task<WalletBalanceDto> GetBalanceAsync(int userId);
        Task<FundWalletResponseDto> FundWalletAsync(int userId, FundWalletDto dto);
        Task<TransactionHistoryResponseDto> GetTransactionHistoryAsync(int userId, TransactionFilterDto filter);

        // Used internally by Airtime, Data, Cable etc.
        Task DebitWalletAsync(int userId, decimal amount, string transactionType, string description, string reference);
        Task<bool> HasSufficientBalanceAsync(int userId, decimal amount);
    }
}