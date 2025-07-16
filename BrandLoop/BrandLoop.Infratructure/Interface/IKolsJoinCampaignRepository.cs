using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IKolsJoinCampaignRepository
    {
        Task UpdateKolMoney(int campaignId, string uid, int money);
        Task UpdateKolJoinCampaignStatus(int campaignId, string uid, KolJoinCampaignStatus status);
        Task<List<KolsJoinCampaign>> GetKolsJoinCampaignsOfBrand(string brandUid, int year);
        Task<List<KolsJoinCampaign>> GetKolJoinCampaignOfInfluencer(string uid, int year);
        Task<Dictionary<int, int>> GetKolsCountByCampaignIdsAsync(List<int> campaignIds);
    }
}
