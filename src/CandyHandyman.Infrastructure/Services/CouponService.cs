using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class CouponService : ICouponService
{
    private readonly AppDbContext _context;

    public CouponService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CouponDto> CreateCouponAsync(CreateCouponDto dto)
    {
        var coupon = new Coupon
        {
            Id = Guid.NewGuid(),
            Code = dto.Code.ToUpper(),
            Name = dto.Name,
            Type = dto.Type,
            DiscountValue = dto.DiscountValue,
            MinAmount = dto.MinAmount,
            MaxUses = dto.MaxUses,
            ExpiresAt = dto.ExpiresAt
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();

        return new CouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Name = coupon.Name,
            Type = coupon.Type,
            DiscountValue = coupon.DiscountValue,
            MinAmount = coupon.MinAmount,
            MaxUses = coupon.MaxUses,
            UsedCount = coupon.UsedCount,
            ExpiresAt = coupon.ExpiresAt,
            IsActive = coupon.IsActive
        };
    }

    public async Task<CouponValidationResult> ValidateAsync(string code, decimal orderAmount, Guid userId)
    {
        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code.ToUpper() && c.IsActive);
        if (coupon == null)
            return new CouponValidationResult { IsValid = false, Message = "优惠码无效" };

        if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt < DateTime.UtcNow)
            return new CouponValidationResult { IsValid = false, Message = "优惠码已过期" };

        if (coupon.UsedCount >= coupon.MaxUses)
            return new CouponValidationResult { IsValid = false, Message = "优惠码已用完" };

        if (orderAmount < coupon.MinAmount)
            return new CouponValidationResult { IsValid = false, Message = $"订单金额需满{coupon.MinAmount}元" };

        var existing = await _context.UserCoupons
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == coupon.Id && uc.IsUsed);
        if (existing != null)
            return new CouponValidationResult { IsValid = false, Message = "您已使用过该优惠码" };

        var discount = coupon.Type == CouponType.Percentage
            ? orderAmount * coupon.DiscountValue / 100
            : coupon.DiscountValue;

        return new CouponValidationResult
        {
            IsValid = true,
            CouponId = coupon.Id,
            DiscountAmount = Math.Min(discount, orderAmount),
            Message = $"优惠{discount:F2}元"
        };
    }

    public async Task ApplyCouponAsync(Guid userId, Guid couponId, Guid orderId)
    {
        var userCoupon = new UserCoupon
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CouponId = couponId,
            IsUsed = true,
            UsedAt = DateTime.UtcNow,
            OrderId = orderId
        };

        _context.UserCoupons.Add(userCoupon);

        var coupon = await _context.Coupons.FindAsync(couponId);
        if (coupon != null)
        {
            coupon.UsedCount++;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserCouponDto>> GetUserCouponsAsync(Guid userId)
    {
        return await _context.UserCoupons
            .Where(uc => uc.UserId == userId)
            .Include(uc => uc.Coupon)
            .Select(uc => new UserCouponDto
            {
                Id = uc.Id,
                Code = uc.Coupon.Code,
                Name = uc.Coupon.Name,
                Type = uc.Coupon.Type,
                DiscountValue = uc.Coupon.DiscountValue,
                MinAmount = uc.Coupon.MinAmount,
                ExpiresAt = uc.Coupon.ExpiresAt,
                IsUsed = uc.IsUsed,
                UsedAt = uc.UsedAt
            })
            .ToListAsync();
    }

    public async Task<List<CouponDto>> GetAllCouponsAsync()
    {
        return await _context.Coupons
            .Select(c => new CouponDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Type = c.Type,
                DiscountValue = c.DiscountValue,
                MinAmount = c.MinAmount,
                MaxUses = c.MaxUses,
                UsedCount = c.UsedCount,
                ExpiresAt = c.ExpiresAt,
                IsActive = c.IsActive
            })
            .ToListAsync();
    }
}
