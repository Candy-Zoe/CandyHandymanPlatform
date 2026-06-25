using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class WalletService : IWalletService
{
    private readonly AppDbContext _context;

    public WalletService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetBalanceAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.Balance ?? 0;
    }

    public async Task AddBalanceAsync(Guid userId, decimal amount, WalletTransactionType type, string? description = null, Guid? relatedId = null)
    {
        if (amount <= 0) throw new Exception("充值金额必须大于0");

        var affected = await _context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.Balance, u => u.Balance + amount));

        if (affected == 0) throw new Exception("用户不存在");

        var balance = await _context.Users.Where(u => u.Id == userId).Select(u => u.Balance).FirstAsync();

        _context.WalletTransactions.Add(new WalletTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount,
            Type = type,
            Balance = balance,
            Description = description,
            RelatedId = relatedId
        });
        await _context.SaveChangesAsync();
    }

    public async Task DeductBalanceAsync(Guid userId, decimal amount, WalletTransactionType type, string? description = null, Guid? relatedId = null)
    {
        if (amount <= 0) throw new Exception("扣减金额必须大于0");

        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("用户不存在");

        if (user.Balance < amount)
            throw new Exception("余额不足");

        var affected = await _context.Users
            .Where(u => u.Id == userId && u.Balance >= amount)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.Balance, u => u.Balance - amount));

        if (affected == 0) throw new Exception("余额不足或用户不存在");

        var balance = await _context.Users.Where(u => u.Id == userId).Select(u => u.Balance).FirstAsync();

        _context.WalletTransactions.Add(new WalletTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = -amount,
            Type = type,
            Balance = balance,
            Description = description,
            RelatedId = relatedId
        });
        await _context.SaveChangesAsync();
    }

    public async Task<List<WalletTransactionDto>> GetTransactionsAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        page = Math.Max(1, page);

        return await _context.WalletTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new WalletTransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                Balance = t.Balance,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();
    }
}
