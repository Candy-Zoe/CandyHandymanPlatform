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
}