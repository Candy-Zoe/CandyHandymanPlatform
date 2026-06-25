using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class FavoriteService : IFavoriteService
{
    private readonly AppDbContext _context;

    public FavoriteService(AppDbContext context) => _context = context;

    public async Task AddFavoriteAsync(Guid userId, Guid? serviceId, Guid? handymanId)
    {
        var existing = await _context.Favorites.FirstOrDefaultAsync(f =>
            f.UserId == userId && f.ServiceId == serviceId && f.HandymanProfileId == handymanId);
        if (existing != null) return;

        _context.Favorites.Add(new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ServiceId = serviceId,
            HandymanProfileId = handymanId
        });
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFavoriteAsync(Guid userId, Guid? serviceId, Guid? handymanId)
    {
        var fav = await _context.Favorites.FirstOrDefaultAsync(f =>
            f.UserId == userId && f.ServiceId == serviceId && f.HandymanProfileId == handymanId);
        if (fav == null) return;

        _context.Favorites.Remove(fav);
        await _context.SaveChangesAsync();
    }

    public async Task<List<FavoriteDto>> GetUserFavoritesAsync(Guid userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Service)
            .Include(f => f.HandymanProfile).ThenInclude(h => h!.User)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FavoriteDto
            {
                Id = f.Id,
                ServiceId = f.ServiceId,
                ServiceTitle = f.Service != null ? f.Service.Title : null,
                HandymanProfileId = f.HandymanProfileId,
                HandymanName = f.HandymanProfile != null ? f.HandymanProfile.User.Nickname : null,
                CreatedAt = f.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> IsFavoritedAsync(Guid userId, Guid? serviceId, Guid? handymanId)
    {
        return await _context.Favorites.AnyAsync(f =>
            f.UserId == userId && f.ServiceId == serviceId && f.HandymanProfileId == handymanId);
    }
}
