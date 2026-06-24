using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class User : BaseEntity
{
    public string Nickname { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer;
    public decimal Balance { get; set; }
    public string? Bio { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public HandymanProfile? HandymanProfile { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Order> CustomerOrders { get; set; } = new List<Order>();
    public ICollection<Order> ProviderOrders { get; set; } = new List<Order>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Review> GivenReviews { get; set; } = new List<Review>();
    public ICollection<Review> ReceivedReviews { get; set; } = new List<Review>();
}