using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrandLoop.Infratructure.Persistence
{
    public class BLDBContext : DbContext
    {
        public BLDBContext()
        {
        }
        public BLDBContext(DbContextOptions<BLDBContext> options) : base(options)
        {
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BrandProfile> BrandProfiles { get; set; }
        public DbSet<InfluenceProfile> InfluenceProfiles { get; set; }
        public DbSet<ContentAndStyle> ContentAndStyles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReadStatus> MessageReadStatuses { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionFeature> SubscriptionFeatures { get; set; }
        public DbSet<SubscriptionRegister> SubscriptionRegisters { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<KolsJoinCampaign> KolsJoinCampaigns { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<CampaignInvitation> CampaignInvitations { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<CampaignReport> CampaignReports { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<InfluencerReport> InfluencerReports { get; set; }

        public DbSet<CampaignImage> CampainImages { get; set; }
        public DbSet<InfluencerType> InfluencerTypes { get; set; }

        public DbSet<UserOnlineStatus> UserOnlineStatuses { get; set; }
        public DbSet<Banner> Banners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User and Role relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // User and BrandProfile relationship
            modelBuilder.Entity<BrandProfile>()
                .HasOne(bp => bp.User)
                .WithOne(u => u.BrandProfile)
                .HasForeignKey<BrandProfile>(bp => bp.UID)
                .OnDelete(DeleteBehavior.Cascade);

            // User and InfluenceProfile relationship
            modelBuilder.Entity<InfluenceProfile>()
                .HasOne(ip => ip.User)
                .WithOne(u => u.InfluenceProfile)
                .HasForeignKey<InfluenceProfile>(ip => ip.UID)
                .OnDelete(DeleteBehavior.Cascade);

            // User and ContentAndStyle relationship
            modelBuilder.Entity<ContentAndStyle>()
                .HasOne(cs => cs.User)
                .WithMany(u => u.ContentAndStyles)
                .HasForeignKey(cs => cs.UID)
                .OnDelete(DeleteBehavior.Cascade);

            // User and Skill relationship
            modelBuilder.Entity<Skill>()
                .HasOne(s => s.User)
                .WithMany(u => u.Skills)
                .HasForeignKey(s => s.UID)
                .OnDelete(DeleteBehavior.Cascade);

            // User and Notification relationship
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UID)
                .OnDelete(DeleteBehavior.Cascade);


            // MessageReadStatus relationships
            modelBuilder.Entity<MessageReadStatus>()
                .HasOne(mrs => mrs.Message)
                .WithMany(m => m.ReadStatuses)
                .HasForeignKey(mrs => mrs.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Message>()
                    .HasOne(m => m.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                            modelBuilder.Entity<Message>()
                                .HasOne(m => m.Receiver)
                                .WithMany(u => u.ReceivedMessages)
                                .HasForeignKey(m => m.ReceiverId)
                                .OnDelete(DeleteBehavior.Restrict);



            // Subscription and Feature relationship (many-to-many)
            modelBuilder.Entity<SubscriptionFeature>()
                .HasOne(sf => sf.Subscription)
                .WithMany(s => s.SubscriptionFeatures)
                .HasForeignKey(sf => sf.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubscriptionFeature>()
                .HasOne(sf => sf.Feature)
                .WithMany(f => f.SubscriptionFeatures)
                .HasForeignKey(sf => sf.FeatureId)
                .OnDelete(DeleteBehavior.Restrict);

            // SubscriptionRegister relationships
            modelBuilder.Entity<SubscriptionRegister>()
                .HasOne(sr => sr.User)
                .WithMany(u => u.SubscriptionRegisters)
                .HasForeignKey(sr => sr.UID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubscriptionRegister>()
                .HasOne(sr => sr.Subscription)
                .WithMany(s => s.SubscriptionRegisters)
                .HasForeignKey(sr => sr.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Campaign relationships
            modelBuilder.Entity<Campaign>()
                .HasOne(c => c.Brand)
                .WithMany(bp => bp.Campaigns)
                .HasForeignKey(c => c.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Campaign>()
                .HasOne(c => c.Creator)
                .WithMany(u => u.CreatedCampaigns)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // KolsJoinCampaign relationships
            modelBuilder.Entity<KolsJoinCampaign>()
                .HasOne(kjc => kjc.Campaign)
                .WithMany(c => c.KolsJoinCampaigns)
                .HasForeignKey(kjc => kjc.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KolsJoinCampaign>()
                .HasOne(kjc => kjc.User)
                .WithMany(u => u.KolsJoinCampaigns)
                .HasForeignKey(kjc => kjc.UID)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment relationships
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.SubscriptionRegister)
                .WithMany(sr => sr.Payments)
                .HasForeignKey(p => p.SubscriptionRegisterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.campaign)
                .WithMany(kjc => kjc.Payments)
                .HasForeignKey(p => p.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback relationships
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Campaign)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.FromUser)
                .WithMany()
                .HasForeignKey(f => f.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.ToUser)
                .WithMany() 
                .HasForeignKey(f => f.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // CampaignInvitation relationships
            modelBuilder.Entity<CampaignInvitation>()
                .HasOne(ci => ci.Campaign)
                .WithMany(c => c.CampaignInvitations)
                .HasForeignKey(ci => ci.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CampaignInvitation>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CampaignInvitations)
                .HasForeignKey(ci => ci.UID)
                .OnDelete(DeleteBehavior.Restrict);

            // Deal relationships
            modelBuilder.Entity<Deal>()
                .HasOne(d => d.Invitation)
                .WithMany(ci => ci.Deals)
                .HasForeignKey(d => d.InvitationId)
                .OnDelete(DeleteBehavior.Cascade);

            // CampaignReport relationships
            modelBuilder.Entity<CampaignReport>()
                .HasOne(cr => cr.Campaign)
                .WithMany(c => c.CampaignReports)
                .HasForeignKey(cr => cr.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            // News relationships
            modelBuilder.Entity<News>()
                .HasOne(n => n.AuthorUser)
                .WithMany(u => u.News)
                .HasForeignKey(n => n.Author)
                .OnDelete(DeleteBehavior.Restrict);

            // AuditLog relationships
            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UID)
                .OnDelete(DeleteBehavior.Restrict);

            // Wallet relationships
            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UID)
                .OnDelete(DeleteBehavior.Cascade);

            // Transaction relationships
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.FromWallet)
                .WithMany(w => w.FromTransactions)
                .HasForeignKey(t => t.FromWalletId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ToWallet)
                .WithMany(w => w.ToTransactions)
                .HasForeignKey(t => t.ToWalletId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InfluenceProfile>()
                .HasOne(ip => ip.InfluencerType)
                .WithMany(it => it.InfluenceProfiles)
                .HasForeignKey(ip => ip.InfluencerTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            // Influencer report relationships
            modelBuilder.Entity<KolsJoinCampaign>()
                .HasOne(k => k.InfluencerReport)
                .WithOne(r => r.KolsJoinCampaign)
                .HasForeignKey<InfluencerReport>(r => r.InfluencerReportId);

            // Configure decimal precision for money fields
            modelBuilder.Entity<Wallet>()
                .Property(w => w.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.CommissionAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.CommissionRate)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Deal>()
                .Property(d => d.AdminCommissionRate)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Deal>()
                .Property(d => d.AdminCommissionAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Deal>()
                .Property(d => d.PaidAmount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Banner>(entity =>
            {
                // Khóa chính
                entity.HasKey(b => b.BannerId);

                // InfluenceId là FK bắt buộc
                entity.Property(b => b.InfluenceId)
                      .IsRequired();

                // ImageUrl bắt buộc, độ dài tối đa 500
                entity.Property(b => b.ImageUrl)
                      .IsRequired()
                      .HasMaxLength(500);

                // TargetUrl không bắt buộc, độ dài tối đa 500
                entity.Property(b => b.TargetUrl)
                      .HasMaxLength(500);

                // StartDate, EndDate bắt buộc
                entity.Property(b => b.StartDate)
                      .IsRequired();
                entity.Property(b => b.EndDate)
                      .IsRequired();

                // Quan hệ 1-N với InfluenceProfile
                entity.HasOne(b => b.InfluenceProfile)
                      .WithMany() // nếu bạn thêm ICollection<Banner> Banners trong InfluenceProfile thì đổi thành .WithMany(ip => ip.Banners)
                      .HasForeignKey(b => b.InfluenceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // UserOnlineStatus configuration
            modelBuilder.Entity<UserOnlineStatus>(entity =>
            {
                entity.Property(e => e.IsOnline)
                    .IsRequired();

                entity.Property(e => e.LastSeen)
                    .IsRequired();

                entity.Property(e => e.ConnectionId)
                    .HasMaxLength(100);
            });
            // Default data seeding
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", Description = "System Administrator" },
                new Role { RoleId = 2, RoleName = "Brand", Description = "Brand/Company Account" },
                new Role { RoleId = 3, RoleName = "Influencer", Description = "Key Opinion Leader/Influencer" },
                new Role { RoleId = 4, RoleName = "Guest", Description = "Guest User" }
            );
            modelBuilder.Entity<InfluencerType>().HasData(
                new InfluencerType { Id = 1, Name = "Norman", MinFollower = 0, MaxFollower = 10000, PlatformFee = 10000 },
                new InfluencerType { Id = 2, Name = "Nano Influencers", MinFollower = 10000, MaxFollower = 50000, PlatformFee = 100000 },
                new InfluencerType { Id = 3, Name = "Micro Influencers", MinFollower = 50000, MaxFollower = 100000, PlatformFee = 200000 },
                new InfluencerType { Id = 4, Name = "Mid-Tier Influencers", MinFollower = 100000, MaxFollower = 500000, PlatformFee = 300000 },
                new InfluencerType { Id = 5, Name = "Macro Influencers", MinFollower = 500000, MaxFollower = 1000000, PlatformFee = 500000 }
            );
        }
    }
}
