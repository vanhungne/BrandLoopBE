using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.SubcriptionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IInfluencerDashboardService
    {
        Task<CampaignCard> GetCampaignCardAsync(string uid);
        Task<List<CampaignChart>> GetRevenueChart(string uid, int year);
        Task<List<SubscriptionRegisterDTO>> GetActiveSubscriptionRegisterAsync(string uid);
        Task<List<CampaignSelectOption>> GetCampaignSelectOption(string uid, CampaignStatus status);
        Task<CampaignReportOfInfluencer> GetCampaignReportOfInfluencerAsync(string uid, int campaignId);
    }
}
