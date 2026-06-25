namespace CandyHandyman.Application.Interfaces;

public interface IFeedbackService
{
    Task<FeedbackDto> SubmitAsync(Guid userId, SubmitFeedbackDto dto);
    Task<List<FeedbackDto>> GetUserFeedbacksAsync(Guid userId);
    Task<List<FeedbackDto>> GetAllFeedbacksAsync(int page = 1, int pageSize = 20);
    Task ReplyAsync(Guid feedbackId, string reply);
    Task UpdateStatusAsync(Guid feedbackId, string status);
}

public class FeedbackDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AdminReply { get; set; }
    public DateTime? RepliedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SubmitFeedbackDto
{
    public string Content { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string Type { get; set; } = "Suggestion";
}
