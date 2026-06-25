using CandyHandyman.Core.Entities;
using CandyHandyman.Infrastructure.Data;

namespace CandyHandyman.Infrastructure.Services;

public interface IAuditService
{
    Task LogAsync(Guid? userId, string action, string? entityType = null, Guid? entityId = null, string? details = null, string? ipAddress = null);
}

public class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context) => _context = context;

    public async Task LogAsync(Guid? userId, string action, string? entityType = null, Guid? entityId = null, string? details = null, string? ipAddress = null)
    {
        var log = new OperationLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IpAddress = ipAddress
        };

        _context.OperationLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
