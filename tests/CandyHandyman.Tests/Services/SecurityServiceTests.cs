using CandyHandyman.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace CandyHandyman.Tests.Services;

public class SecurityServiceTests
{
    private readonly SecurityService _service = new();

    [Fact]
    public void SanitizeInput_ShouldRemoveHtmlTags()
    {
        var input = "<script>alert('xss')</script>Hello";

        var result = _service.SanitizeInput(input);

        result.Should().NotContain("<script>");
        result.Should().Contain("Hello");
    }

    [Fact]
    public void SanitizeInput_ShouldRemoveAndEncodeHtml()
    {
        var input = "Hello <b>World</b>";

        var result = _service.SanitizeInput(input);

        result.Should().NotContain("<b>");
        result.Should().Contain("World");
    }

    [Fact]
    public void ContainsSensitiveWords_ShouldDetectSensitiveContent()
    {
        var text = "这是一个赌博网站";

        var result = _service.ContainsSensitiveWords(text, out var foundWords);

        result.Should().BeTrue();
        foundWords.Should().Contain("赌博");
    }

    [Fact]
    public void ContainsSensitiveWords_ShouldReturnFalse_ForCleanText()
    {
        var text = "这是一个正常的维修服务";

        var result = _service.ContainsSensitiveWords(text, out _);

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("13800138000", true)]
    [InlineData("12345678901", false)]
    [InlineData("1380013800", false)]
    [InlineData("", false)]
    public void IsValidPhone_ShouldReturnExpectedResult(string phone, bool expected)
    {
        var result = _service.IsValidPhone(phone);

        result.Should().Be(expected);
    }

    [Fact]
    public void MaskSensitiveInfo_ShouldMaskPhoneNumber()
    {
        var text = "联系电话：13812345678";

        var result = _service.MaskSensitiveInfo(text);

        result.Should().Contain("138****5678");
    }
}
