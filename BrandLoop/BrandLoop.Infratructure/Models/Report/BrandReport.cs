using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.Report
{
    public class BrandReport
    {
        [Required]
        public int CampaignId { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalSpend { get; set; } // Tổng chi tiêu

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalRevenue { get; set; } // Doanh thu
    }
}
