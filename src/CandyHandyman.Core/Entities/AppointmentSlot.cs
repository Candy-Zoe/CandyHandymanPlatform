namespace CandyHandyman.Core.Entities;

public class AppointmentSlot : BaseEntity
{
    public Guid HandymanProfileId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsBooked { get; set; }
    public Guid? OrderId { get; set; }

    public HandymanProfile HandymanProfile { get; set; } = null!;
    public Order? Order { get; set; }
}
