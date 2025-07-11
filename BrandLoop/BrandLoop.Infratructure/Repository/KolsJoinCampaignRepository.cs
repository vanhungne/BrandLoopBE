﻿using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class KolsJoinCampaignRepository : IKolsJoinCampaignRepository
    {
        private readonly BLDBContext _context;
        public KolsJoinCampaignRepository(BLDBContext context)
        {
            _context = context;
        }

        public async Task<List<KolsJoinCampaign>> GetKolJoinCampaignOfInfluencer(string uid, int year)
        {
            var kolJoinCampaign = await _context.KolsJoinCampaigns
                .Include(k => k.Campaign)
                .Where(k => k.UID == uid && k.Campaign.UploadedDate.Year == year)
                .ToListAsync();
            return kolJoinCampaign;
        }

        public Task<List<KolsJoinCampaign>> GetKolsJoinCampaignsOfBrand(string brandUid, int year)
        {
            var kolsJoinCampaigns = _context.KolsJoinCampaigns
                .Include(k => k.Campaign)
                .Where(k => k.Campaign.CreatedBy == brandUid && k.Campaign.UploadedDate.Year == year)
                .ToListAsync();
            return kolsJoinCampaigns;
        }

        public async Task UpdateKolMoney(int campaignId, string uid, int money)
        {
            var kolJoinCampaign = await _context.KolsJoinCampaigns.FirstOrDefaultAsync(k => k.CampaignId == campaignId && k.UID == uid);
            if (kolJoinCampaign == null)
                throw new Exception("Kol Join Campaign not found");
            kolJoinCampaign.InfluencerEarning = money;
            _context.KolsJoinCampaigns.Update(kolJoinCampaign);
            await _context.SaveChangesAsync();
        }
    }
}
