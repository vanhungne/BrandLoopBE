using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }

        [Required]
        [StringLength(255)]
        public string SubscriptionName { get; set; }

        public int? Duration { get; set; } // số ngày

        public decimal? Price { get; set; }

        public string Description { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; }
        public virtual ICollection<SubscriptionRegister> SubscriptionRegisters { get; set; }
    }
}
