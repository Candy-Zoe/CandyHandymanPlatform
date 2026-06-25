using CandyHandyman.Infrastructure.Services;
using CandyHandyman.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace CandyHandyman.Tests.Services;

public class PasswordServiceTests
{
    private readonly PasswordService _service = new();

    [Fact]
    public void HashPassword_ShouldReturnDifferentHashes()
    {
        var hash1 = _service.HashPassword("Password123!");
        var hash2 = _service.HashPassword("Password123!");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatches()
    {
        var password = "Password123!";
        var hash = _service.HashPassword(password);

        var result = _service.VerifyPassword(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatch()
    {
        var hash = _service.HashPassword("Password123!");

        var result = _service.VerifyPassword("WrongPassword123!", hash);

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("Password123!", true)]
    [InlineData("Ab1!", false)]
    [InlineData("password123!", false)]
    [InlineData("PASSWORD123!", false)]
    [InlineData("Password123", false)]
    [InlineData("", false)]
    public void ValidatePassword_ShouldReturnExpectedResult(string password, bool expectedValid)
    {
        var result = _service.ValidatePassword(password);

        result.IsValid.Should().Be(expectedValid);
    }
}
