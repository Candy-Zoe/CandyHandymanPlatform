using System.Security.Claims;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly IWechatPayService _wechatPayService;

    public WalletController(IWalletService walletService, IWechatPayService wechatPayService)
    {
        _walletService = walletService;
        _wechatPayService = wechatPayService;
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = GetUserId();
        var balance = await _walletService.GetBalanceAsync(userId);
        return Ok(new { balance });
    }

    [HttpPost("recharge")]
    public async Task<IActionResult> Recharge([FromBody] RechargeDto dto)
    {
        var userId = GetUserId();
        var orderNo = $"WL{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        var result = await _wechatPayService.CreatePaymentAsync(orderNo, dto.Amount, "钱包充值");
        return Ok(new
        {
            orderNo,
            result.PrepayId,
            result.PaymentUrl,
            result.QrCodeUrl
        });
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawDto dto)
    {
        var userId = GetUserId();
        try
        {
            await _walletService.DeductBalanceAsync(userId, dto.Amount, WalletTransactionType.Withdraw, "余额提现");
            return Ok(new { message = "提现成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetUserId();
        var transactions = await _walletService.GetTransactionsAsync(userId, page, pageSize);
        return Ok(transactions);
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public class RechargeDto
{
    public decimal Amount { get; set; }
}

public class WithdrawDto
{
    public decimal Amount { get; set; }
}
