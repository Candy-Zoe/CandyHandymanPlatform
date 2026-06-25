using CandyHandyman.Core.Enums;

namespace CandyHandyman.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string title, string content, NotificationType type, Guid? relatedId = null, string? relatedType = null);
    Task SendBatchNotificationAsync(List<Guid> userIds, string title, string content, NotificationType type, Guid? relatedId = null, string? relatedType = null);
    Task SendPushAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null);
    Task SendWechatTemplateAsync(string openid, string templateId, Dictionary<string, string> data);
}
