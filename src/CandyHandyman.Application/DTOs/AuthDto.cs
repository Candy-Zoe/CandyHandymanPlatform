using System.ComponentModel.DataAnnotations;

namespace CandyHandyman.Application.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "昵称不能为空")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "昵称长度需在2-20个字符之间")]
    public string Nickname { get; set; } = string.Empty;

    [Required(ErrorMessage = "手机号不能为空")]
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(64, MinimumLength = 8, ErrorMessage = "密码长度需在8-64个字符之间")]
    public string Password { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "简介不能超过500个字符")]
    public string? Bio { get; set; }
}

public class LoginDto
{
    [Required(ErrorMessage = "手机号不能为空")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Role { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string? Bio { get; set; }
}

public class UpdateProfileDto
{
    [StringLength(20, MinimumLength = 2, ErrorMessage = "昵称长度需在2-20个字符之间")]
    public string? Nickname { get; set; }

    [StringLength(500, ErrorMessage = "简介不能超过500个字符")]
    public string? Bio { get; set; }

    [Range(-90, 90, ErrorMessage = "纬度范围为-90到90")]
    public double? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "经度范围为-180到180")]
    public double? Longitude { get; set; }
}
