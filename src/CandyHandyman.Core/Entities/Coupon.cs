using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CouponType Type { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinAmount { get; set; }
    public int MaxUses { get; set; }
    public int UsedCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
}
