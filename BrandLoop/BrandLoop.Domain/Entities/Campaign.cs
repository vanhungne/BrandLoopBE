using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;

namespace BrandLoop.Domain.Entities
{
    public class Campaign
    {
        [Key]
        public int CampaignId { get; set; }

        public int BrandId { get; set; }

        [Required]
        [StringLength(255)]
        public string CampaignName { get; set; }

        public string Description { get; set; }

        public DateTime UploadedDate { get; set; } = DateTime.Now;

        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public string ContentRequirements { get; set; }

        public string CampaignGoals { get; set; }

        public decimal? Budget { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? Deadline { get; set; }

        [StringLength(50)]
        public CampainStatus Status { get; set; } = CampainStatus.Approved;

        [Required]
        [StringLength(32)]
        public string CreatedBy { get; set; }

        // Navigation properties
        [ForeignKey("BrandId")]
        public virtual BrandProfile Brand { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User Creator { get; set; }

        public virtual ICollection<KolsJoinCampaign> KolsJoinCampaigns { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<CampaignInvitation> CampaignInvitations { get; set; }
        public virtual ICollection<CampaignReport> CampaignReports { get; set; }
        public virtual ICollection<CampaignImage> CampaignImages { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }

    }
}