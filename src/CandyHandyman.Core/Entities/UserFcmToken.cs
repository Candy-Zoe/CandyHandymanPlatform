namespace CandyHandyman.Core.Entities;

public class UserFcmToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? DeviceType { get; set; }

    public User User { get; set; } = null!;
}
