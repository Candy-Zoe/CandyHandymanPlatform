using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class FeedbackService : IFeedbackService
{
    private readonly AppDbContext _context;

    public FeedbackService(AppDbContext context) => _context = context;

    public async Task<FeedbackDto> SubmitAsync(Guid userId, SubmitFeedbackDto dto)
    {
        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Content = dto.Content,
            ContactInfo = dto.ContactInfo,
            Type = Enum.Parse<FeedbackType>(dto.Type, true),
            Status = FeedbackStatus.Pending
        };
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();
        return MapToDto(feedback);
    }

    public async Task<List<FeedbackDto>> GetUserFeedbacksAsync(Guid userId)
    {
        return await _context.Feedbacks
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => MapToDto(f))
            .ToListAsync();
    }

    public async Task<List<FeedbackDto>> GetAllFeedbacksAsync(int page = 1, int pageSize = 20)
    {
        return await _context.Feedbacks
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => MapToDto(f))
            .ToListAsync();
    }

    public async Task ReplyAsync(Guid feedbackId, string reply)
    {
        var f = await _context.Feedbacks.FindAsync(feedbackId);
        if (f == null) return;
        f.AdminReply = reply;
        f.RepliedAt = DateTime.UtcNow;
        f.Status = FeedbackStatus.Resolved;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(Guid feedbackId, string status)
    {
        var f = await _context.Feedbacks.FindAsync(feedbackId);
        if (f == null) return;
        f.Status = Enum.Parse<FeedbackStatus>(status, true);
        await _context.SaveChangesAsync();
    }

    private static FeedbackDto MapToDto(Feedback f) => new()
    {
        Id = f.Id,
        Content = f.Content,
        ContactInfo = f.ContactInfo,
        Type = f.Type.ToString(),
        Status = f.Status.ToString(),
        AdminReply = f.AdminReply,
        RepliedAt = f.RepliedAt,
        CreatedAt = f.CreatedAt
    };
}
