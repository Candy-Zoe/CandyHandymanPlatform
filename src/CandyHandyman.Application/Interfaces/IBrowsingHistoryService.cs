namespace CandyHandyman.Application.Interfaces;

public interface IBrowsingHistoryService
{
    Task RecordAsync(Guid userId, Guid? serviceId, Guid? handymanId);
    Task<List<BrowsingHistoryDto>> GetUserHistoryAsync(Guid userId, int limit = 50);
    Task ClearHistoryAsync(Guid userId);
}

public class BrowsingHistoryDto
{
    public Guid Id { get; set; }
    public Guid? ServiceId { get; set; }
    public string? ServiceTitle { get; set; }
    public Guid? HandymanProfileId { get; set; }
    public string? HandymanName { get; set; }
    public DateTime ViewedAt { get; set; }
}
