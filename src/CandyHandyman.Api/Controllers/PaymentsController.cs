using System.Security.Claims;
using CandyHandyman.Application.DTOs;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IRepository<Payment> _paymentRepo;
    private readonly IRepository<Order> _orderRepo;
    private readonly INotificationService _notificationService;
    private readonly IWechatPayService _wechatPayService;
    private readonly IWalletService _walletService;
    private readonly ICouponService _couponService;

    public PaymentsController(
        IRepository<Payment> paymentRepo,
        IRepository<Order> orderRepo,
        INotificationService notificationService,
        IWechatPayService wechatPayService,
        IWalletService walletService,
        ICouponService couponService)
    {
        _paymentRepo = paymentRepo;
        _orderRepo = orderRepo;
        _notificationService = notificationService;
        _wechatPayService = wechatPayService;
        _walletService = walletService;
        _couponService = couponService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
    {
        var userId = GetUserId();
        var order = await _orderRepo.GetByIdAsync(dto.OrderId);
        if (order == null) return NotFound();
        if (order.CustomerId != userId) return Forbid();

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = dto.OrderId,
            Amount = order.TotalAmount,
            PaymentMethod = dto.PaymentMethod,
            Status = PaymentStatus.Paid,
            TransactionId = $"TXN{DateTime.UtcNow:yyyyMMddHHmmfff}"
        };

        await _paymentRepo.AddAsync(payment);

        order.Status = OrderStatus.Accepted;
        order.AcceptedAt = DateTime.UtcNow;
        await _orderRepo.UpdateAsync(order);

        await _notificationService.SendNotificationAsync(
            order.ProviderId,
            "订单已支付",
            $"订单 {order.OrderNo} 已支付成功，金额：{order.TotalAmount:F2}元",
            NotificationType.PaymentUpdate,
            order.Id,
            "Order");

        return Ok(new
        {
            payment.Id,
            payment.TransactionId,
            payment.Amount,
            payment.PaymentMethod,
            payment.Status,
            message = "支付成功"
        });
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetPaymentHistory()
    {
        var userId = GetUserId();
        var payments = (await _paymentRepo.GetAllAsync()).ToList();
        var orderIds = payments.Select(p => p.OrderId).ToList();

        var orders = (await _orderRepo.GetAllAsync())
            .Where(o => orderIds.Contains(o.Id) && (o.CustomerId == userId || o.ProviderId == userId))
            .ToList();

        var orderDict = orders.ToDictionary(o => o.Id);
        var result = payments
            .Where(p => orderDict.ContainsKey(p.OrderId))
            .Select(p => new
            {
                p.Id,
                p.TransactionId,
                p.Amount,
                p.PaymentMethod,
                p.Status,
                p.CreatedAt
            })
            .ToList();

        return Ok(result);
    }

    [HttpPost("wechat/create")]
    public async Task<IActionResult> CreateWechatPayment([FromBody] WechatPayCreateDto dto)
    {
        var userId = GetUserId();
        var order = await _orderRepo.GetByIdAsync(dto.OrderId);
        if (order == null) return NotFound();
        if (order.CustomerId != userId) return Forbid();

        var result = await _wechatPayService.CreatePaymentAsync(
            order.OrderNo,
            order.TotalAmount,
            $"万能工匠-订单{order.OrderNo}",
            dto.OpenId);

        return Ok(new
        {
            order.OrderNo,
            result.PrepayId,
            result.PaymentUrl,
            result.QrCodeUrl,
            result.CodeUrl
        });
    }

    [HttpPost("wechat/notify")]
    [AllowAnonymous]
    public async Task<IActionResult> WechatPayNotify()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        var signature = Request.Headers["Wechatpay-Signature"].FirstOrDefault() ?? "";
        var timestamp = Request.Headers["Wechatpay-Timestamp"].FirstOrDefault() ?? "";
        var nonce = Request.Headers["Wechatpay-Nonce"].FirstOrDefault() ?? "";

        var isValid = await _wechatPayService.HandleNotifyAsync(body, signature, timestamp, nonce);
        if (!isValid) return BadRequest();

        var payload = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(body);
        var orderNo = payload?.GetValueOrDefault("out_trade_no")?.ToString();
        if (string.IsNullOrEmpty(orderNo)) return Ok();

        var order = (await _orderRepo.GetAllAsync())
            .FirstOrDefault(o => o.OrderNo == orderNo);
        if (order == null) return Ok();

        var existingPayment = (await _paymentRepo.GetAllAsync())
            .FirstOrDefault(p => p.OrderId == order.Id);
        if (existingPayment != null) return Ok();

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Amount = order.TotalAmount,
            PaymentMethod = "WechatPay",
            Status = PaymentStatus.Paid,
            TransactionId = $"TXN{DateTime.UtcNow:yyyyMMddHHmmssfff}{Random.Shared.Next(1000, 9999)}"
        };

        await _paymentRepo.AddAsync(payment);

        order.Status = OrderStatus.Accepted;
        order.AcceptedAt = DateTime.UtcNow;
        await _orderRepo.UpdateAsync(order);

        await _notificationService.SendNotificationAsync(
            order.ProviderId,
            "订单已支付",
            $"订单 {order.OrderNo} 已支付成功，金额：{order.TotalAmount:F2}元",
            NotificationType.PaymentUpdate,
            order.Id,
            "Order");

        return Ok();
    }

    [HttpPost("refund")]
    public async Task<IActionResult> Refund([FromBody] RefundDto dto)
    {
        var userId = GetUserId();
        var order = await _orderRepo.GetByIdAsync(dto.OrderId);
        if (order == null) return NotFound();

        var payment = (await _paymentRepo.GetAllAsync()).FirstOrDefault(p => p.OrderId == order.Id && p.Status == PaymentStatus.Paid);
        if (payment == null) return BadRequest(new { message = "无有效支付记录" });

        var result = await _wechatPayService.RefundAsync(order.OrderNo, dto.Amount, dto.Reason);

        if (result.Success)
        {
            payment.Status = PaymentStatus.Refunded;
            await _paymentRepo.UpdateAsync(payment);

            await _walletService.AddBalanceAsync(
                order.CustomerId,
                dto.Amount,
                WalletTransactionType.Refund,
                $"订单{order.OrderNo}退款",
                order.Id);

            await _notificationService.SendNotificationAsync(
                order.CustomerId,
                "退款成功",
                $"订单 {order.OrderNo} 已退款{dto.Amount:F2}元",
                NotificationType.PaymentUpdate,
                order.Id,
                "Order");
        }

        return Ok(new { result.Success, result.RefundId, result.Message });
    }

    [HttpPost("wallet/pay")]
    public async Task<IActionResult> WalletPay([FromBody] WalletPayDto dto)
    {
        var userId = GetUserId();
        var order = await _orderRepo.GetByIdAsync(dto.OrderId);
        if (order == null) return NotFound();
        if (order.CustomerId != userId) return Forbid();

        var balance = await _walletService.GetBalanceAsync(userId);
        var finalAmount = order.TotalAmount - dto.DiscountAmount;

        if (balance < finalAmount)
            return BadRequest(new { message = "余额不足" });

        try
        {
            await _walletService.DeductBalanceAsync(
                userId,
                finalAmount,
                WalletTransactionType.Payment,
                $"订单{order.OrderNo}支付",
                order.Id);

            if (dto.CouponId.HasValue)
            {
                await _couponService.ApplyCouponAsync(userId, dto.CouponId.Value, order.Id);
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Amount = finalAmount,
                PaymentMethod = "Wallet",
                Status = PaymentStatus.Paid,
                TransactionId = $"WLT{DateTime.UtcNow:yyyyMMddHHmmfff}"
            };

            await _paymentRepo.AddAsync(payment);

            order.Status = OrderStatus.Accepted;
            order.AcceptedAt = DateTime.UtcNow;
            await _orderRepo.UpdateAsync(order);

            await _notificationService.SendNotificationAsync(
                order.ProviderId,
                "订单已支付",
                $"订单 {order.OrderNo} 已支付成功，金额：{finalAmount:F2}元",
                NotificationType.PaymentUpdate,
                order.Id,
                "Order");

            return Ok(new { message = "支付成功", amount = finalAmount });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetPaymentStatus(Guid id)
    {
        var payment = await _paymentRepo.GetByIdAsync(id);
        if (payment == null) return NotFound();

        return Ok(new
        {
            payment.Id,
            payment.Status,
            payment.Amount,
            payment.TransactionId,
            payment.CreatedAt
        });
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public class WechatPayCreateDto
{
    public Guid OrderId { get; set; }
    public string? OpenId { get; set; }
}

public class RefundDto
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class WalletPayDto
{
    public Guid OrderId { get; set; }
    public decimal DiscountAmount { get; set; }
    public Guid? CouponId { get; set; }
}