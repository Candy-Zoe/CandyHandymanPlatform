using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class HelpService : IHelpService
{
    private readonly AppDbContext _context;

    public HelpService(AppDbContext context) => _context = context;

    public async Task<List<HelpTopicDto>> GetAllAsync()
    {
        return await _context.HelpTopics
            .Where(h => h.IsActive)
            .OrderBy(h => h.SortOrder)
            .Select(h => MapToDto(h))
            .ToListAsync();
    }

    public async Task<List<HelpTopicDto>> GetByCategoryAsync(string category)
    {
        return await _context.HelpTopics
            .Where(h => h.IsActive && h.Category == category)
            .OrderBy(h => h.SortOrder)
            .Select(h => MapToDto(h))
            .ToListAsync();
    }

    public async Task<HelpTopicDto?> GetByIdAsync(Guid id)
    {
        var h = await _context.HelpTopics.FindAsync(id);
        if (h == null) return null;
        h.ViewCount++;
        await _context.SaveChangesAsync();
        return MapToDto(h);
    }

    public async Task<HelpTopicDto> CreateAsync(CreateHelpTopicDto dto)
    {
        var topic = new HelpTopic
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Content = dto.Content,
            Category = dto.Category,
            SortOrder = dto.SortOrder
        };
        _context.HelpTopics.Add(topic);
        await _context.SaveChangesAsync();
        return MapToDto(topic);
    }

    public async Task UpdateAsync(Guid id, CreateHelpTopicDto dto)
    {
        var h = await _context.HelpTopics.FindAsync(id);
        if (h == null) return;
        h.Title = dto.Title;
        h.Content = dto.Content;
        h.Category = dto.Category;
        h.SortOrder = dto.SortOrder;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var h = await _context.HelpTopics.FindAsync(id);
        if (h == null) return;
        _context.HelpTopics.Remove(h);
        await _context.SaveChangesAsync();
    }

    private static HelpTopicDto MapToDto(HelpTopic h) => new()
    {
        Id = h.Id,
        Title = h.Title,
        Content = h.Content,
        Category = h.Category,
        SortOrder = h.SortOrder,
        IsActive = h.IsActive,
        ViewCount = h.ViewCount
    };
}
