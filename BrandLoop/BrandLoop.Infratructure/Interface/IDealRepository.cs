using BrandLoop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IDealRepository
    {
        Task<Deal> GetDealByIdAsync(int dealId);
        Task<Deal> CreateDealAsync(CampaignInvitation invitation, decimal price);
        Task<List<Deal>> GetAllDealsByCampaignId(int campaignId, string brandUid);
        Task<List<Deal>> GetAllKolDeals(string kolUid);
        Task<Deal> UpdateDeal(int id, string description);
    }
}
