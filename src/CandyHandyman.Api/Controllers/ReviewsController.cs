using System.Security.Claims;
using CandyHandyman.Application.DTOs;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IRepository<Review> _reviewRepo;
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<User> _userRepo;
    private readonly INotificationService _notificationService;
    private readonly ILevelService _levelService;
    private readonly IRepository<HandymanProfile> _handymanRepo;

    public ReviewsController(
        IRepository<Review> reviewRepo,
        IRepository<Order> orderRepo,
        IRepository<User> userRepo,
        INotificationService notificationService,
        ILevelService levelService,
        IRepository<HandymanProfile> handymanRepo)
    {
        _reviewRepo = reviewRepo;
        _orderRepo = orderRepo;
        _userRepo = userRepo;
        _notificationService = notificationService;
        _levelService = levelService;
        _handymanRepo = handymanRepo;
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create(CreateReviewDto dto)
    {
        var userId = GetUserId();
        var order = await _orderRepo.GetByIdAsync(dto.OrderId);
        if (order == null) return NotFound(new { message = "订单不存在" });
        if (order.CustomerId != userId) return Forbid();
        if (order.Status != Core.Enums.OrderStatus.Completed) return BadRequest(new { message = "订单未完成，无法评价" });

        var existing = (await _reviewRepo.GetAllAsync()).FirstOrDefault(r => r.OrderId == dto.OrderId);
        if (existing != null) return BadRequest(new { message = "已评价" });

        var review = new Review
        {
            Id = Guid.NewGuid(),
            OrderId = dto.OrderId,
            CustomerId = userId,
            ProviderId = order.ProviderId,
            Rating = dto.Rating,
            Content = dto.Content
        };

        await _reviewRepo.AddAsync(review);

        var profile = (await _handymanRepo.GetAllAsync()).FirstOrDefault(h => h.UserId == order.ProviderId);
        if (profile != null)
        {
            var allReviews = (await _reviewRepo.GetAllAsync()).Where(r => r.ProviderId == order.ProviderId).ToList();
            profile.TotalReviews = allReviews.Count;
            profile.AverageRating = allReviews.Any() ? (decimal)allReviews.Average(r => r.Rating) : 0;
            await _handymanRepo.UpdateAsync(profile);
            await _levelService.EvaluateAndUpgradeAsync(profile.Id);
        }

        await _notificationService.SendNotificationAsync(
            order.ProviderId,
            "收到新评价",
            $"您收到一个{dto.Rating}星评价",
            NotificationType.Review,
            review.Id,
            "Review");

        return Ok(new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Content = review.Content,
            CreatedAt = review.CreatedAt
        });
    }

    [HttpGet("handyman/{handymanId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReviewDto>>> GetByHandyman(Guid handymanId)
    {
        var reviews = (await _reviewRepo.GetAllAsync())
            .Where(r => r.ProviderId == handymanId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        var users = (await _userRepo.GetAllAsync()).ToDictionary(u => u.Id);

        return Ok(reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            Rating = r.Rating,
            Content = r.Content,
            Customer = users.TryGetValue(r.CustomerId, out var customer) ? new UserDto
            {
                Id = customer.Id,
                Nickname = customer.Nickname,
                Avatar = customer.Avatar,
                Phone = customer.Phone,
                Role = customer.Role.ToString(),
                Balance = customer.Balance
            } : null,
            CreatedAt = r.CreatedAt
        }).ToList());
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}