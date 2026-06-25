using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class HandymanProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public int YearsOfExperience { get; set; }
    public double ServiceRadius { get; set; } = 10;
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalCompletedOrders { get; set; }
    public CraftsmanLevel Level { get; set; } = CraftsmanLevel.Junior;
    public bool IsVerified { get; set; }
    public bool IsAvailable { get; set; } = true;

    public User User { get; set; } = null!;
    public ICollection<SkillCategory> Categories { get; set; } = new List<SkillCategory>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
}