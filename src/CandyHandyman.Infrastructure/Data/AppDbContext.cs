using CandyHandyman.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<SkillCategory> SkillCategories => Set<SkillCategory>();
    public DbSet<HandymanProfile> HandymanProfiles => Set<HandymanProfile>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceMedia> ServiceMedia => Set<ServiceMedia>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<IdentityVerification> IdentityVerifications => Set<IdentityVerification>();
    public DbSet<CraftsmanCertification> CraftsmanCertifications => Set<CraftsmanCertification>();
    public DbSet<InsurancePolicy> InsurancePolicies => Set<InsurancePolicy>();
    public DbSet<Dispute> Disputes => Set<Dispute>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Phone).IsUnique();
            e.Property(u => u.Balance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasIndex(rt => rt.Token).IsUnique();
            e.HasOne(rt => rt.User).WithMany(u => u.RefreshTokens).HasForeignKey(rt => rt.UserId);
        });

        modelBuilder.Entity<SkillCategory>(e =>
        {
            e.HasOne(sc => sc.Parent).WithMany(sc => sc.Children).HasForeignKey(sc => sc.ParentId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HandymanProfile>(e =>
        {
            e.HasOne(hp => hp.User).WithOne(u => u.HandymanProfile).HasForeignKey<HandymanProfile>(hp => hp.UserId);
            e.Property(hp => hp.AverageRating).HasPrecision(3, 2);
            e.HasMany(hp => hp.Categories).WithMany(sc => sc.HandymanProfiles);
        });

        modelBuilder.Entity<Service>(e =>
        {
            e.HasOne(s => s.HandymanProfile).WithMany(hp => hp.Services).HasForeignKey(s => s.HandymanProfileId);
            e.HasOne(s => s.Category).WithMany(sc => sc.Services).HasForeignKey(s => s.CategoryId);
            e.Property(s => s.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ServiceMedia>(e =>
        {
            e.HasOne(sm => sm.Service).WithMany(s => s.Media).HasForeignKey(sm => sm.ServiceId);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasIndex(o => o.OrderNo).IsUnique();
            e.HasOne(o => o.Service).WithMany(s => s.Orders).HasForeignKey(o => o.ServiceId);
            e.HasOne(o => o.Customer).WithMany(u => u.CustomerOrders).HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.Provider).WithMany(u => u.ProviderOrders).HasForeignKey(o => o.ProviderId).OnDelete(DeleteBehavior.Restrict);
            e.Property(o => o.UnitPrice).HasPrecision(18, 2);
            e.Property(o => o.TotalAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Conversation>(e =>
        {
            e.HasOne(c => c.User1).WithMany().HasForeignKey(c => c.User1Id).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(c => c.User2).WithMany().HasForeignKey(c => c.User2Id).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Message>(e =>
        {
            e.HasOne(m => m.Conversation).WithMany(c => c.Messages).HasForeignKey(m => m.ConversationId);
            e.HasOne(m => m.Sender).WithMany(u => u.SentMessages).HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Review>(e =>
        {
            e.HasOne(r => r.Order).WithOne(o => o.Review).HasForeignKey<Review>(r => r.OrderId);
            e.HasOne(r => r.Customer).WithMany(u => u.GivenReviews).HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Provider).WithMany(u => u.ReceivedReviews).HasForeignKey(r => r.ProviderId).OnDelete(DeleteBehavior.Restrict);
            e.Property(r => r.Rating).HasMaxLength(1);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.HasOne(p => p.Order).WithOne(o => o.Payment).HasForeignKey<Payment>(p => p.OrderId);
            e.Property(p => p.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<IdentityVerification>(e =>
        {
            e.HasOne(iv => iv.User).WithMany().HasForeignKey(iv => iv.UserId);
            e.HasIndex(iv => iv.UserId).IsUnique();
        });

        modelBuilder.Entity<CraftsmanCertification>(e =>
        {
            e.HasOne(cc => cc.HandymanProfile).WithMany().HasForeignKey(cc => cc.HandymanProfileId);
        });

        modelBuilder.Entity<InsurancePolicy>(e =>
        {
            e.HasOne(ip => ip.Order).WithMany().HasForeignKey(ip => ip.OrderId);
            e.HasOne(ip => ip.Customer).WithMany().HasForeignKey(ip => ip.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(ip => ip.Provider).WithMany().HasForeignKey(ip => ip.ProviderId).OnDelete(DeleteBehavior.Restrict);
            e.Property(ip => ip.Premium).HasPrecision(18, 2);
            e.Property(ip => ip.CoverageAmount).HasPrecision(18, 2);
            e.HasIndex(ip => ip.PolicyNo).IsUnique();
        });

        modelBuilder.Entity<Dispute>(e =>
        {
            e.HasOne(d => d.Order).WithMany().HasForeignKey(d => d.OrderId);
            e.HasOne(d => d.Initiator).WithMany().HasForeignKey(d => d.InitiatorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.Respondent).WithMany().HasForeignKey(d => d.RespondentId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}