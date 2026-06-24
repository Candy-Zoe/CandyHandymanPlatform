using CandyHandyman.Core.Enums;

namespace CandyHandyman.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNo { get; set; } = string.Empty;
    public ServiceDto? Service { get; set; }
    public HandymanDto? Provider { get; set; }
    public UserDto? Customer { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string Address { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ReviewDto? Review { get; set; }
}

public class CreateOrderDto
{
    public Guid ServiceId { get; set; }
    public int Quantity { get; set; } = 1;
    public string Address { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? ScheduledAt { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; } = string.Empty;
    public UserDto? Customer { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewDto
{
    public Guid OrderId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; } = string.Empty;
}