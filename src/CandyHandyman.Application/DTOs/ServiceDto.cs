using CandyHandyman.Core.Enums;

namespace CandyHandyman.Application.DTOs;

public class ServiceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PricingType PricingType { get; set; }
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty;
    public ServiceStatus Status { get; set; }
    public int ViewCount { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public HandymanDto Handyman { get; set; } = null!;
    public List<ServiceMediaDto> Media { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ServiceMediaDto
{
    public Guid Id { get; set; }
    public MediaType MediaType { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public int SortOrder { get; set; }
}

public class CreateServiceDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public PricingType PricingType { get; set; }
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class HandymanDto
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public int YearsOfExperience { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsVerified { get; set; }
}

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
    public List<CategoryDto> Children { get; set; } = new();
}