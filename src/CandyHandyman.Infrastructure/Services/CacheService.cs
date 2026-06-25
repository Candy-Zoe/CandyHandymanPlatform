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
    private readonly HashSet<string> _keys = new();
    private readonly object _lock = new();

    public CacheService(IMemoryCache cache) => _cache = cache;

    public T? Get<T>(string key)
    {
        return _cache.TryGetValue(key, out T? value) ? value : default;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5),
            Size = 1
        };

        _cache.Set(key, value, options);

        lock (_lock)
        {
            _keys.Add(key);
        }
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        lock (_lock)
        {
            _keys.Remove(key);
        }
    }

    public void RemoveByPrefix(string prefix)
    {
        lock (_lock)
        {
            var keysToRemove = _keys.Where(k => k.StartsWith(prefix)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _keys.Remove(key);
            }
        }
    }
}
