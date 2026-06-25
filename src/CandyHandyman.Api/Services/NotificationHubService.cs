using CandyHandyman.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CandyHandyman.Api.Services;

public class NotificationHubService : INotificationHub
{
    private readonly IHubContext<CandyHandyman.Api.Hubs.NotificationHub> _hubContext;

    public NotificationHubService(IHubContext<Hubs.NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(Guid userId, string method, object message)
    {
        await _hubContext.Clients.User(userId.ToString()).SendAsync(method, message);
    }
}
