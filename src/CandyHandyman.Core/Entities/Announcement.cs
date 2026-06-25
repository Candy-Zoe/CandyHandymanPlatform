using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class Announcement : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementType Type { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid? CreatedBy { get; set; }
}

public enum AnnouncementType
{
    System = 0,
    Promotion = 1,
    Maintenance = 2,
    Notice = 3
}
