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
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PaymentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTimeHelper.GetVietnamNow();

        public int Amount { get; set; }

        [StringLength(50)]
        public PaymentStatus Status { get; set; }

        [StringLength(50)]
        public PaymentType Type { get; set; }

        public int? SubscriptionRegisterId { get; set; }

        public int? CampaignId { get; set; }
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [StringLength(100)]
        public string TransactionCode { get; set; }

        // Navigation properties
        [ForeignKey("SubscriptionRegisterId")]
        public virtual SubscriptionRegister SubscriptionRegister { get; set; }

        [ForeignKey("CampaignId")]
        public virtual Campaign campaign { get; set; }
    }
}
