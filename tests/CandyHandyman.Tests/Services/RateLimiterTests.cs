using CandyHandyman.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace CandyHandyman.Tests.Services;

public class RateLimiterTests
{
    private readonly InMemoryRateLimiter _limiter = new();

    [Fact]
    public void IsAllowed_ShouldReturnTrue_WhenUnderLimit()
    {
        var result = _limiter.IsAllowed("test-key", 5, 60);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsAllowed_ShouldReturnFalse_WhenOverLimit()
    {
        for (int i = 0; i < 5; i++)
        {
            _limiter.IsAllowed("test-key", 5, 60);
        }

        var result = _limiter.IsAllowed("test-key", 5, 60);

        result.Should().BeFalse();
    }

    [Fact]
    public void GetRemainingRequests_ShouldReturnCorrectCount()
    {
        _limiter.IsAllowed("test-key", 5, 60);
        _limiter.IsAllowed("test-key", 5, 60);

        var remaining = _limiter.GetRemainingRequests("test-key", 5, 60);

        remaining.Should().Be(3);
    }

    [Fact]
    public void IsAllowed_ShouldAllowDifferentKeysIndependently()
    {
        for (int i = 0; i < 5; i++)
        {
            _limiter.IsAllowed("key1", 5, 60);
        }

        var result = _limiter.IsAllowed("key2", 5, 60);

        result.Should().BeTrue();
    }
}
