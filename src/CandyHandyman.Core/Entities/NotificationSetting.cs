using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class NotificationSetting : BaseEntity
{
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public bool Enabled { get; set; } = true;

    public User User { get; set; } = null!;
}
