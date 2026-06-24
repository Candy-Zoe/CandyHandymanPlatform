using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NearbyController : ControllerBase
{
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<HandymanProfile> _handymanRepo;

    public NearbyController(IRepository<User> userRepo, IRepository<HandymanProfile> handymanRepo)
    {
        _userRepo = userRepo;
        _handymanRepo = handymanRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetNearbyHandymen(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10,
        [FromQuery] Guid? categoryId = null)
    {
        var handymen = (await _handymanRepo.GetAllAsync())
            .Where(h => h.User.Latitude.HasValue && h.User.Longitude.HasValue && h.IsAvailable)
            .ToList();

        var results = handymen
            .Select(h => new
            {
                h.Id,
                h.User.Nickname,
                h.User.Avatar,
                h.User.Latitude,
                h.User.Longitude,
                h.AverageRating,
                h.TotalReviews,
                h.IsVerified,
                Distance = CalculateDistance(latitude, longitude, h.User.Latitude!.Value, h.User.Longitude!.Value)
            })
            .Where(h => h.Distance <= radiusKm)
            .OrderBy(h => h.Distance)
            .Take(50)
            .ToList();

        return Ok(results);
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double deg) => deg * Math.PI / 180;
}