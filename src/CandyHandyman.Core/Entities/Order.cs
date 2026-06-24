using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class Order : BaseEntity
{
    public string OrderNo { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ProviderId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string Address { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Service Service { get; set; } = null!;
    public User Customer { get; set; } = null!;
    public User Provider { get; set; } = null!;
    public Review? Review { get; set; }
    public Payment? Payment { get; set; }
}