using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.SubcriptionModel
{
    public class PayOutModel
    {
        public long PaymentId { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public int? CampaignId { get; set; }
    }
}
