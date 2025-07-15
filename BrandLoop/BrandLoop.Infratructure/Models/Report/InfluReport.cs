using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.Report
{
    public class InfluReport
    {
        public int CampaignId { get; set; } // ID của chiến dịch
        [Required]
        [StringLength(100)] // Tối đa 100 ký tự, vì có thể ghi "3 bài đăng + 2 story"
        public string TotalContent { get; set; } // Tổng số bài đăng, video, story

        [Range(0, int.MaxValue)]
        public int TotalReach { get; set; } // Số người dùng duy nhất thấy nội dung

        [Range(0, int.MaxValue)]
        public int TotalImpressions { get; set; } // Số lần nội dung được hiển thị

        [Range(0, int.MaxValue)]
        public int TotalEngagement { get; set; } // Likes + comments + shares + saves

        [Range(0, int.MaxValue)]
        public int TotalClicks { get; set; } // Nhấp vào link (UTM, bio)

        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
        public int? Rating { get; set; }
        public string Description { get; set; }
        public List<EvidenceDTO> Evidences { get; set; } = new List<EvidenceDTO>();
    }
    public class EvidenceDTO
    {
        public string Description { get; set; }
        public string Link { get; set; }
    }
    public class InfluencerReportModel : InfluReport
    {
        public double AvgEngagementRate { get; set; }
        public FeedbackDTO Feedback { get; set; } = new FeedbackDTO();
    }
}
