using BrandLoop.Domain.Enums;
using BrandLoop.Shared.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.Report
{
    public class CreateFeedback
    {
        public int CampaignId { get; set; }

        [StringLength(32)]
        public string ToUserId { get; set; } // Người được đánh giá
        public string Description { get; set; }

        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
        public int? Rating { get; set; } // CHECK (rating BETWEEN 1 AND 5)
        public string EvidenceDescription { get; set; }
        public string EvidenceLink { get; set; }
        public int InfluencerMoney { get; set; } // Số tiền influencer nhận được từ chiến dịch
    }

    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }

        public int CampaignId { get; set; }
        public string FromUserId { get; set; } // Người đánh giá
        public string ToUserId { get; set; } // Người được đánh giá

        public string Description { get; set; }
        public int? Rating { get; set; } // CHECK (rating BETWEEN 1 AND 5)

        public string FeedbackFrom { get; set; }
        public string FeedbackTo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTimeHelper.GetVietnamNow();
    }

    public class ShowFeedback
    {
        public int FeedbackId { get; set; }
        public string FromUserId { get; set; }
        public string FromUserName { get; set; } // Tên người đánh giá
        public string Description { get; set; }
        public int? Rating { get; set; }
    }
}
