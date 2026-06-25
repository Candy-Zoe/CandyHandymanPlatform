namespace CandyHandyman.Application.Interfaces;

public interface IAnnouncementService
{
    Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto dto);
    Task<List<AnnouncementDto>> GetPublishedAsync();
    Task<List<AnnouncementDto>> GetAllAsync();
    Task UpdateAsync(Guid id, CreateAnnouncementDto dto);
    Task PublishAsync(Guid id);
    Task DeleteAsync(Guid id);
}

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAnnouncementDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = "System";
    public DateTime? ExpiresAt { get; set; }
}
