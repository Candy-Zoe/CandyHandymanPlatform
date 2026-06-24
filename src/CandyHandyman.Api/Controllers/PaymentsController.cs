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

    public PaymentsController(IRepository<Payment> paymentRepo, IRepository<Order> orderRepo)
    {
        _paymentRepo = paymentRepo;
        _orderRepo = orderRepo;
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

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}