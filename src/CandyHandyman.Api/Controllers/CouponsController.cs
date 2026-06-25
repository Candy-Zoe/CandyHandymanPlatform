using System.Security.Claims;
using CandyHandyman.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly ICouponService _couponService;

    public CouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto dto)
    {
        var coupon = await _couponService.CreateCouponAsync(dto);
        return Ok(coupon);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllCoupons()
    {
        var coupons = await _couponService.GetAllCouponsAsync();
        return Ok(coupons);
    }

    [HttpPost("validate")]
    [Authorize]
    public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponDto dto)
    {
        var userId = GetUserId();
        var result = await _couponService.ValidateAsync(dto.Code, dto.OrderAmount, userId);
        return Ok(result);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyCoupons()
    {
        var userId = GetUserId();
        var coupons = await _couponService.GetUserCouponsAsync(userId);
        return Ok(coupons);
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public class ValidateCouponDto
{
    public string Code { get; set; } = string.Empty;
    public decimal OrderAmount { get; set; }
}
