namespace CandyHandyman.Core.Entities;

public class Feedback : BaseEntity
{
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public FeedbackType Type { get; set; }
    public FeedbackStatus Status { get; set; }
    public string? AdminReply { get; set; }
    public DateTime? RepliedAt { get; set; }

    public User User { get; set; } = null!;
}

public enum FeedbackType
{
    Bug = 0,
    Suggestion = 1,
    Complaint = 2,
    Other = 3
}

public enum FeedbackStatus
{
    Pending = 0,
    Processing = 1,
    Resolved = 2
}
