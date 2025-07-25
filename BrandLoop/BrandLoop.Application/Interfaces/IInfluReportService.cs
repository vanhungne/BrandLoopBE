using BrandLoop.Infratructure.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IInfluReportService
    {
        Task FinishReport(string userId, InfluReport influReportDto);
        Task<InfluencerReportModel> GetReportByCampaignId(int reportId, string influencerUID);
        Task<List<FeedbackDTO>> GetFeedbacksOfBrandByCampaignId(int campaignId, string brandUID);
        Task<BrandFeedbackDTO> GetFeedbackOfInfluencerByCampaignId(int campaignId,string brandUID,  string influencerUID);
        Task<BrandFeedbackDTO> GetFeedbackFromBrandOfKol(int campaignId, string influencerUID);
    }
}
