using System.Text.RegularExpressions;

namespace CandyHandyman.Infrastructure.Services;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string storedHash);
    PasswordValidationResult ValidatePassword(string password);
}

public class PasswordService : IPasswordService
{
    private static readonly Regex UppercaseRegex = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex LowercaseRegex = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex DigitRegex = new(@"\d", RegexOptions.Compiled);
    private static readonly Regex SpecialCharRegex = new(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]", RegexOptions.Compiled);

    public string HashPassword(string password)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedHashBytes = Convert.FromBase64String(parts[1]);

        using var hmac = new System.Security.Cryptography.HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        return storedHashBytes.SequenceEqual(computedHash);
    }

    public PasswordValidationResult ValidatePassword(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(password))
            return new PasswordValidationResult { IsValid = false, Errors = new List<string> { "密码不能为空" } };

        if (password.Length < 8)
            errors.Add("密码长度至少8位");

        if (password.Length > 64)
            errors.Add("密码长度不能超过64位");

        if (!UppercaseRegex.IsMatch(password))
            errors.Add("密码需包含至少一个大写字母");

        if (!LowercaseRegex.IsMatch(password))
            errors.Add("密码需包含至少一个小写字母");

        if (!DigitRegex.IsMatch(password))
            errors.Add("密码需包含至少一个数字");

        if (!SpecialCharRegex.IsMatch(password))
            errors.Add("密码需包含至少一个特殊字符");

        return new PasswordValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}

public class PasswordValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}
