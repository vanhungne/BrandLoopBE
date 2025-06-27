using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class CampaignFilterModel
    {
        public string? Search { get; set; }
        public CampaignStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SortBy { get; set; } = "LastUpdate";
        public bool SortDesc { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
