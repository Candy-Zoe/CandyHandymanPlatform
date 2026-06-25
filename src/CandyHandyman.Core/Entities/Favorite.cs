namespace CandyHandyman.Core.Entities;

public class Favorite : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? ServiceId { get; set; }
    public Guid? HandymanProfileId { get; set; }

    public User User { get; set; } = null!;
    public Service? Service { get; set; }
    public HandymanProfile? HandymanProfile { get; set; }
}
