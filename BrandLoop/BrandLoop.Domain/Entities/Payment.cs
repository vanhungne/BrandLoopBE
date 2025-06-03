using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Payment
    {
        [Key]
        public long PaymentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal? Amount { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(50)]
        public string Type { get; set; } // subscription, campaign

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
