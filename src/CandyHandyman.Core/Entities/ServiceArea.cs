namespace CandyHandyman.Core.Entities;

public class ServiceArea : BaseEntity
{
    public Guid HandymanProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public double RadiusKm { get; set; } = 10;
    public bool IsActive { get; set; } = true;

    public HandymanProfile HandymanProfile { get; set; } = null!;
}
