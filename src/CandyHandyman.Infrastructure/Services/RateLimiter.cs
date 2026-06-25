using System.Collections.Concurrent;

namespace CandyHandyman.Infrastructure.Services;

public interface IRateLimiter
{
    bool IsAllowed(string key, int maxRequests = 60, int windowSeconds = 60);
    int GetRemainingRequests(string key, int maxRequests = 60, int windowSeconds = 60);
}

public class InMemoryRateLimiter : IRateLimiter
{
    private readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();

    public bool IsAllowed(string key, int maxRequests = 60, int windowSeconds = 60)
    {
        var now = DateTime.UtcNow;
        var windowStart = now.AddSeconds(-windowSeconds);

        var timestamps = _requests.GetOrAdd(key, _ => new List<DateTime>());

        lock (timestamps)
        {
            timestamps.RemoveAll(t => t < windowStart);

            if (timestamps.Count >= maxRequests)
                return false;

            timestamps.Add(now);
            return true;
        }
    }

    public int GetRemainingRequests(string key, int maxRequests = 60, int windowSeconds = 60)
    {
        var now = DateTime.UtcNow;
        var windowStart = now.AddSeconds(-windowSeconds);

        if (!_requests.TryGetValue(key, out var timestamps))
            return maxRequests;

        lock (timestamps)
        {
            timestamps.RemoveAll(t => t < windowStart);
            return Math.Max(0, maxRequests - timestamps.Count);
        }
    }
}
