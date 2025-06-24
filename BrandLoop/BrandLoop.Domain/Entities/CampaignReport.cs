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

        public int CampaignId { get; set; }
        public decimal TotalSpend { get; set; } // Tổng chi tiêu
        public decimal TotalRevenue { get; set; } // Doanh thu

        public int? TotalReach { get; set; } = 0; // Số người dùng duy nhất tiếp cận nội dung
        public int? TotalImpressions { get; set; } = 0; // Số lần nội dung được hiển thị
        public int? TotalEngagement { get; set; } = 0; // Tổng tương tác (likes + comments + shares + saves)
        public int? TotalClicks { get; set; } = 0; // Nhấp vào link (UTM, bio)
        public double? AvgEngagementRate { get; set; } = 0; // (TotalEngagement / TotalReach) * 100
        public decimal? CostPerEngagement { get; set; } = 0; // TotalSpend / TotalEngagement
        public decimal? ROAS { get; set; } = 0;// Revenue / TotalSpend
        public DateTime CreatedAt { get; set; } = DateTimeHelper.GetVietnamNow();

        // Navigation properties
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }
    }

}
