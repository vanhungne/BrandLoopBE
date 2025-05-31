using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.CampainModel;
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
        private readonly IMapper _mapper;
        public DealService(IDealRepository dealRepository, IMapper mapper)
        {
            _dealRepository = dealRepository;
            _mapper = mapper;
        }
        public async Task<List<DealDTO>> GetAllDealsByCampaignId(int campaignId, string brandUid)
        {
            var deals = await _dealRepository.GetAllDealsByCampaignId(campaignId, brandUid);
            return _mapper.Map<List<DealDTO>>(deals);
        }

        public async Task<List<DealDTO>> GetAllKolDeals(string kolUid)
        {
            var deals = await _dealRepository.GetAllKolDeals(kolUid);
            return _mapper.Map<List<DealDTO>>(deals);
        }

        public async Task<DealDTO> GetDealByIdAsync(int dealId, string Uid)
        {
            var deal = await _dealRepository.GetDealByIdAsync(dealId);
            var isAllowToEdit = deal.Invitation.Campaign.CreatedBy == Uid || deal.Invitation.UID == Uid;
            if (!isAllowToEdit)
                throw new UnauthorizedAccessException("You are not allowed to view this deal.");

            return _mapper.Map<DealDTO>(deal);
        }

        public async Task<DealDTO> UpdateDeal(int id, string description, string Uid)
        {
            var deal = await _dealRepository.GetDealByIdAsync(id);
            var isAllowToEdit = deal.Invitation.Campaign.CreatedBy == Uid || deal.Invitation.UID == Uid;
            if (!isAllowToEdit)
                throw new UnauthorizedAccessException("You are not allowed to edit this deal.");

            var updated = await _dealRepository.UpdateDeal(id, description);
            return _mapper.Map<DealDTO>(updated);
        }
    }
}
