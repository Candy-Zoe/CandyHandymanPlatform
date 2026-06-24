using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class ServiceMedia : BaseEntity
{
    public Guid ServiceId { get; set; }
    public MediaType MediaType { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public int SortOrder { get; set; }

    public Service Service { get; set; } = null!;
}