using System.Text.RegularExpressions;

namespace CandyHandyman.Infrastructure.Services;

public interface ISecurityService
{
    string SanitizeInput(string input);
    bool ContainsSensitiveWords(string text, out List<string> foundWords);
    string MaskSensitiveInfo(string text);
    bool IsValidPhone(string phone);
    bool IsValidIdCard(string idCard);
}

public class SecurityService : ISecurityService
{
    private static readonly List<string> SensitiveWords = new()
    {
        "赌博", "色情", "毒品", "枪支", "诈骗", "传销",
        "暴力", "恐怖", "反动", "政治敏感词"
    };

    private static readonly Regex HtmlTagRegex = new(@"<[^>]+>", RegexOptions.Compiled);
    private static readonly Regex ScriptRegex = new(@"<script[^>]*>.*?</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex SqlInjectionRegex = new(@"(\b(DELETE|DROP|INSERT|UPDATE|EXEC|EXECUTE|UNION|SELECT)\b)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex PhoneRegex = new(@"^1[3-9]\d{9}$", RegexOptions.Compiled);
    private static readonly Regex IdCardRegex = new(@"^\d{17}[\dXx]$", RegexOptions.Compiled);

    public string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var result = ScriptRegex.Replace(input, "");
        result = HtmlTagRegex.Replace(result, "");
        result = System.Net.WebUtility.HtmlEncode(result);
        return result.Trim();
    }

    public bool ContainsSensitiveWords(string text, out List<string> foundWords)
    {
        foundWords = new List<string>();
        if (string.IsNullOrEmpty(text)) return false;

        foreach (var word in SensitiveWords)
        {
            if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
            {
                foundWords.Add(word);
            }
        }

        return foundWords.Count > 0;
    }

    public string MaskSensitiveInfo(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        text = Regex.Replace(text, @"(\d{3})\d{4}(\d{4})", "$1****$2");
        text = Regex.Replace(text, @"(\d{6})\d{8}(\d{4})", "$1********$2");
        return text;
    }

    public bool IsValidPhone(string phone)
    {
        return !string.IsNullOrEmpty(phone) && PhoneRegex.IsMatch(phone);
    }

    public bool IsValidIdCard(string idCard)
    {
        return !string.IsNullOrEmpty(idCard) && IdCardRegex.IsMatch(idCard);
    }
}
