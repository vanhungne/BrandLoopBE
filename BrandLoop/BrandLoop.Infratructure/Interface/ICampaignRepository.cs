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
        Task<Campaign> UpdateCampaignStatusAsync(int campaignId, CampaignStatus status);
        Task<Campaign> DuplicateCampaignAsync(int campaignId);
        Task<int> getIdBrand(string uid);
        Task<List<Campaign>> GetAllCampaignByUid(string uid);
        Task<List<Campaign>> GetAllCampaignByUid(CampaignStatus? status,string? name, string uid);
        Task<Campaign> StartCampaign(int campaignId);
        Task ConfirmPaymentToStartCampaign(int campaignId);
        Task<Campaign> EndCampaign(int campaignId);
        Task<Campaign> CancelCampaign(int campaignId);
        Task<List<KolsJoinCampaign>> GetKolsJoinCampaigns(int campaignId);
        Task<List<Campaign>> GetAllCampaignsOfBrandWithStatus(string uid, CampaignStatus status);
        Task UpdateKolJoinCampaignStatus(int kolJoinCampaignId, KolJoinCampaignStatus status);
        Task<List<Campaign>> GetBrandCampaignsByYear(string uid, int year);
        Task<List<Campaign>> GetAllCampaignsInfluJoined(string uid);
        Task<List<Campaign>> GetAllCampaignsInfluJoined(string uid, int year);
        Task<List<Campaign>> GetAllCampaignsInfluJoinedWithStatus(string uid, CampaignStatus status);

    }
}
