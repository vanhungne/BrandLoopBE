using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class DealDTO
    {
        public int DealId { get; set; }
        public int InvitationId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public decimal AdminCommissionRate { get; set; }
        public decimal AdminCommissionAmount { get; set; }
        public string PaymentStatus { get; set; } // e.g., "unpaid", "paid"
        public decimal PaidAmount { get; set; } // Amount that has been paid
    }
}
