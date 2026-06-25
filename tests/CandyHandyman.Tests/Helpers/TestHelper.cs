using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Tests.Helpers;

public static class TestHelper
{
    public static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    public static void SeedTestData(AppDbContext context)
    {
        var userId = Guid.NewGuid();
        var handymanId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();

        context.Users.Add(new Core.Entities.User
        {
            Id = userId,
            Nickname = "测试用户",
            Phone = "13800138000",
            PasswordHash = "test.hash",
            Role = Core.Enums.UserRole.Customer,
            Balance = 1000
        });

        context.SkillCategories.Add(new Core.Entities.SkillCategory
        {
            Id = categoryId,
            Name = "水电维修"
        });

        context.HandymanProfiles.Add(new Core.Entities.HandymanProfile
        {
            Id = handymanId,
            UserId = userId,
            YearsOfExperience = 5,
            AverageRating = 4.5m,
            TotalReviews = 10,
            TotalCompletedOrders = 20,
            Level = Core.Enums.CraftsmanLevel.Intermediate,
            IsVerified = true,
            IsAvailable = true
        });

        context.Services.Add(new Core.Entities.Service
        {
            Id = serviceId,
            HandymanProfileId = handymanId,
            CategoryId = categoryId,
            Title = "水电维修服务",
            Description = "专业水电维修",
            Price = 100,
            Unit = "次",
            Status = Core.Enums.ServiceStatus.Active
        });

        context.SaveChanges();
    }
}
