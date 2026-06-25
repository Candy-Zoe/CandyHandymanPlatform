using CandyHandyman.Core.Enums;

namespace CandyHandyman.Application.Interfaces;

public interface IWalletService
{
    Task<decimal> GetBalanceAsync(Guid userId);
    Task AddBalanceAsync(Guid userId, decimal amount, WalletTransactionType type, string? description = null, Guid? relatedId = null);
    Task DeductBalanceAsync(Guid userId, decimal amount, WalletTransactionType type, string? description = null, Guid? relatedId = null);
    Task<List<WalletTransactionDto>> GetTransactionsAsync(Guid userId, int page = 1, int pageSize = 20);
}

public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public WalletTransactionType Type { get; set; }
    public decimal Balance { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
