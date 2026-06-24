using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class Service : BaseEntity
{
    public Guid HandymanProfileId { get; set; }
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PricingType PricingType { get; set; }
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty;
    public ServiceStatus Status { get; set; } = ServiceStatus.Active;
    public int ViewCount { get; set; }

    public HandymanProfile HandymanProfile { get; set; } = null!;
    public SkillCategory Category { get; set; } = null!;
    public ICollection<ServiceMedia> Media { get; set; } = new List<ServiceMedia>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}