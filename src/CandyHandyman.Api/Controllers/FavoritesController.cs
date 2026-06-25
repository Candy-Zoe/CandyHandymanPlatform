using System.Security.Claims;
using CandyHandyman.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService) => _favoriteService = favoriteService;

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddFavoriteDto dto)
    {
        await _favoriteService.AddFavoriteAsync(GetUserId(), dto.ServiceId, dto.HandymanProfileId);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Remove([FromBody] AddFavoriteDto dto)
    {
        await _favoriteService.RemoveFavoriteAsync(GetUserId(), dto.ServiceId, dto.HandymanProfileId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetMyFavorites()
    {
        var favorites = await _favoriteService.GetUserFavoritesAsync(GetUserId());
        return Ok(favorites);
    }

    [HttpGet("check")]
    public async Task<IActionResult> Check([FromQuery] Guid? serviceId, [FromQuery] Guid? handymanId)
    {
        var isFavorited = await _favoriteService.IsFavoritedAsync(GetUserId(), serviceId, handymanId);
        return Ok(new { isFavorited });
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public class AddFavoriteDto
{
    public Guid? ServiceId { get; set; }
    public Guid? HandymanProfileId { get; set; }
}
