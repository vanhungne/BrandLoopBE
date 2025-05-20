using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        public int CampaignId { get; set; }

        [StringLength(50)]
        public string UserName { get; set; } // người được đánh giá (KOL hoặc brand)

        public string Description { get; set; }

        public int? Rating { get; set; } // CHECK (rating BETWEEN 1 AND 5)

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? EditedAt { get; set; }

        // Navigation properties
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }
    }
}
