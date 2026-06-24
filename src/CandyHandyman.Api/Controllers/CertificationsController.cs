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

    public CertificationsController(IRepository<CraftsmanCertification> certRepo, IRepository<HandymanProfile> handymanRepo)
    {
        _certRepo = certRepo;
        _handymanRepo = handymanRepo;
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