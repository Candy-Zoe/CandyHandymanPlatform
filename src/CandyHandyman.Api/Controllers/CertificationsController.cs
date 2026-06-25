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
public class CertificationsController : ControllerBase
{
    private readonly IRepository<CraftsmanCertification> _certRepo;
    private readonly IRepository<HandymanProfile> _handymanRepo;
    private readonly INotificationService _notificationService;

    public CertificationsController(IRepository<CraftsmanCertification> certRepo, IRepository<HandymanProfile> handymanRepo, INotificationService notificationService)
    {
        _certRepo = certRepo;
        _handymanRepo = handymanRepo;
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitCertification([FromBody] CraftsmanCertification dto)
    {
        var userId = GetUserId();
        var profile = (await _handymanRepo.GetAllAsync()).FirstOrDefault(h => h.UserId == userId);
        if (profile == null) return BadRequest(new { message = "请先注册为工匠" });

        dto.Id = Guid.NewGuid();
        dto.HandymanProfileId = profile.Id;
        dto.Status = VerificationStatus.Pending;
        await _certRepo.AddAsync(dto);

        await _notificationService.SendNotificationAsync(
            userId,
            "资质认证已提交",
            $"您的资质认证 [{dto.CertificateName}] 已提交，等待审核",
            NotificationType.Certification,
            dto.Id,
            "Certification");

        return Ok(new { message = "提交成功，等待审核" });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyCertifications()
    {
        var userId = GetUserId();
        var profile = (await _handymanRepo.GetAllAsync()).FirstOrDefault(h => h.UserId == userId);
        if (profile == null) return Ok(new List<object>());

        var certs = (await _certRepo.GetAllAsync())
            .Where(c => c.HandymanProfileId == profile.Id)
            .Select(c => new
            {
                c.Id,
                c.SkillName,
                c.CertificateName,
                c.CertificateNo,
                c.Status,
                c.RejectReason
            })
            .ToList();

        return Ok(certs);
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}