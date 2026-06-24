using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class CraftsmanCertification : BaseEntity
{
    public Guid HandymanProfileId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string CertificateName { get; set; } = string.Empty;
    public string CertificateNo { get; set; } = string.Empty;
    public string CertificateUrl { get; set; } = string.Empty;
    public string? IssuingAuthority { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public string? RejectReason { get; set; }

    public HandymanProfile HandymanProfile { get; set; } = null!;
}