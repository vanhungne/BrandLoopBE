using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Evidence
    {
        [Key]
        public int EvidenceId { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        public EvidenceType EvidenceOf { get; set; }

        [Required]
        [StringLength(500)]
        public string Link { get; set; }

        [ForeignKey("InfluencerReport")]
        public int? InfluencerReportId { get; set; }

        public InfluencerReport InfluencerReport { get; set; }

        public int? KolsJoinCampaignId { get; set; }

        [ForeignKey("KolsJoinCampaignId")]
        public KolsJoinCampaign KolsJoinCampaign { get; set; }
    }
}
