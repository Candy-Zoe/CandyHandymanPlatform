using CandyHandyman.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RankingsController : ControllerBase
{
    private readonly IRankingService _rankingService;

    public RankingsController(IRankingService rankingService)
    {
        _rankingService = rankingService;
    }

    [HttpGet("handymen")]
    public async Task<IActionResult> GetHandymenRanking(
        [FromQuery] Guid? categoryId,
        [FromQuery] int top = 20)
    {
        var ranking = await _rankingService.GetRankingAsync(categoryId, top);
        return Ok(ranking);
    }
}
