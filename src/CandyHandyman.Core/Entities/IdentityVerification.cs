using CandyHandyman.Core.Enums;

namespace CandyHandyman.Core.Entities;

public class IdentityVerification : BaseEntity
{
    public Guid UserId { get; set; }
    public string RealName { get; set; } = string.Empty;
    public string IdCardNumber { get; set; } = string.Empty;
    public string IdCardFrontUrl { get; set; } = string.Empty;
    public string IdCardBackUrl { get; set; } = string.Empty;
    public string? FacePhotoUrl { get; set; }
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public string? RejectReason { get; set; }
    public DateTime? VerifiedAt { get; set; }

    public User User { get; set; } = null!;
}