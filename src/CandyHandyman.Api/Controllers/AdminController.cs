using System.Security.Claims;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<Service> _serviceRepo;
    private readonly IRepository<Review> _reviewRepo;

    public AdminController(
        IRepository<User> userRepo,
        IRepository<Order> orderRepo,
        IRepository<Service> serviceRepo,
        IRepository<Review> reviewRepo)
    {
        _userRepo = userRepo;
        _orderRepo = orderRepo;
        _serviceRepo = serviceRepo;
        _reviewRepo = reviewRepo;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var users = (await _userRepo.GetAllAsync()).ToList();
        var orders = (await _orderRepo.GetAllAsync()).ToList();
        var services = (await _serviceRepo.GetAllAsync()).ToList();

        return Ok(new
        {
            totalUsers = users.Count,
            totalOrders = orders.Count,
            totalServices = services.Count,
            totalRevenue = orders.Where(o => o.Status == Core.Enums.OrderStatus.Completed).Sum(o => o.TotalAmount),
            pendingDisputes = orders.Count(o => o.Status == Core.Enums.OrderStatus.Disputed)
        });
    }

    [HttpGet("users")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var users = (await _userRepo.GetAllAsync())
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new
            {
                u.Id,
                u.Nickname,
                u.Phone,
                Role = u.Role.ToString(),
                u.Balance,
                u.CreatedAt
            })
            .ToList();

        return Ok(users);
    }

    [HttpGet("orders")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var orders = (await _orderRepo.GetAllAsync())
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new
            {
                o.Id,
                o.OrderNo,
                o.TotalAmount,
                Status = o.Status.ToString(),
                o.Address,
                o.CreatedAt
            })
            .ToList();

        return Ok(orders);
    }

    [HttpGet("stats/daily")]
    public async Task<IActionResult> GetDailyStats([FromQuery] int days = 7)
    {
        var orders = (await _orderRepo.GetAllAsync()).ToList();
        var startDate = DateTime.UtcNow.Date.AddDays(-days);

        var dailyStats = Enumerable.Range(0, days + 1)
            .Select(i => startDate.AddDays(i))
            .Select(date => new
            {
                Date = date.ToString("yyyy-MM-dd"),
                OrderCount = orders.Count(o => o.CreatedAt.Date == date),
                Revenue = orders.Where(o => o.CreatedAt.Date == date && o.Status == Core.Enums.OrderStatus.Completed).Sum(o => o.TotalAmount)
            })
            .ToList();

        return Ok(dailyStats);
    }

    [HttpGet("stats/overview")]
    public async Task<IActionResult> GetOverviewStats()
    {
        var users = (await _userRepo.GetAllAsync()).ToList();
        var orders = (await _orderRepo.GetAllAsync()).ToList();
        var services = (await _serviceRepo.GetAllAsync()).ToList();
        var reviews = (await _reviewRepo.GetAllAsync()).ToList();

        var today = DateTime.UtcNow.Date;
        var thisWeek = DateTime.UtcNow.AddDays(-7);
        var thisMonth = DateTime.UtcNow.AddMonths(-1);

        return Ok(new
        {
            totalUsers = users.Count,
            newUsersToday = users.Count(u => u.CreatedAt.Date == today),
            newUsersThisWeek = users.Count(u => u.CreatedAt >= thisWeek),
            totalOrders = orders.Count,
            ordersToday = orders.Count(o => o.CreatedAt.Date == today),
            completedOrders = orders.Count(o => o.Status == Core.Enums.OrderStatus.Completed),
            totalRevenue = orders.Where(o => o.Status == Core.Enums.OrderStatus.Completed).Sum(o => o.TotalAmount),
            revenueToday = orders.Where(o => o.Status == Core.Enums.OrderStatus.Completed && o.CreatedAt.Date == today).Sum(o => o.TotalAmount),
            totalServices = services.Count,
            totalReviews = reviews.Count,
            averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
            pendingDisputes = orders.Count(o => o.Status == Core.Enums.OrderStatus.Disputed)
        });
    }

    [HttpGet("stats/top-services")]
    public async Task<IActionResult> GetTopServices([FromQuery] int top = 10)
    {
        var orders = (await _orderRepo.GetAllAsync()).ToList();
        var services = (await _serviceRepo.GetAllAsync()).ToDictionary(s => s.Id);

        var topServices = orders
            .Where(o => o.Status == Core.Enums.OrderStatus.Completed)
            .GroupBy(o => o.ServiceId)
            .OrderByDescending(g => g.Count())
            .Take(top)
            .Select(g => new
            {
                ServiceId = g.Key,
                Title = services.TryGetValue(g.Key, out var s) ? s.Title : "Unknown",
                OrderCount = g.Count(),
                TotalRevenue = g.Sum(o => o.TotalAmount)
            })
            .ToList();

        return Ok(topServices);
    }
}