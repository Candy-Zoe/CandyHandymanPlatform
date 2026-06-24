using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Users.Any()) return;

        var adminId = Guid.NewGuid();
        var admin = new User
        {
            Id = adminId,
            Nickname = "管理员",
            Phone = "13800000000",
            PasswordHash = HashPassword("admin123"),
            Role = UserRole.Admin
        };

        var categories = new List<SkillCategory>
        {
            new() { Id = Guid.NewGuid(), Name = "水电维修", Icon = "🔧", SortOrder = 1 },
            new() { Id = Guid.NewGuid(), Name = "木工家具", Icon = "🪚", SortOrder = 2 },
            new() { Id = Guid.NewGuid(), Name = "墙面粉刷", Icon = "🎨", SortOrder = 3 },
            new() { Id = Guid.NewGuid(), Name = "管道疏通", Icon = "🚿", SortOrder = 4 },
            new() { Id = Guid.NewGuid(), Name = "家电维修", Icon = "🔌", SortOrder = 5 },
            new() { Id = Guid.NewGuid(), Name = "门窗安装", Icon = "🚪", SortOrder = 6 },
            new() { Id = Guid.NewGuid(), Name = "搬家服务", Icon = "📦", SortOrder = 7 },
            new() { Id = Guid.NewGuid(), Name = "清洁保洁", Icon = "🧹", SortOrder = 8 }
        };

        context.Users.Add(admin);
        context.SkillCategories.AddRange(categories);
        context.SaveChanges();
    }

    private static string HashPassword(string password)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA256();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }
}