namespace CandyHandyman.Core.Entities;

public class UserCoupon : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CouponId { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public Guid? OrderId { get; set; }

    public User User { get; set; } = null!;
    public Coupon Coupon { get; set; } = null!;
}
