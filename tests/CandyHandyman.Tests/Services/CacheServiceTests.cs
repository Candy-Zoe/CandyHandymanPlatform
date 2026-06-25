using CandyHandyman.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace CandyHandyman.Tests.Services;

public class CacheServiceTests
{
    private readonly CacheService _service;

    public CacheServiceTests()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        _service = new CacheService(memoryCache);
    }

    [Fact]
    public void Set_ShouldStoreValue()
    {
        _service.Set("key1", "value1");

        var result = _service.Get<string>("key1");

        result.Should().Be("value1");
    }

    [Fact]
    public void Get_ShouldReturnDefault_WhenKeyNotExists()
    {
        var result = _service.Get<string>("nonexistent");

        result.Should().BeNull();
    }

    [Fact]
    public void Remove_ShouldDeleteValue()
    {
        _service.Set("key1", "value1");
        _service.Remove("key1");

        var result = _service.Get<string>("key1");

        result.Should().BeNull();
    }

    [Fact]
    public void RemoveByPrefix_ShouldDeleteMatchingKeys()
    {
        _service.Set("user:1", "user1");
        _service.Set("user:2", "user2");
        _service.Set("order:1", "order1");

        _service.RemoveByPrefix("user:");

        _service.Get<string>("user:1").Should().BeNull();
        _service.Get<string>("user:2").Should().BeNull();
        _service.Get<string>("order:1").Should().Be("order1");
    }
}
