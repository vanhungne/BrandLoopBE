using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Configurations;

namespace BrandLoop.Infratructure.Interface
{
    public interface ICampaignRepository : IRepository<Campaign>
    {
        Task<IEnumerable<Campaign>> GetBrandCampaignsAsync(string uid);
        Task<Campaign> GetCampaignDetailAsync(int campaignId);
        Task<Campaign> CreateCampaignAsync(Campaign campaign);
        Task<Campaign> UpdateCampaignAsync(Campaign campaign);
        Task<bool> DeleteCampaignAsync(int campaignId);
        Task<Campaign> UpdateCampaignStatusAsync(int campaignId, CampainStatus status);
        Task<Campaign> DuplicateCampaignAsync(int campaignId);
        Task<int> getIdBrand(string uid);
    }
}
