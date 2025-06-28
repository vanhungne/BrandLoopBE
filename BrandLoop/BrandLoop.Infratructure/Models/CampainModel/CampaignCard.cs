using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class CampaignCard
    {
        public int totalCampaigns { get; set; }
        public int totalInprogressCampaigns { get; set; }
        public int totalUnstartedCampaigns { get; set; }
        public int totalCompletedCampaigns { get; set; }
        public int totalOverdueCampaigns { get; set; }
        public int totalUnpaidCampaigns { get; set; }
    }

    public class CampaignChart
    {
        public int month { get; set; }
        public int approvedCampaigns { get; set; }
        public int inprogressCampaigns { get; set; }
        public int completedCampaigns { get; set; }
    }
    public class CampaignSelectOption
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
    }

    public class CampaignDashboardDetail
    {
        public DateTime? StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public decimal? Budget { get; set; }
        public string Status { get; set; }

        public int totalKolsJoin { get; set; } = 0;
        public int totalInvitations { get; set; } = 0;
        public int totalAcceptedInvitations { get; set; } = 0;
        public int totalRejectedInvitations { get; set; } = 0;
        public int othersInvitations { get; set; } = 0;
        public decimal TotalSpend { get; set; } = 0;
        public decimal TotalRevenue { get; set; } = 0;
        public int? TotalReach { get; set; } = 0;
        public int? TotalImpressions { get; set; } = 0;
        public int? TotalEngagement { get; set; } = 0;
        public int? TotalClicks { get; set; } = 0;
        public double? AvgEngagementRate { get; set; } = 0;
        public decimal? CostPerEngagement { get; set; } = 0;
        public decimal? ROAS { get; set; } = 0;
        public List<ShowFeedback> feedbacks { get; set; } = new List<ShowFeedback>();
    }

    public class CampaignReportOfInfluencer {
        public DateTime? StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public decimal? Budget { get; set; }
        public string Status { get; set; }

        public int totalKolsJoin { get; set; } = 0;
        public int totalInvitations { get; set; } = 0;
        public int totalAcceptedInvitations { get; set; } = 0;
        public int totalRejectedInvitations { get; set; } = 0;
        public int othersInvitations { get; set; } = 0;
        public decimal TotalSpend { get; set; } = 0;
        public decimal TotalRevenue { get; set; } = 0;
        public int? TotalReach { get; set; } = 0;
        public int? TotalImpressions { get; set; } = 0;
        public int? TotalEngagement { get; set; } = 0;
        public int? TotalClicks { get; set; } = 0;
        public double? AvgEngagementRate { get; set; } = 0;
        public decimal? CostPerEngagement { get; set; } = 0;
        public decimal? ROAS { get; set; } = 0;

        public KolInCampaignTracking? report { get; set; }
        public ShowFeedback? FeedbackOfInfluencer { get; set; }
    }
}