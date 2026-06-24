namespace CandyHandyman.Core.Entities;

public class SkillCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }

    public SkillCategory? Parent { get; set; }
    public ICollection<SkillCategory> Children { get; set; } = new List<SkillCategory>();
    public ICollection<HandymanProfile> HandymanProfiles { get; set; } = new List<HandymanProfile>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
}