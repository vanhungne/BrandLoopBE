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
    public class CampaignInvitation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvitationId { get; set; }

        public int CampaignId { get; set; }

        [Required]
        [StringLength(32)]
        public string UID { get; set; } // KOL được mời

        [StringLength(255)]
        public string Title { get; set; }

        public string Message { get; set; }
        public string NegotiateMessage { get; set; }

        public decimal? ProposedRate { get; set; } = 0;

        public CampaignInvitationStatus Status { get; set; } = CampaignInvitationStatus.pending;

        public DateTime CreatedAt { get; set; }

        public decimal? Price { get; set; } = 0;

        public JoinCampaignType Type { get; set; }

        // Navigation properties
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }

        [ForeignKey("UID")]
        public virtual User User { get; set; }

        public virtual ICollection<Deal> Deals { get; set; }
    }
}
