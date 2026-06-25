using System.Security.Claims;
using CandyHandyman.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly IRepository<Core.Entities.HandymanProfile> _handymanRepo;

    public ScheduleController(IScheduleService scheduleService, IRepository<Core.Entities.HandymanProfile> handymanRepo)
    {
        _scheduleService = scheduleService;
        _handymanRepo = handymanRepo;
    }

    [HttpGet("{handymanId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSchedules(Guid handymanId)
    {
        var schedules = await _scheduleService.GetSchedulesAsync(handymanId);
        return Ok(schedules.Select(s => new
        {
            s.Id,
            s.DayOfWeek,
            StartTime = s.StartTime.ToString(@"hh\:mm"),
            EndTime = s.EndTime.ToString(@"hh\:mm"),
            s.IsAvailable
        }));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSchedules([FromBody] List<Application.Interfaces.ScheduleDto> schedules)
    {
        var userId = GetUserId();
        var profile = (await _handymanRepo.GetAllAsync()).FirstOrDefault(h => h.UserId == userId);
        if (profile == null) return BadRequest(new { message = "请先注册为工匠" });

        await _scheduleService.UpdateSchedulesAsync(profile.Id, schedules);
        return Ok();
    }

    [HttpGet("{handymanId}/slots")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableSlots(Guid handymanId, [FromQuery] DateTime date)
    {
        var slots = await _scheduleService.GetAvailableSlotsAsync(handymanId, date);
        return Ok(slots.Select(s => new
        {
            s.Id,
            s.Date,
            StartTime = s.StartTime.ToString(@"hh\:mm"),
            EndTime = s.EndTime.ToString(@"hh\:mm"),
            s.IsBooked
        }));
    }

    [HttpPost("{handymanId}/slots/generate")]
    public async Task<IActionResult> GenerateSlots(Guid handymanId, [FromQuery] int days = 14)
    {
        var userId = GetUserId();
        var profile = (await _handymanRepo.GetAllAsync()).FirstOrDefault(h => h.UserId == userId);
        if (profile == null || profile.Id != handymanId) return Forbid();

        await _scheduleService.GenerateSlotsAsync(handymanId, DateTime.UtcNow.Date, days);
        return Ok(new { message = $"已生成未来{days}天的预约时段" });
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
