using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.CampainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IDealService
    {
        Task<DealDTO> GetDealByIdAsync(int dealId, string uid);
        Task<List<DealDTO>> GetAllDealsByCampaignId(int campaignId, string Uid);
        Task<List<DealDTO>> GetAllKolDeals(string kolUid);
        Task<DealDTO> UpdateDeal(int id, string description, string uid);
    }
}
