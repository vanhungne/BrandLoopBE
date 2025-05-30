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
    public class KolsJoinCampaign
    {
        [Key]
        public int KolsJoinCampaignId { get; set; }

        public int CampaignId { get; set; }

        [Required]
        [StringLength(32)]
        public string UID { get; set; }

        [StringLength(50)]
        public KolJoinCampaignStatus Status { get; set; } = KolJoinCampaignStatus.Pending;

        public DateTime AppliedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }

        [ForeignKey("UID")]
        public virtual User User { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
