using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class SubscriptionFeature
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SubscriptionId { get; set; }

        public int FeatureId { get; set; }

        // Navigation properties
        [ForeignKey("SubscriptionId")]
        public virtual Subscription Subscription { get; set; }

        [ForeignKey("FeatureId")]
        public virtual Feature Feature { get; set; }
    }
}
