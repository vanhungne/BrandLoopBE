using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Deal
    {
        [Key]
        public int DealId { get; set; }

        public int InvitationId { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "pending"; // pending, accepted, rejected, completed

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? EditedAt { get; set; }
        // Tỷ lệ hoa hồng cho admin
        [Column(TypeName = "decimal(5,2)")]
        public decimal? AdminCommissionRate { get; set; } = 10.0m; // Mặc định 10%

        // Số tiền hoa hồng cho admin
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AdminCommissionAmount { get; set; }

        // Trạng thái thanh toán: unpaid, partially_paid, paid
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "unpaid";

        // Số tiền đã thanh toán
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PaidAmount { get; set; } = 0;

        // Navigation properties
        [ForeignKey("InvitationId")]
        public virtual CampaignInvitation Invitation { get; set; }
    }
}
