using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class InfluencerReport
    {
        [Key]
        public int InfluencerReportId { get; set; }

        public string TotalContent { get; set; } // Tổng số bài đăng, video, story

        public int TotalReach { get; set; } // Số người dùng duy nhất thấy nội dung

        public int TotalImpressions { get; set; } // Số lần nội dung được hiển thị

        public int TotalEngagement { get; set; } // Likes + comments + shares + saves

        public double AvgEngagementRate { get; set; } // (TotalEngagement / TotalReach) * 100

        public int TotalClicks { get; set; } // Nhấp vào link (UTM, bio)
    }
}
