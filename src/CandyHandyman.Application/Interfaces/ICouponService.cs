using CandyHandyman.Core.Enums;

namespace CandyHandyman.Application.Interfaces;

public interface ICouponService
{
    Task<CouponDto> CreateCouponAsync(CreateCouponDto dto);
    Task<CouponValidationResult> ValidateAsync(string code, decimal orderAmount, Guid userId);
    Task ApplyCouponAsync(Guid userId, Guid couponId, Guid orderId);
    Task<List<UserCouponDto>> GetUserCouponsAsync(Guid userId);
    Task<List<CouponDto>> GetAllCouponsAsync();
}

public class CouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CouponType Type { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinAmount { get; set; }
    public int MaxUses { get; set; }
    public int UsedCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCouponDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CouponType Type { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinAmount { get; set; }
    public int MaxUses { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class CouponValidationResult
{
    public bool IsValid { get; set; }
    public Guid? CouponId { get; set; }
    public decimal DiscountAmount { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class UserCouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CouponType Type { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinAmount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
}
