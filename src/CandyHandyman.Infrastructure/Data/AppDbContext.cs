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
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationSetting> NotificationSettings => Set<NotificationSetting>();
    public DbSet<UserFcmToken> UserFcmTokens => Set<UserFcmToken>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<AppointmentSlot> AppointmentSlots => Set<AppointmentSlot>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<UserCoupon> UserCoupons => Set<UserCoupon>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<BrowsingHistory> BrowsingHistories => Set<BrowsingHistory>();
    public DbSet<ServiceArea> ServiceAreas => Set<ServiceArea>();
    public DbSet<OrderTemplate> OrderTemplates => Set<OrderTemplate>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<HelpTopic> HelpTopics => Set<HelpTopic>();
    public DbSet<OperationLog> OperationLogs => Set<OperationLog>();

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
            e.HasIndex(s => new { s.CategoryId, s.Status });
            e.HasIndex(s => new { s.HandymanProfileId, s.Status });
            e.HasIndex(s => s.CreatedAt);
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
            e.HasIndex(o => new { o.CustomerId, o.Status });
            e.HasIndex(o => new { o.ProviderId, o.Status });
            e.HasIndex(o => o.CreatedAt);
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
            e.HasIndex(m => new { m.ConversationId, m.CreatedAt });
            e.HasOne(m => m.Conversation).WithMany(c => c.Messages).HasForeignKey(m => m.ConversationId);
            e.HasOne(m => m.Sender).WithMany(u => u.SentMessages).HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Review>(e =>
        {
            e.HasIndex(r => r.OrderId).IsUnique();
            e.HasIndex(r => new { r.ProviderId, r.CreatedAt });
            e.HasOne(r => r.Order).WithOne(o => o.Review).HasForeignKey<Review>(r => r.OrderId);
            e.HasOne(r => r.Customer).WithMany(u => u.GivenReviews).HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Provider).WithMany(u => u.ReceivedReviews).HasForeignKey(r => r.ProviderId).OnDelete(DeleteBehavior.Restrict);
            e.Property(r => r.Rating).HasMaxLength(1);
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

        modelBuilder.Entity<Notification>(e =>
        {
            e.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId);
            e.HasIndex(n => new { n.UserId, n.IsRead });
        });

        modelBuilder.Entity<NotificationSetting>(e =>
        {
            e.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId);
            e.HasIndex(s => new { s.UserId, s.Type }).IsUnique();
        });

        modelBuilder.Entity<UserFcmToken>(e =>
        {
            e.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);
            e.HasIndex(t => t.Token).IsUnique();
        });

        modelBuilder.Entity<Schedule>(e =>
        {
            e.HasOne(s => s.HandymanProfile).WithMany().HasForeignKey(s => s.HandymanProfileId);
            e.HasIndex(s => new { s.HandymanProfileId, s.DayOfWeek }).IsUnique();
        });

        modelBuilder.Entity<AppointmentSlot>(e =>
        {
            e.HasOne(s => s.HandymanProfile).WithMany().HasForeignKey(s => s.HandymanProfileId);
            e.HasOne(s => s.Order).WithMany().HasForeignKey(s => s.OrderId).IsRequired(false);
            e.HasIndex(s => new { s.HandymanProfileId, s.Date, s.StartTime }).IsUnique();
        });

        modelBuilder.Entity<Coupon>(e =>
        {
            e.HasIndex(c => c.Code).IsUnique();
            e.Property(c => c.DiscountValue).HasPrecision(18, 2);
            e.Property(c => c.MinAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<UserCoupon>(e =>
        {
            e.HasOne(uc => uc.User).WithMany().HasForeignKey(uc => uc.UserId);
            e.HasOne(uc => uc.Coupon).WithMany().HasForeignKey(uc => uc.CouponId);
            e.HasIndex(uc => new { uc.UserId, uc.CouponId }).IsUnique();
        });

        modelBuilder.Entity<WalletTransaction>(e =>
        {
            e.HasIndex(t => new { t.UserId, t.CreatedAt });
            e.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.Property(t => t.Balance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.HasIndex(p => p.OrderId).IsUnique();
            e.HasIndex(p => new { p.Status, p.CreatedAt });
            e.HasOne(p => p.Order).WithOne(o => o.Payment).HasForeignKey<Payment>(p => p.OrderId);
            e.Property(p => p.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Favorite>(e =>
        {
            e.HasIndex(f => new { f.UserId, f.ServiceId });
            e.HasIndex(f => new { f.UserId, f.HandymanProfileId });
            e.HasOne(f => f.User).WithMany().HasForeignKey(f => f.UserId);
            e.HasOne(f => f.Service).WithMany().HasForeignKey(f => f.ServiceId).IsRequired(false);
            e.HasOne(f => f.HandymanProfile).WithMany().HasForeignKey(f => f.HandymanProfileId).IsRequired(false);
        });

        modelBuilder.Entity<BrowsingHistory>(e =>
        {
            e.HasIndex(h => new { h.UserId, h.UpdatedAt });
            e.HasOne(h => h.User).WithMany().HasForeignKey(h => h.UserId);
            e.HasOne(h => h.Service).WithMany().HasForeignKey(h => h.ServiceId).IsRequired(false);
            e.HasOne(h => h.HandymanProfile).WithMany().HasForeignKey(h => h.HandymanProfileId).IsRequired(false);
        });

        modelBuilder.Entity<ServiceArea>(e =>
        {
            e.HasOne(a => a.HandymanProfile).WithMany().HasForeignKey(a => a.HandymanProfileId);
        });

        modelBuilder.Entity<OrderTemplate>(e =>
        {
            e.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);
        });

        modelBuilder.Entity<Announcement>(e =>
        {
            e.HasIndex(a => a.Title);
        });

        modelBuilder.Entity<Feedback>(e =>
        {
            e.HasOne(f => f.User).WithMany().HasForeignKey(f => f.UserId);
        });

        modelBuilder.Entity<HelpTopic>(e =>
        {
            e.HasIndex(h => h.Category);
        });

        modelBuilder.Entity<OperationLog>(e =>
        {
            e.HasIndex(l => new { l.EntityType, l.EntityId });
        });
    }
}