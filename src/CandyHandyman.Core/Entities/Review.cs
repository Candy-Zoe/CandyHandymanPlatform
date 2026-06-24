namespace CandyHandyman.Core.Entities;

public class Review : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ProviderId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; } = string.Empty;

    public Order Order { get; set; } = null!;
    public User Customer { get; set; } = null!;
    public User Provider { get; set; } = null!;
}