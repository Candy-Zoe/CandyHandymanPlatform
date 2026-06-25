using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class BrowsingHistoryService : IBrowsingHistoryService
{
    private readonly AppDbContext _context;

    public BrowsingHistoryService(AppDbContext context) => _context = context;

    public async Task RecordAsync(Guid userId, Guid? serviceId, Guid? handymanId)
    {
        var existing = await _context.BrowsingHistories.FirstOrDefaultAsync(h =>
            h.UserId == userId && h.ServiceId == serviceId && h.HandymanProfileId == handymanId);
        if (existing != null)
        {
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return;
        }

        _context.BrowsingHistories.Add(new BrowsingHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ServiceId = serviceId,
            HandymanProfileId = handymanId
        });
        await _context.SaveChangesAsync();
    }

    public async Task<List<BrowsingHistoryDto>> GetUserHistoryAsync(Guid userId, int limit = 50)
    {
        return await _context.BrowsingHistories
            .Where(h => h.UserId == userId)
            .Include(h => h.Service)
            .Include(h => h.HandymanProfile).ThenInclude(h => h!.User)
            .OrderByDescending(h => h.UpdatedAt)
            .Take(limit)
            .Select(h => new BrowsingHistoryDto
            {
                Id = h.Id,
                ServiceId = h.ServiceId,
                ServiceTitle = h.Service != null ? h.Service.Title : null,
                HandymanProfileId = h.HandymanProfileId,
                HandymanName = h.HandymanProfile != null ? h.HandymanProfile.User.Nickname : null,
                ViewedAt = h.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task ClearHistoryAsync(Guid userId)
    {
        var items = await _context.BrowsingHistories.Where(h => h.UserId == userId).ToListAsync();
        _context.BrowsingHistories.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}
