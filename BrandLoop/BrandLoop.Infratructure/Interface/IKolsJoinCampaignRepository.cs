using BrandLoop.Domain.Entities;
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
        Task<List<KolsJoinCampaign>> GetKolsJoinCampaignsOfBrand(string brandUid, int year);
        Task<List<KolsJoinCampaign>> GetKolJoinCampaignOfInfluencer(string uid, int year);
    }
}
