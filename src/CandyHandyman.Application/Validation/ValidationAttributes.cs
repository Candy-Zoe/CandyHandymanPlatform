using System.ComponentModel.DataAnnotations;

namespace CandyHandyman.Application.Validation;

public class PhoneAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return ValidationResult.Success;

            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^1[3-9]\d{9}$"))
                return new ValidationResult("手机号格式不正确");
        }
        return ValidationResult.Success;
    }
}

public class NoHtmlAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string text && !string.IsNullOrEmpty(text))
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(text, @"<[^>]+>"))
                return new ValidationResult("不允许包含HTML标签");
        }
        return ValidationResult.Success;
    }
}

public class NoSqlInjectionAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string text && !string.IsNullOrEmpty(text))
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(text, @"(\b(DELETE|DROP|INSERT|UPDATE|EXEC|EXECUTE|UNION|SELECT)\b)", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                return new ValidationResult("输入包含非法关键字");
        }
        return ValidationResult.Success;
    }
}
