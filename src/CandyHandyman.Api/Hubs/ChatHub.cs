using System.Security.Claims;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using CandyHandyman.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CandyHandyman.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private static readonly Dictionary<string, string> _onlineUsers = new();
    private readonly AppDbContext _db;

    public ChatHub(AppDbContext db)
    {
        _db = db;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            _onlineUsers[userId] = Context.ConnectionId;
            await Clients.All.SendAsync("UserOnline", userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null && _onlineUsers.ContainsKey(userId))
        {
            _onlineUsers.Remove(userId);
            await Clients.All.SendAsync("UserOffline", userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task SendMessage(string conversationId, string receiverId, string content, string messageType = "Text")
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (senderId == null) return;

        var conversation = await _db.Conversations.FindAsync(Guid.Parse(conversationId));
        if (conversation == null) return;

        var senderGuid = Guid.Parse(senderId);
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = Guid.Parse(conversationId),
            SenderId = senderGuid,
            Content = content,
            MessageType = Enum.Parse<MessageType>(messageType, true),
            IsRead = false
        };

        _db.Messages.Add(message);
        conversation.LastMessageAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var dto = new
        {
            Id = message.Id.ToString(),
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            MessageType = messageType,
            CreatedAt = message.CreatedAt.ToString("o")
        };

        await Clients.Group(conversationId).SendAsync("ReceiveMessage", dto);

        if (_onlineUsers.TryGetValue(receiverId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("NewMessageNotification", dto);
        }
    }

    public async Task Typing(string conversationId)
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (senderId == null) return;
        await Clients.Group(conversationId).SendAsync("UserTyping", senderId);
    }

    public async Task MarkAsRead(string conversationId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return;

        var senderGuid = Guid.Parse(userId);
        var convGuid = Guid.Parse(conversationId);
        var unread = _db.Messages
            .Where(m => m.ConversationId == convGuid && m.SenderId != senderGuid && !m.IsRead)
            .ToList();

        foreach (var msg in unread)
        {
            msg.IsRead = true;
        }
        await _db.SaveChangesAsync();

        await Clients.Group(conversationId).SendAsync("MessagesRead", userId);
    }

    public async Task SendLocationNotification(string receiverId, double latitude, double longitude)
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (senderId == null) return;

        if (_onlineUsers.TryGetValue(receiverId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("LocationUpdate", new
            {
                SenderId = senderId,
                Latitude = latitude,
                Longitude = longitude
            });
        }
    }
}