using CandyHandyman.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementService _announcementService;

    public AnnouncementsController(IAnnouncementService announcementService) => _announcementService = announcementService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublished()
    {
        var announcements = await _announcementService.GetPublishedAsync();
        return Ok(announcements);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var announcements = await _announcementService.GetAllAsync();
        return Ok(announcements);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAnnouncementDto dto)
    {
        var announcement = await _announcementService.CreateAsync(dto);
        return Ok(announcement);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateAnnouncementDto dto)
    {
        await _announcementService.UpdateAsync(id, dto);
        return Ok();
    }

    [HttpPost("{id}/publish")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Publish(Guid id)
    {
        await _announcementService.PublishAsync(id);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _announcementService.DeleteAsync(id);
        return Ok();
    }
}
