using CandyHandyman.Core.Entities;

namespace CandyHandyman.Application.Interfaces;

public interface IScheduleService
{
    Task<List<Schedule>> GetSchedulesAsync(Guid handymanId);
    Task UpdateSchedulesAsync(Guid handymanId, List<ScheduleDto> schedules);
    Task<List<AppointmentSlot>> GetAvailableSlotsAsync(Guid handymanId, DateTime date);
    Task GenerateSlotsAsync(Guid handymanId, DateTime startDate, int days);
}

public class ScheduleDto
{
    public Guid? Id { get; set; }
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}

public class AppointmentSlotDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsBooked { get; set; }
}
