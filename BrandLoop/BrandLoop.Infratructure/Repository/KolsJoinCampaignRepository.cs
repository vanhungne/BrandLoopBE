using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
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
                throw new Exception("Không tìm thấy Influencer tham gia chiến dịch này.");
            kolJoinCampaign.InfluencerEarning = money;
            _context.KolsJoinCampaigns.Update(kolJoinCampaign);
            await _context.SaveChangesAsync();
        }
        public async Task<Dictionary<int, int>> GetKolsCountByCampaignIdsAsync(List<int> campaignIds)
        {
            var kolCounts = await _context.KolsJoinCampaigns
                .Where(k => campaignIds.Contains(k.CampaignId))
                .GroupBy(k => k.CampaignId)
                .Select(g => new { CampaignId = g.Key, Count = g.Count() })
                .ToListAsync();

            return kolCounts.ToDictionary(k => k.CampaignId, k => k.Count);
        }

        public async Task UpdateKolJoinCampaignStatus(int campaignId, string uid, KolJoinCampaignStatus status)
        {
            var kolJoinCampaign = await _context.KolsJoinCampaigns.FirstOrDefaultAsync(k => k.CampaignId == campaignId && k.UID == uid);
            if (kolJoinCampaign == null)
                throw new Exception("Không tìm thấy Influencer tham gia chiến dịch này.");

            kolJoinCampaign.Status = status;
            _context.KolsJoinCampaigns.Update(kolJoinCampaign);
            await _context.SaveChangesAsync();
        }
    }
}
