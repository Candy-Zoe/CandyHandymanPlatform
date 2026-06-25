using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class AnnouncementService : IAnnouncementService
{
    private readonly AppDbContext _context;

    public AnnouncementService(AppDbContext context) => _context = context;

    public async Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto dto)
    {
        if (!Enum.TryParse<AnnouncementType>(dto.Type, true, out var announcementType))
            announcementType = AnnouncementType.Notice;

        var announcement = new Announcement
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Content = dto.Content,
            Type = announcementType,
            ExpiresAt = dto.ExpiresAt
        };
        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();
        return MapToDto(announcement);
    }

    public async Task<List<AnnouncementDto>> GetPublishedAsync()
    {
        return await _context.Announcements
            .Where(a => a.IsPublished && (!a.ExpiresAt.HasValue || a.ExpiresAt > DateTime.UtcNow))
            .OrderByDescending(a => a.PublishedAt)
            .Select(a => MapToDto(a))
            .ToListAsync();
    }

    public async Task<List<AnnouncementDto>> GetAllAsync()
    {
        return await _context.Announcements
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => MapToDto(a))
            .ToListAsync();
    }

    public async Task UpdateAsync(Guid id, CreateAnnouncementDto dto)
    {
        var a = await _context.Announcements.FindAsync(id);
        if (a == null) return;
        a.Title = dto.Title;
        a.Content = dto.Content;
        if (Enum.TryParse<AnnouncementType>(dto.Type, true, out var type))
            a.Type = type;
        a.ExpiresAt = dto.ExpiresAt;
        await _context.SaveChangesAsync();
    }

    public async Task PublishAsync(Guid id)
    {
        var a = await _context.Announcements.FindAsync(id);
        if (a == null) return;
        a.IsPublished = true;
        a.PublishedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var a = await _context.Announcements.FindAsync(id);
        if (a == null) return;
        _context.Announcements.Remove(a);
        await _context.SaveChangesAsync();
    }

    private static AnnouncementDto MapToDto(Announcement a) => new()
    {
        Id = a.Id,
        Title = a.Title,
        Content = a.Content,
        Type = a.Type.ToString(),
        IsPublished = a.IsPublished,
        PublishedAt = a.PublishedAt,
        ExpiresAt = a.ExpiresAt,
        CreatedAt = a.CreatedAt
    };
}
