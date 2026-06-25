using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class WalletTransaction : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public WalletTransactionType Type { get; set; }
    public decimal Balance { get; set; }
    public string? Description { get; set; }
    public Guid? RelatedId { get; set; }

    public User User { get; set; } = null!;
}
