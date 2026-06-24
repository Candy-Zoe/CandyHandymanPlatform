using System.Security.Claims;
using CandyHandyman.Application.DTOs;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IRepository<User> _userRepo;

    public UsersController(IRepository<User> userRepo) => _userRepo = userRepo;

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userId = GetUserId();
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return NotFound();
        return Ok(MapToDto(user));
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateMe(UpdateProfileDto dto)
    {
        var userId = GetUserId();
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return NotFound();

        if (dto.Nickname != null) user.Nickname = dto.Nickname;
        if (dto.Bio != null) user.Bio = dto.Bio;
        if (dto.Latitude.HasValue) user.Latitude = dto.Latitude;
        if (dto.Longitude.HasValue) user.Longitude = dto.Longitude;

        await _userRepo.UpdateAsync(user);
        return Ok(MapToDto(user));
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Nickname = user.Nickname,
        Phone = user.Phone,
        Avatar = user.Avatar,
        Role = user.Role.ToString(),
        Balance = user.Balance,
        Bio = user.Bio
    };
}