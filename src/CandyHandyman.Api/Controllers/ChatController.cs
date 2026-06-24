using System.Security.Claims;
using CandyHandyman.Application.DTOs;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IRepository<Conversation> _conversationRepo;
    private readonly IRepository<Message> _messageRepo;
    private readonly IRepository<User> _userRepo;

    public ChatController(
        IRepository<Conversation> conversationRepo,
        IRepository<Message> messageRepo,
        IRepository<User> userRepo)
    {
        _conversationRepo = conversationRepo;
        _messageRepo = messageRepo;
        _userRepo = userRepo;
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationDto>>> GetConversations()
    {
        var userId = GetUserId();
        var conversations = (await _conversationRepo.GetAllAsync())
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .ToList();

        var result = new List<ConversationDto>();
        foreach (var conv in conversations)
        {
            var otherUserId = conv.User1Id == userId ? conv.User2Id : conv.User1Id;
            var otherUser = await _userRepo.GetByIdAsync(otherUserId);
            var messages = (await _messageRepo.GetAllAsync())
                .Where(m => m.ConversationId == conv.Id)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();
            var lastMsg = messages.FirstOrDefault();
            var unreadCount = messages.Count(m => m.SenderId != userId && !m.IsRead);

            result.Add(new ConversationDto
            {
                Id = conv.Id,
                OtherUser = otherUser != null ? new UserDto
                {
                    Id = otherUser.Id,
                    Nickname = otherUser.Nickname,
                    Avatar = otherUser.Avatar,
                    Phone = otherUser.Phone,
                    Role = otherUser.Role.ToString(),
                    Balance = otherUser.Balance
                } : null,
                LastMessage = lastMsg != null ? new MessageDto
                {
                    Id = lastMsg.Id,
                    SenderId = lastMsg.SenderId,
                    Content = lastMsg.Content,
                    MessageType = lastMsg.MessageType,
                    CreatedAt = lastMsg.CreatedAt
                } : null,
                UnreadCount = unreadCount,
                LastMessageAt = conv.LastMessageAt
            });
        }

        return Ok(result);
    }

    [HttpGet("conversations/{id}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var allMessages = (await _messageRepo.GetAllAsync())
            .Where(m => m.ConversationId == id)
            .OrderBy(m => m.CreatedAt)
            .ToList();

        var users = (await _userRepo.GetAllAsync()).ToDictionary(u => u.Id);

        var paged = allMessages
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(paged.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderName = users.TryGetValue(m.SenderId, out var sender) ? sender.Nickname : "未知",
            SenderAvatar = sender?.Avatar,
            Content = m.Content,
            MessageType = m.MessageType,
            IsRead = m.IsRead,
            CreatedAt = m.CreatedAt
        }).ToList());
    }

    [HttpPost("messages")]
    public async Task<ActionResult<MessageDto>> SendMessage(SendMessageDto dto)
    {
        var userId = GetUserId();
        var conversations = await _conversationRepo.GetAllAsync();
        var conversation = conversations.FirstOrDefault(c =>
            (c.User1Id == userId && c.User2Id == dto.ReceiverId) ||
            (c.User1Id == dto.ReceiverId && c.User2Id == userId));

        if (conversation == null)
        {
            conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                User1Id = userId,
                User2Id = dto.ReceiverId,
                LastMessageAt = DateTime.UtcNow
            };
            await _conversationRepo.AddAsync(conversation);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderId = userId,
            Content = dto.Content,
            MessageType = dto.MessageType
        };

        await _messageRepo.AddAsync(message);

        conversation.LastMessageAt = DateTime.UtcNow;
        await _conversationRepo.UpdateAsync(conversation);

        var sender = await _userRepo.GetByIdAsync(userId);
        return Ok(new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderName = sender?.Nickname ?? "",
            SenderAvatar = sender?.Avatar,
            Content = message.Content,
            MessageType = message.MessageType,
            IsRead = false,
            CreatedAt = message.CreatedAt
        });
    }

    [HttpPut("messages/read")]
    public async Task<IActionResult> MarkAsRead([FromQuery] Guid conversationId)
    {
        var userId = GetUserId();
        var messages = (await _messageRepo.GetAllAsync())
            .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead)
            .ToList();

        foreach (var msg in messages)
        {
            msg.IsRead = true;
            await _messageRepo.UpdateAsync(msg);
        }

        return Ok();
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}