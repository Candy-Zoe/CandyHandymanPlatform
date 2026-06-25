namespace CandyHandyman.Core.Entities;

public class Schedule : BaseEntity
{
    public Guid HandymanProfileId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;

    public HandymanProfile HandymanProfile { get; set; } = null!;
}
