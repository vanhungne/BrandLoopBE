using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class CampaignTracking
    {
        public int CampaignId { get; set; }
        public int BrandId { get; set; }
        public string CampaignName { get; set; }
        public string Description { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public string Status { get; set; }

        // Campaign report properties
        public decimal TotalSpend { get; set; }
        public decimal TotalRevenue { get; set; }
        public int? TotalReach { get; set; } = 0;
        public int? TotalImpressions { get; set; } = 0;
        public int? TotalEngagement { get; set; } = 0;
        public int? TotalClicks { get; set; } = 0;
        public double? AvgEngagementRate { get; set; } = 0;
        public decimal? CostPerEngagement { get; set; } = 0;
        public decimal? ROAS { get; set; } = 0;

        public List<KolInCampaignTracking> kolInCampaignTrackings { get; set; } = new List<KolInCampaignTracking>();
    }
    public class KolInCampaignTracking
    {
        public int KolsJoinCampaignId { get; set; }
        public string UID { get; set; }
        public string KolName { get; set; }
        public string Status { get; set; }

        public string TotalContent { get; set; }

        public int TotalReach { get; set; }

        public int TotalImpressions { get; set; }

        public int TotalEngagement { get; set; }

        public double AvgEngagementRate { get; set; }

        public int TotalClicks { get; set; }
    }
}
