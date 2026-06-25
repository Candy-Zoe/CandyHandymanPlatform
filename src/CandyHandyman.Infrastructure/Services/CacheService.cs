using Microsoft.Extensions.Caching.Memory;

namespace CandyHandyman.Infrastructure.Services;

public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan? expiration = null);
    void Remove(string key);
    void RemoveByPrefix(string prefix);
}

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly Dictionary<string, DateTime> _keyExpiry = new();
    private readonly object _lock = new();
    private DateTime _lastPrune = DateTime.UtcNow;

    public CacheService(IMemoryCache cache) => _cache = cache;

    public T? Get<T>(string key)
    {
        return _cache.TryGetValue(key, out T? value) ? value : default;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var exp = expiration ?? TimeSpan.FromMinutes(5);
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = exp,
            Size = 1
        };

        options.RegisterPostEvictionCallback((evictedKey, _, _, _) =>
        {
            lock (_lock)
            {
                _keyExpiry.Remove(evictedKey.ToString()!);
            }
        });

        _cache.Set(key, value, options);

        lock (_lock)
        {
            _keyExpiry[key] = DateTime.UtcNow.Add(exp);
            PruneExpiredKeys();
        }
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        lock (_lock)
        {
            _keyExpiry.Remove(key);
        }
    }

    public void RemoveByPrefix(string prefix)
    {
        lock (_lock)
        {
            var keysToRemove = _keyExpiry.Keys.Where(k => k.StartsWith(prefix)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _keyExpiry.Remove(key);
            }
        }
    }

    private void PruneExpiredKeys()
    {
        if (DateTime.UtcNow - _lastPrune < TimeSpan.FromMinutes(1)) return;
        _lastPrune = DateTime.UtcNow;

        var now = DateTime.UtcNow;
        var expired = _keyExpiry.Where(kv => kv.Value < now).Select(kv => kv.Key).ToList();
        foreach (var key in expired)
        {
            _cache.Remove(key);
            _keyExpiry.Remove(key);
        }
    }
}
