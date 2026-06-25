using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Schedule>> GetSchedulesAsync(Guid handymanId)
    {
        return await _context.Schedules
            .Where(s => s.HandymanProfileId == handymanId)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync();
    }

    public async Task UpdateSchedulesAsync(Guid handymanId, List<ScheduleDto> schedules)
    {
        var existing = await _context.Schedules
            .Where(s => s.HandymanProfileId == handymanId)
            .ToListAsync();

        _context.Schedules.RemoveRange(existing);

        foreach (var dto in schedules)
        {
            var schedule = new Schedule
            {
                Id = Guid.NewGuid(),
                HandymanProfileId = handymanId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = TimeSpan.Parse(dto.StartTime),
                EndTime = TimeSpan.Parse(dto.EndTime),
                IsAvailable = dto.IsAvailable
            };
            _context.Schedules.Add(schedule);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<AppointmentSlot>> GetAvailableSlotsAsync(Guid handymanId, DateTime date)
    {
        return await _context.AppointmentSlots
            .Where(s => s.HandymanProfileId == handymanId
                     && s.Date.Date == date.Date
                     && !s.IsBooked)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task GenerateSlotsAsync(Guid handymanId, DateTime startDate, int days)
    {
        var schedules = await GetSchedulesAsync(handymanId);
        var scheduleDict = schedules.ToDictionary(s => s.DayOfWeek);

        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            var dayOfWeek = (int)date.DayOfWeek;

            if (!scheduleDict.TryGetValue(dayOfWeek, out var schedule) || !schedule.IsAvailable)
                continue;

            var existingSlots = await _context.AppointmentSlots
                .Where(s => s.HandymanProfileId == handymanId && s.Date.Date == date.Date)
                .CountAsync();

            if (existingSlots > 0) continue;

            var currentTime = schedule.StartTime;
            while (currentTime + TimeSpan.FromHours(1) <= schedule.EndTime)
            {
                var slot = new AppointmentSlot
                {
                    Id = Guid.NewGuid(),
                    HandymanProfileId = handymanId,
                    Date = date.Date,
                    StartTime = currentTime,
                    EndTime = currentTime + TimeSpan.FromHours(1),
                    IsBooked = false
                };
                _context.AppointmentSlots.Add(slot);
                currentTime += TimeSpan.FromHours(1);
            }
        }

        await _context.SaveChangesAsync();
    }
}
