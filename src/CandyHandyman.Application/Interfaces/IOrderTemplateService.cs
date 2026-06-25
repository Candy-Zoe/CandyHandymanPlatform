namespace CandyHandyman.Application.Interfaces;

public interface IOrderTemplateService
{
    Task<OrderTemplateDto> CreateAsync(Guid userId, CreateOrderTemplateDto dto);
    Task<List<OrderTemplateDto>> GetUserTemplatesAsync(Guid userId);
    Task UpdateAsync(Guid userId, Guid templateId, CreateOrderTemplateDto dto);
    Task DeleteAsync(Guid userId, Guid templateId);
}

public class OrderTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
}

public class CreateOrderTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
}
