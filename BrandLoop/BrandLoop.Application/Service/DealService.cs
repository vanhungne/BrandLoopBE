using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class DealService : IDealService
    {
        private readonly IDealRepository _dealRepository;
        public DealService(IDealRepository dealRepository)
        {
            _dealRepository = dealRepository;
        }
        public async Task<List<Deal>> GetAllDealsByCampaignId(int campaignId, string brandUid)
        {
            var deals = await _dealRepository.GetAllDealsByCampaignId(campaignId, brandUid);
            return deals;
        }

        public async Task<List<Deal>> GetAllKolDeals(string kolUid)
        {
            var deals = await _dealRepository.GetAllKolDeals(kolUid);
            return deals;
        }

        public async Task<Deal> GetDealByIdAsync(int dealId, string Uid)
        {
            var deal = await _dealRepository.GetDealByIdAsync(dealId);
            var isAllowToEdit = deal.Invitation.Campaign.CreatedBy == Uid || deal.Invitation.UID == Uid;
            if (!isAllowToEdit)
                throw new UnauthorizedAccessException("You are not allowed to view this deal.");

            return deal;
        }

        public async Task<Deal> UpdateDeal(int id, string description, string Uid)
        {
            var deal = await _dealRepository.GetDealByIdAsync(id);
            var isAllowToEdit = deal.Invitation.Campaign.CreatedBy == Uid || deal.Invitation.UID == Uid;
            if (!isAllowToEdit)
                throw new UnauthorizedAccessException("You are not allowed to edit this deal.");

            return await _dealRepository.UpdateDeal(id, description);
        }
    }
}
