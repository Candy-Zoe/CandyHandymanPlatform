using System.Security.Claims;
using CandyHandyman.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeedbacksController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;

    public FeedbacksController(IFeedbackService feedbackService) => _feedbackService = feedbackService;

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitFeedbackDto dto)
    {
        var feedback = await _feedbackService.SubmitAsync(GetUserId(), dto);
        return Ok(feedback);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyFeedbacks()
    {
        var feedbacks = await _feedbackService.GetUserFeedbacksAsync(GetUserId());
        return Ok(feedbacks);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var feedbacks = await _feedbackService.GetAllFeedbacksAsync(page, pageSize);
        return Ok(feedbacks);
    }

    [HttpPost("{id}/reply")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reply(Guid id, [FromBody] ReplyDto dto)
    {
        await _feedbackService.ReplyAsync(id, dto.Reply);
        return Ok();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] StatusDto dto)
    {
        await _feedbackService.UpdateStatusAsync(id, dto.Status);
        return Ok();
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public class ReplyDto
{
    public string Reply { get; set; } = string.Empty;
}

public class StatusDto
{
    public string Status { get; set; } = string.Empty;
}
