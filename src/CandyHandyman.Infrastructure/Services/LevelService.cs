using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Enums;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class LevelService : ILevelService
{
    private readonly AppDbContext _context;

    public LevelService(AppDbContext context)
    {
        _context = context;
    }

    public async Task EvaluateAndUpgradeAsync(Guid handymanId)
    {
        var profile = await _context.HandymanProfiles.FindAsync(handymanId);
        if (profile == null) return;

        var currentLevel = profile.Level;
        var newLevel = CalculateLevel(profile.TotalCompletedOrders, (double)profile.AverageRating);

        if (newLevel > currentLevel)
        {
            profile.Level = newLevel;
            await _context.SaveChangesAsync();
        }
    }

    private static CraftsmanLevel CalculateLevel(int completedOrders, double averageRating)
    {
        if (completedOrders >= 200 && averageRating >= 4.8)
            return CraftsmanLevel.Expert;

        if (completedOrders >= 50 && averageRating >= 4.5)
            return CraftsmanLevel.Senior;

        if (completedOrders >= 10 && averageRating >= 4.0)
            return CraftsmanLevel.Intermediate;

        return CraftsmanLevel.Junior;
    }
}
