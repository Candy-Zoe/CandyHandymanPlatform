using System.Security.Claims;
using CandyHandyman.Application.DTOs;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using CandyHandyman.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IRepository<Notification> _notificationRepo;
    private readonly IRepository<NotificationSetting> _settingRepo;
    private readonly IRepository<UserFcmToken> _fcmTokenRepo;

    public NotificationsController(
        IRepository<Notification> notificationRepo,
        IRepository<NotificationSetting> settingRepo,
        IRepository<UserFcmToken> fcmTokenRepo)
    {
        _notificationRepo = notificationRepo;
        _settingRepo = settingRepo;
        _fcmTokenRepo = fcmTokenRepo;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<NotificationDto>>> GetAll(
        [FromQuery] NotificationType? type,
        [FromQuery] bool? isRead,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var userId = GetUserId();
        var query = (await _notificationRepo.GetAllAsync())
            .Where(n => n.UserId == userId)
            .AsQueryable();

        if (type.HasValue)
            query = query.Where(n => n.Type == type);
        if (isRead.HasValue)
            query = query.Where(n => n.IsRead == isRead);

        var total = query.Count();
        var items = query.OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                Type = n.Type,
                IsRead = n.IsRead,
                RelatedId = n.RelatedId,
                RelatedType = n.RelatedType,
                Image = n.Image,
                CreatedAt = n.CreatedAt
            })
            .ToList();

        return Ok(new PagedResult<NotificationDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = GetUserId();
        var count = (await _notificationRepo.GetAllAsync())
            .Count(n => n.UserId == userId && !n.IsRead);
        return Ok(new { count });
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var notification = await _notificationRepo.GetByIdAsync(id);
        if (notification == null) return NotFound();
        if (notification.UserId != GetUserId()) return Forbid();

        notification.IsRead = true;
        await _notificationRepo.UpdateAsync(notification);
        return Ok();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetUserId();
        var unread = (await _notificationRepo.GetAllAsync())
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToList();

        foreach (var n in unread)
        {
            n.IsRead = true;
        }
        await _notificationRepo.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var notification = await _notificationRepo.GetByIdAsync(id);
        if (notification == null) return NotFound();
        if (notification.UserId != GetUserId()) return Forbid();

        await _notificationRepo.DeleteAsync(notification);
        return Ok();
    }

    [HttpGet("settings")]
    public async Task<ActionResult<List<NotificationSettingDto>>> GetSettings()
    {
        var userId = GetUserId();
        var settings = (await _settingRepo.GetAllAsync())
            .Where(s => s.UserId == userId)
            .Select(s => new NotificationSettingDto
            {
                Type = s.Type,
                Enabled = s.Enabled
            })
            .ToList();

        var allTypes = Enum.GetValues<NotificationType>();
        var existingTypes = settings.Select(s => s.Type).ToHashSet();
        foreach (var t in allTypes)
        {
            if (!existingTypes.Contains(t))
            {
                settings.Add(new NotificationSettingDto { Type = t, Enabled = true });
            }
        }

        return Ok(settings.OrderBy(s => s.Type).ToList());
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings(List<NotificationSettingDto> dtos)
    {
        var userId = GetUserId();
        var existing = (await _settingRepo.GetAllAsync())
            .Where(s => s.UserId == userId)
            .ToDictionary(s => s.Type);

        foreach (var dto in dtos)
        {
            if (existing.TryGetValue(dto.Type, out var setting))
            {
                setting.Enabled = dto.Enabled;
                await _settingRepo.UpdateAsync(setting);
            }
            else
            {
                await _settingRepo.AddAsync(new Core.Entities.NotificationSetting
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = dto.Type,
                    Enabled = dto.Enabled
                });
            }
        }

        return Ok();
    }

    [HttpPost("fcm-token")]
    public async Task<IActionResult> RegisterFcmToken([FromBody] FcmTokenDto dto)
    {
        var userId = GetUserId();
        var existing = (await _fcmTokenRepo.GetAllAsync())
            .FirstOrDefault(t => t.UserId == userId && t.Token == dto.Token);

        if (existing == null)
        {
            await _fcmTokenRepo.AddAsync(new UserFcmToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = dto.Token,
                DeviceType = dto.DeviceType
            });
        }

        return Ok();
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public class FcmTokenDto
{
    public string Token { get; set; } = string.Empty;
    public string? DeviceType { get; set; }
}
