using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Shared.Helper;
using BrandLoop.Domain.Enums;

namespace BrandLoop.Domain.Entities
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        public int CampaignId { get; set; }

        [StringLength(32)]
        public string FromUserId { get; set; } // Người đánh giá

        [StringLength(32)]
        public string ToUserId { get; set; } // Người được đánh giá

        public string Description { get; set; }
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
        public int? Rating { get; set; } // CHECK (rating BETWEEN 1 AND 5)

        public FeedbackType FeedbackFrom { get; set; } // 1: Brand, 2: Influencer
        public DateTime CreatedAt { get; set; } = DateTimeHelper.GetVietnamNow();

        // Navigation properties
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }

        [ForeignKey("FromUserId")]
        public virtual User FromUser { get; set; }

        [ForeignKey("ToUserId")]
        public virtual User ToUser { get; set; }
    }
}
