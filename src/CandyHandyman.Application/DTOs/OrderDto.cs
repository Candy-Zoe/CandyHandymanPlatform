using System.ComponentModel.DataAnnotations;
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
    [Required(ErrorMessage = "服务ID不能为空")]
    public Guid ServiceId { get; set; }

    [Range(1, 100, ErrorMessage = "数量需在1-100之间")]
    public int Quantity { get; set; } = 1;

    [Required(ErrorMessage = "地址不能为空")]
    [StringLength(200, ErrorMessage = "地址不能超过200个字符")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "联系电话不能为空")]
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "联系电话格式不正确")]
    public string ContactPhone { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "描述不能超过500个字符")]
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
    [Required(ErrorMessage = "订单ID不能为空")]
    public Guid OrderId { get; set; }

    [Range(1, 5, ErrorMessage = "评分需在1-5之间")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "评价内容不能为空")]
    [StringLength(1000, ErrorMessage = "评价内容不能超过1000个字符")]
    public string Content { get; set; } = string.Empty;
}
