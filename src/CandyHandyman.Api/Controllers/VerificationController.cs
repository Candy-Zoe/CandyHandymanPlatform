using System.Security.Claims;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VerificationController : ControllerBase
{
    private readonly IRepository<IdentityVerification> _verificationRepo;
    private readonly IRepository<User> _userRepo;
    private readonly INotificationService _notificationService;

    public VerificationController(IRepository<IdentityVerification> verificationRepo, IRepository<User> userRepo, INotificationService notificationService)
    {
        _verificationRepo = verificationRepo;
        _userRepo = userRepo;
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitVerification([FromBody] IdentityVerification dto)
    {
        var userId = GetUserId();
        var existing = (await _verificationRepo.GetAllAsync()).FirstOrDefault(v => v.UserId == userId);
        if (existing != null && existing.Status == VerificationStatus.Pending)
            return BadRequest(new { message = "已有待审核的认证" });

        dto.Id = Guid.NewGuid();
        dto.UserId = userId;
        dto.Status = VerificationStatus.Pending;
        await _verificationRepo.AddAsync(dto);

        await _notificationService.SendNotificationAsync(
            userId,
            "实名认证已提交",
            "您的实名认证已提交，等待审核",
            NotificationType.Certification,
            dto.Id,
            "Verification");

        return Ok(new { message = "提交成功，等待审核" });
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetVerificationStatus()
    {
        var userId = GetUserId();
        var verification = (await _verificationRepo.GetAllAsync()).FirstOrDefault(v => v.UserId == userId);
        if (verification == null) return Ok(new { status = "NotSubmitted" });
        return Ok(new
        {
            status = verification.Status.ToString(),
            realName = verification.RealName,
            verifiedAt = verification.VerifiedAt
        });
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}