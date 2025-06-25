using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IInfluencerReportRepository
    {
        Task AddInfluencerReport(InfluencerReport influReport);
        Task<List<InfluencerReport>> GetReportsByCampaignId(int campaignId);
        Task<InfluencerReport> GetReportById(int reportId);
        Task AddCampaignReport(CampaignReport campaignReport);
    }
}
