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
public class InsuranceController : ControllerBase
{
    private readonly IRepository<InsurancePolicy> _insuranceRepo;
    private readonly IRepository<Order> _orderRepo;

    public InsuranceController(IRepository<InsurancePolicy> insuranceRepo, IRepository<Order> orderRepo)
    {
        _insuranceRepo = insuranceRepo;
        _orderRepo = orderRepo;
    }

    [HttpPost]
    public async Task<IActionResult> PurchaseInsurance([FromBody] InsurancePurchaseDto dto)
    {
        var userId = GetUserId();
        var order = await _orderRepo.GetByIdAsync(dto.OrderId);
        if (order == null) return NotFound();

        var insuranceType = Enum.Parse<InsuranceType>(dto.Type);
        var premium = insuranceType switch
        {
            InsuranceType.PersonalInjury => 5.0m,
            InsuranceType.PropertyDamage => 8.0m,
            InsuranceType.Comprehensive => 12.0m,
            _ => 5.0m
        };

        var policy = new InsurancePolicy
        {
            Id = Guid.NewGuid(),
            OrderId = dto.OrderId,
            CustomerId = userId,
            ProviderId = order.ProviderId,
            PolicyNo = $"INS{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}",
            Type = insuranceType,
            Premium = premium,
            CoverageAmount = premium * 100,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        await _insuranceRepo.AddAsync(policy);
        return Ok(new
        {
            policy.Id,
            policy.PolicyNo,
            policy.Premium,
            policy.CoverageAmount,
            Type = policy.Type.ToString(),
            policy.EndDate
        });
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetOrderInsurance(Guid orderId)
    {
        var policy = (await _insuranceRepo.GetAllAsync())
            .FirstOrDefault(p => p.OrderId == orderId);

        if (policy == null) return Ok(new { hasInsurance = false });

        return Ok(new
        {
            hasInsurance = true,
            policy.PolicyNo,
            Type = policy.Type.ToString(),
            policy.Premium,
            policy.CoverageAmount,
            Status = policy.Status.ToString(),
            policy.EndDate
        });
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}