using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class KolsJoinCampaign
    {
        [Key]
        public int KolsJoinCampaignId { get; set; }

        public int CampaignId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "pending"; // pending, accepted, rejected, completed

        public DateTime AppliedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
