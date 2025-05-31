using BrandLoop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IDealService
    {
        Task<Deal> GetDealByIdAsync(int dealId, string uid);
        Task<List<Deal>> GetAllDealsByCampaignId(int campaignId, string Uid);
        Task<List<Deal>> GetAllKolDeals(string kolUid);
        Task<Deal> UpdateDeal(int id, string description, string uid);
    }
}
