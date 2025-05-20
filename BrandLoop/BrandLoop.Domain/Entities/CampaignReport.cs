using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class CampaignReport
    {
        [Key]
        public int CampaignReportId { get; set; }

        public int CampaignId { get; set; }

        public string Result { get; set; }

        public decimal? Revenue { get; set; }

        public int? CustomerAmount { get; set; }

        public int? Reach { get; set; }

        public int? Engagement { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }
    }

}
