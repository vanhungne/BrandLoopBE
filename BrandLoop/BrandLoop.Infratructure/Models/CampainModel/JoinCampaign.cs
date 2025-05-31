using BrandLoop.Domain.Enums;
using BrandLoop.Shared.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class JoinCampaign
    {
        [Required]
        public int CampaignId { get; set; }

        [Required]
        [StringLength(32)]
        public string UID { get; set; } // KOL được mời

        [StringLength(255)]
        public string Title { get; set; }

        public string Message { get; set; }

        public decimal? ProposedRate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTimeHelper.GetVietnamNow();
    }

    public class InvitationResponse
    {
        public int InvitationId { get; set; }
        public string UID {get; set; }
        public string Message { get; set; }
        public decimal? Price { get; set; }
    }
}
