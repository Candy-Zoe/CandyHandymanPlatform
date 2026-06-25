using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public Guid? RelatedId { get; set; }
    public string? RelatedType { get; set; }
    public string? Image { get; set; }

    public User User { get; set; } = null!;
}
