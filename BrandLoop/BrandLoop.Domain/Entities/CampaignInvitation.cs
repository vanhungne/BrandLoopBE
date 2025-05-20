using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class CampaignInvitation
    {
        [Key]
        public int InvitationId { get; set; }

        public int CampaignId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; } // KOL được mời

        [StringLength(255)]
        public string Title { get; set; }

        public string Message { get; set; }

        public decimal? ProposedRate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "pending"; // pending, accepted, rejected

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal? Price { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        // Navigation properties
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }

        public virtual ICollection<Deal> Deals { get; set; }
    }
}
