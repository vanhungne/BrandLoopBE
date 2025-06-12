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
    public class Feature
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeatureId { get; set; }

        [Required]
        [StringLength(255)]
        public string FeatureName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTimeHelper.GetVietnamNow();

        // Navigation properties
        public virtual ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; }
    }
}
