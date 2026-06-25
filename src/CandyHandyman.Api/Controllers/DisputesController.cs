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
public class DisputesController : ControllerBase
{
    private readonly IRepository<Dispute> _disputeRepo;
    private readonly IRepository<Order> _orderRepo;
    private readonly INotificationService _notificationService;

    public DisputesController(IRepository<Dispute> disputeRepo, IRepository<Order> orderRepo, INotificationService notificationService)
    {
        _disputeRepo = disputeRepo;
        _orderRepo = orderRepo;
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDispute([FromBody] CreateDisputeDto dto)
    {
        var userId = GetUserId();
        var order = await _orderRepo.GetByIdAsync(dto.OrderId);
        if (order == null) return NotFound();

        var respondentId = order.CustomerId == userId ? order.ProviderId : order.CustomerId;

        var dispute = new Dispute
        {
            Id = Guid.NewGuid(),
            OrderId = dto.OrderId,
            InitiatorId = userId,
            RespondentId = respondentId,
            Reason = dto.Reason,
            Description = dto.Description,
            EvidenceUrls = dto.EvidenceUrls
        };

        await _disputeRepo.AddAsync(dispute);

        order.Status = OrderStatus.Disputed;
        await _orderRepo.UpdateAsync(order);

        await _notificationService.SendNotificationAsync(
            respondentId,
            "新争议",
            $"订单 {order.OrderNo} 收到争议：{dto.Reason}",
            NotificationType.Dispute,
            dispute.Id,
            "Dispute");

        return Ok(new { message = "争议已提交" });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyDisputes()
    {
        var userId = GetUserId();
        var disputes = (await _disputeRepo.GetAllAsync())
            .Where(d => d.InitiatorId == userId || d.RespondentId == userId)
            .Select(d => new
            {
                d.Id,
                d.OrderId,
                d.Reason,
                d.Status,
                d.Resolution,
                d.CreatedAt,
                d.ResolvedAt
            })
            .ToList();

        return Ok(disputes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDispute(Guid id)
    {
        var dispute = await _disputeRepo.GetByIdAsync(id);
        if (dispute == null) return NotFound();

        return Ok(new
        {
            dispute.Id,
            dispute.OrderId,
            dispute.Reason,
            dispute.Description,
            dispute.EvidenceUrls,
            dispute.Status,
            dispute.Resolution,
            dispute.AdminNotes,
            dispute.CreatedAt,
            dispute.ResolvedAt
        });
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}