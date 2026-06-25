namespace CandyHandyman.Application.Interfaces;

public interface IFavoriteService
{
    Task AddFavoriteAsync(Guid userId, Guid? serviceId, Guid? handymanId);
    Task RemoveFavoriteAsync(Guid userId, Guid? serviceId, Guid? handymanId);
    Task<List<FavoriteDto>> GetUserFavoritesAsync(Guid userId);
    Task<bool> IsFavoritedAsync(Guid userId, Guid? serviceId, Guid? handymanId);
}

public class FavoriteDto
{
    public Guid Id { get; set; }
    public Guid? ServiceId { get; set; }
    public string? ServiceTitle { get; set; }
    public Guid? HandymanProfileId { get; set; }
    public string? HandymanName { get; set; }
    public DateTime CreatedAt { get; set; }
}
