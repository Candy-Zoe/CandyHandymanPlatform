using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class Dispute : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid InitiatorId { get; set; }
    public Guid RespondentId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? EvidenceUrls { get; set; }
    public DisputeStatus Status { get; set; } = DisputeStatus.Open;
    public string? Resolution { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Order Order { get; set; } = null!;
    public User Initiator { get; set; } = null!;
    public User Respondent { get; set; } = null!;
}