using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class InfluencerReport
    {
        [Key, ForeignKey("KolsJoinCampaign")]
        public int InfluencerReportId { get; set; }

        [Required]
        [StringLength(100)] // Tối đa 100 ký tự, vì có thể ghi "3 bài đăng + 2 story"
        public string TotalContent { get; set; } // Tổng số bài đăng, video, story

        [Range(0, int.MaxValue)]
        public int TotalReach { get; set; } // Số người dùng duy nhất thấy nội dung

        [Range(0, int.MaxValue)]
        public int TotalImpressions { get; set; } // Số lần nội dung được hiển thị

        [Range(0, int.MaxValue)]
        public int TotalEngagement { get; set; } // Likes + comments + shares + saves

        [Range(0.0, 100.0)]
        public double AvgEngagementRate { get; set; } // (TotalEngagement / TotalReach) * 100

        [Range(0, int.MaxValue)]
        public int TotalClicks { get; set; } // Nhấp vào link (UTM, bio)

        public virtual KolsJoinCampaign KolsJoinCampaign { get; set; }
        public virtual ICollection<Evidence> Evidences { get; set; }
    }
}
