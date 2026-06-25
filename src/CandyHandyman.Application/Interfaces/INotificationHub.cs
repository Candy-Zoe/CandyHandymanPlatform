namespace CandyHandyman.Application.Interfaces;

public interface INotificationHub
{
    Task SendToUserAsync(Guid userId, string method, object message);
}
