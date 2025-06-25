using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Enums;

namespace BrandLoop.Domain.Entities
{
    public class User
    {
        [Key]
        [StringLength(32)]
        public string UID { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(255)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        public int RoleId { get; set; }

        public UserStatus Status { get; set; }

        [StringLength(255)]
        public string ProfileImage { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public virtual BrandProfile BrandProfile { get; set; }
        public virtual InfluenceProfile InfluenceProfile { get; set; }
        public virtual ICollection<ContentAndStyle> ContentAndStyles { get; set; }
        public virtual ICollection<Skill> Skills { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
        public virtual ICollection<Message> ReceivedMessages { get; set; }
        public virtual ICollection<MessageReadStatus> MessageReadStatuses { get; set; }
        public virtual UserOnlineStatus OnlineStatus { get; set; }
        public virtual ICollection<SubscriptionRegister> SubscriptionRegisters { get; set; }
        public virtual ICollection<Campaign> CreatedCampaigns { get; set; }
        public virtual ICollection<KolsJoinCampaign> KolsJoinCampaigns { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<CampaignInvitation> CampaignInvitations { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
        public virtual ICollection<RefreshTokens> RefreshTokens { get; set; }
    }
}
