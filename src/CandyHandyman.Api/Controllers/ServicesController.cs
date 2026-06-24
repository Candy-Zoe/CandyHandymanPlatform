using System.Security.Claims;
using CandyHandyman.Application.DTOs;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IRepository<Service> _serviceRepo;
    private readonly IRepository<HandymanProfile> _handymanRepo;
    private readonly IRepository<ServiceMedia> _mediaRepo;
    private readonly IRepository<SkillCategory> _categoryRepo;

    public ServicesController(
        IRepository<Service> serviceRepo,
        IRepository<HandymanProfile> handymanRepo,
        IRepository<ServiceMedia> mediaRepo,
        IRepository<SkillCategory> categoryRepo)
    {
        _serviceRepo = serviceRepo;
        _handymanRepo = handymanRepo;
        _mediaRepo = mediaRepo;
        _categoryRepo = categoryRepo;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ServiceDto>>> GetAll(
        [FromQuery] Guid? categoryId,
        [FromQuery] string? keyword,
        [FromQuery] PricingType? pricingType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var services = (await _serviceRepo.GetAllAsync()).Where(s => s.Status == ServiceStatus.Active).AsQueryable();

        if (categoryId.HasValue)
            services = services.Where(s => s.CategoryId == categoryId);
        if (pricingType.HasValue)
            services = services.Where(s => s.PricingType == pricingType);
        if (!string.IsNullOrEmpty(keyword))
            services = services.Where(s => s.Title.Contains(keyword) || s.Description.Contains(keyword));

        var result = services.OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var handymen = (await _handymanRepo.GetAllAsync()).ToDictionary(h => h.Id);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);

        return Ok(result.Select(s => MapToDto(s, handymen, categories)).ToList());
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceDto>> GetById(Guid id)
    {
        var service = await _serviceRepo.GetByIdAsync(id);
        if (service == null) return NotFound();

        service.ViewCount++;
        await _serviceRepo.UpdateAsync(service);

        var handymen = (await _handymanRepo.GetAllAsync()).ToDictionary(h => h.Id);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);

        return Ok(MapToDto(service, handymen, categories));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create(CreateServiceDto dto)
    {
        var userId = GetUserId();
        var handymen = (await _handymanRepo.GetAllAsync()).ToList();
        var profile = handymen.FirstOrDefault(h => h.UserId == userId);
        if (profile == null) return BadRequest(new { message = "请先注册为工匠" });

        var service = new Service
        {
            Id = Guid.NewGuid(),
            HandymanProfileId = profile.Id,
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            Description = dto.Description,
            PricingType = dto.PricingType,
            Price = dto.Price,
            Unit = dto.Unit
        };

        await _serviceRepo.AddAsync(service);
        return Ok(MapToDto(service, handymen.ToDictionary(h => h.Id), new()));
    }

    [HttpPost("{id}/media")]
    public async Task<IActionResult> UploadMedia(Guid id, [FromForm] IFormFile file, [FromForm] MediaType mediaType)
    {
        var service = await _serviceRepo.GetByIdAsync(id);
        if (service == null) return NotFound();

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine("uploads", fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var media = new ServiceMedia
        {
            Id = Guid.NewGuid(),
            ServiceId = id,
            MediaType = mediaType,
            MediaUrl = $"/uploads/{fileName}"
        };

        await _mediaRepo.AddAsync(media);
        return Ok(new { media.Id, media.MediaUrl });
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static ServiceDto MapToDto(Service s, Dictionary<Guid, HandymanProfile> handymen, Dictionary<Guid, SkillCategory> categories)
    {
        HandymanDto? handymanDto = null;
        if (handymen.TryGetValue(s.HandymanProfileId, out var hp))
        {
            handymanDto = new HandymanDto
            {
                Id = hp.Id,
                Nickname = hp.User?.Nickname ?? "",
                Avatar = hp.User?.Avatar,
                YearsOfExperience = hp.YearsOfExperience,
                AverageRating = hp.AverageRating,
                TotalReviews = hp.TotalReviews,
                IsVerified = hp.IsVerified
            };
        }

        var categoryName = "";
        if (categories.TryGetValue(s.CategoryId, out var cat))
        {
            categoryName = cat.Name;
        }

        return new ServiceDto
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            PricingType = s.PricingType,
            Price = s.Price,
            Unit = s.Unit,
            Status = s.Status,
            ViewCount = s.ViewCount,
            CategoryId = s.CategoryId,
            CategoryName = categoryName,
            Handyman = handymanDto!,
            CreatedAt = s.CreatedAt,
            Media = s.Media?.Select(m => new ServiceMediaDto
            {
                Id = m.Id,
                MediaType = m.MediaType,
                MediaUrl = m.MediaUrl,
                CoverUrl = m.CoverUrl,
                SortOrder = m.SortOrder
            }).ToList() ?? new()
        };
    }
}