using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class InsurancePolicy : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ProviderId { get; set; }
    public string PolicyNo { get; set; } = string.Empty;
    public InsuranceType Type { get; set; }
    public decimal Premium { get; set; }
    public decimal CoverageAmount { get; set; }
    public InsuranceStatus Status { get; set; } = InsuranceStatus.Active;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Order Order { get; set; } = null!;
    public User Customer { get; set; } = null!;
    public User Provider { get; set; } = null!;
}