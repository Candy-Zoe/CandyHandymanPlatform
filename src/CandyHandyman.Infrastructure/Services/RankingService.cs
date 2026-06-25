using CandyHandyman.Application.Interfaces;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class RankingService : IRankingService
{
    private readonly AppDbContext _context;

    public RankingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HandymanRankingDto>> GetRankingAsync(Guid? categoryId, int top = 20)
    {
        var query = _context.HandymanProfiles
            .Include(h => h.User)
            .Where(h => h.IsAvailable);

        if (categoryId.HasValue)
        {
            query = query.Where(h => h.Categories.Any(c => c.Id == categoryId));
        }

        var handymen = await query
            .OrderByDescending(h => h.AverageRating)
            .ThenByDescending(h => h.TotalCompletedOrders)
            .Take(top)
            .Select(h => new HandymanRankingDto
            {
                HandymanId = h.Id,
                Nickname = h.User.Nickname,
                Avatar = h.User.Avatar,
                Level = h.Level.ToString(),
                AverageRating = h.AverageRating,
                TotalCompletedOrders = h.TotalCompletedOrders,
                TotalReviews = h.TotalReviews
            })
            .ToListAsync();

        return handymen;
    }
}
