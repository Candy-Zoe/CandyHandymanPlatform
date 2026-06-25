using System.Security.Claims;
using CandyHandyman.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BrowsingHistoryController : ControllerBase
{
    private readonly IBrowsingHistoryService _historyService;

    public BrowsingHistoryController(IBrowsingHistoryService historyService) => _historyService = historyService;

    [HttpPost]
    public async Task<IActionResult> Record([FromBody] RecordHistoryDto dto)
    {
        await _historyService.RecordAsync(GetUserId(), dto.ServiceId, dto.HandymanProfileId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetHistory([FromQuery] int limit = 50)
    {
        var history = await _historyService.GetUserHistoryAsync(GetUserId(), limit);
        return Ok(history);
    }

    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        await _historyService.ClearHistoryAsync(GetUserId());
        return Ok();
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public class RecordHistoryDto
{
    public Guid? ServiceId { get; set; }
    public Guid? HandymanProfileId { get; set; }
}
