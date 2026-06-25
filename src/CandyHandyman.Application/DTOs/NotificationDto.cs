using CandyHandyman.Core.Enums;

namespace CandyHandyman.Application.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public Guid? RelatedId { get; set; }
    public string? RelatedType { get; set; }
    public string? Image { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationSettingDto
{
    public NotificationType Type { get; set; }
    public bool Enabled { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
