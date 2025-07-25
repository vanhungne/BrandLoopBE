using BrandLoop.Domain.Enums;
using BrandLoop.Shared.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.Dashboard
{
    public class PaymentDTO
    {
        public long PaymentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class PaymentDetailDTO
    {
        public long PaymentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionCode { get; set; }
        public string? SubscriptionName { get; set; }
        public string? FullName { get; set; }
        public string? CampaignName { get; set; }
        public string? CompanyName { get; set; }
    }
}
