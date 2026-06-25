namespace CandyHandyman.Application.Interfaces;

public interface IRankingService
{
    Task<List<HandymanRankingDto>> GetRankingAsync(Guid? categoryId, int top = 20);
}

public class HandymanRankingDto
{
    public Guid HandymanId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Level { get; set; } = string.Empty;
    public decimal AverageRating { get; set; }
    public int TotalCompletedOrders { get; set; }
    public int TotalReviews { get; set; }
}
