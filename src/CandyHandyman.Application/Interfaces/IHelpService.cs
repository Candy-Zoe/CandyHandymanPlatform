namespace CandyHandyman.Application.Interfaces;

public interface IHelpService
{
    Task<List<HelpTopicDto>> GetAllAsync();
    Task<List<HelpTopicDto>> GetByCategoryAsync(string category);
    Task<HelpTopicDto?> GetByIdAsync(Guid id);
    Task<HelpTopicDto> CreateAsync(CreateHelpTopicDto dto);
    Task UpdateAsync(Guid id, CreateHelpTopicDto dto);
    Task DeleteAsync(Guid id);
}

public class HelpTopicDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int ViewCount { get; set; }
}

public class CreateHelpTopicDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
