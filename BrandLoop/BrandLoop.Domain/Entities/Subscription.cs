using BrandLoop.Shared.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubscriptionId { get; set; }

        [Required]
        [StringLength(255)]
        public string SubscriptionName { get; set; }

        public int? Duration { get; set; } // số ngày

        public decimal? Price { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTimeHelper.GetVietnamNow();
        public bool isDeleted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; }
        public virtual ICollection<SubscriptionRegister> SubscriptionRegisters { get; set; }
    }
}
