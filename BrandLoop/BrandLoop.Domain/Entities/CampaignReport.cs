using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Shared.Helper;

namespace BrandLoop.Domain.Entities
{
    public class CampaignReport
    {
        [Key]
        public int CampaignReportId { get; set; }

        [Required]
        public int CampaignId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalSpend { get; set; } // Tổng chi tiêu

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalRevenue { get; set; } // Doanh thu

        [Range(0, int.MaxValue)]
        public int? TotalReach { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int? TotalImpressions { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int? TotalEngagement { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int? TotalClicks { get; set; } = 0;

        [Range(0.0, 100.0)]
        public double? AvgEngagementRate { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal? CostPerEngagement { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal? ROAS { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTimeHelper.GetVietnamNow();

        // Navigation
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }
    }

}
