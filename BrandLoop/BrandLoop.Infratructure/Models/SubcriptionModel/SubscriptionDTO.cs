using BrandLoop.Shared.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.SubcriptionModel
{
    public class SubscriptionDTO
    {
        public int SubscriptionId { get; set; }

        public string SubscriptionName { get; set; }

        public int? Duration { get; set; } // số ngày

        public decimal? Price { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
