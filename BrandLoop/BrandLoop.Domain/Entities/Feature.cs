using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Feature
    {
        [Key]
        public int FeatureId { get; set; }

        [Required]
        [StringLength(255)]
        public string FeatureName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; }
    }
}
