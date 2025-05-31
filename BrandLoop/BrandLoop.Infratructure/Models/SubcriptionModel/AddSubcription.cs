using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.SubcriptionModel
{
    public class AddSubcription
    {
        [Required]
        [StringLength(255)]
        public string SubscriptionName { get; set; }

        public int? Duration { get; set; } 

        public decimal? Price { get; set; }

        public string Description { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
