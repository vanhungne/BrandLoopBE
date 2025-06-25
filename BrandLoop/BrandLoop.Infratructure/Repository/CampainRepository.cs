using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Configurations;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using BrandLoop.Shared.Helper;
using Microsoft.EntityFrameworkCore;

namespace BrandLoop.Infratructure.Repository
{
    public class CampaignRepository : Repository<Campaign>, ICampaignRepository
    {
        private readonly BLDBContext _context;

        public CampaignRepository(BLDBContext context) : base(context)
        {
            _context = context;
        }
        public async Task<int> getIdBrand(string uid) {
            return await _context.BrandProfiles
                .Where(b => b.UID == uid)
                .Select(b => b.BrandId)
                .FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<Campaign>> GetBrandCampaignsAsync(string uid)
        {
            return await _context.Campaigns.Include(c => c.CampaignImages)
                .Include(c => c.Brand)
                .Include(c => c.Creator)
                .Where(c => c.Brand.UID == uid)
                .OrderByDescending(c => c.LastUpdate)
                .ToListAsync();
        }

        public async Task<Campaign> GetCampaignDetailAsync(int campaignId)
        {
            return await _context.Campaigns
                .Include(c => c.CampaignImages)
                .Include(c => c.Brand)
                .Include(c => c.Creator)
                .Include(c => c.KolsJoinCampaigns)
                    .ThenInclude(kjc => kjc.InfluencerReport)
                .Include(c => c.KolsJoinCampaigns)
                    .ThenInclude(kjc => kjc.User)
                .Include(c => c.Feedbacks)
                .Include(c => c.CampaignInvitations)
                .Include(c => c.CampaignReports)
                .FirstOrDefaultAsync(c => c.CampaignId == campaignId);
        }

        public async Task<Campaign> CreateCampaignAsync(Campaign campaign)
        {
            await _context.Campaigns.AddAsync(campaign);
            await _context.SaveChangesAsync();
            return campaign;
        }

        public async Task<Campaign> UpdateCampaignAsync(Campaign campaign)
        {
            var existingCampaign = await _context.Campaigns.FindAsync(campaign.CampaignId);
            if (existingCampaign == null) return null;

            _context.Entry(existingCampaign).CurrentValues.SetValues(campaign);
            existingCampaign.LastUpdate = DateTime.Now;
            await _context.SaveChangesAsync();
            return existingCampaign;
        }

        public async Task<bool> DeleteCampaignAsync(int campaignId)
        {
            var campaign = await _context.Campaigns.FindAsync(campaignId);
            if (campaign == null) return false;

            campaign.Status = CampainStatus.Deleted;
            campaign.LastUpdate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Campaign> UpdateCampaignStatusAsync(int campaignId, CampainStatus status)
        {
            var campaign = await _context.Campaigns.FindAsync(campaignId);
            if (campaign == null) return null;

            campaign.Status = status;
            campaign.LastUpdate = DateTime.Now;
            await _context.SaveChangesAsync();
            return campaign;
        }

        public async Task<Campaign> DuplicateCampaignAsync(int campaignId)
        {
            var originalCampaign = await _context.Campaigns.FindAsync(campaignId);
            if (originalCampaign == null) return null;

            var duplicatedCampaign = new Campaign
            {
                BrandId = originalCampaign.BrandId,
                CampaignName = $"{originalCampaign.CampaignName} (Copy)",
                Description = originalCampaign.Description,
                ContentRequirements = originalCampaign.ContentRequirements,
                CampaignGoals = originalCampaign.CampaignGoals,
                Budget = originalCampaign.Budget,
                Deadline = originalCampaign.Deadline,
                Status = CampainStatus.Pending,
                CreatedBy = originalCampaign.CreatedBy,
                UploadedDate = DateTime.Now,
                LastUpdate = DateTime.Now
            };

            await _context.Campaigns.AddAsync(duplicatedCampaign);
            await _context.SaveChangesAsync();
            return duplicatedCampaign;
        }

        public async Task<List<Campaign>> GetAllCampaignByUid(string uid)
        {
            var camaigns = await _context.Campaigns
                .Include(c=>c.CampaignImages)
                .Include(c => c.Brand)
                .Include(c => c.Creator)
                .Where(c => c.CreatedBy == uid)
                .OrderByDescending(c => c.LastUpdate)
                .ToListAsync();
            return camaigns;
        }

        public async Task<Campaign> StartCampaign(int campaignId)
        {
            var campaign = await GetCampaignDetailAsync(campaignId);
            if (campaign == null) return null;

            if (campaign.Status != CampainStatus.Approved)
                throw new InvalidOperationException("Campaign can only be started if it is in Approved status.");

            campaign.Status = CampainStatus.Pending;
            campaign.LastUpdate = DateTimeHelper.GetVietnamNow();
            campaign.StartTime = DateTimeHelper.GetVietnamNow();
            _context.Campaigns.Update(campaign);
            await _context.SaveChangesAsync();
            return campaign;
        }
        public async Task ConfirmPaymentToStartCampaign(int campaignId)
        {
            var campaign = await GetCampaignDetailAsync(campaignId);
            if (campaign == null) return;
            if (campaign.Status != CampainStatus.Pending)
                throw new InvalidOperationException("Campaign can only be started if it is in Pending status.");

            campaign.Status = CampainStatus.InProgress;
            campaign.LastUpdate = DateTimeHelper.GetVietnamNow();
            _context.Campaigns.Update(campaign);

            // Update KolsJoinCampaigns status to Active
            var kolJoinCampaigns = await _context.KolsJoinCampaigns
                .Where(k => k.CampaignId == campaignId && k.Status == KolJoinCampaignStatus.Pending)
                .ToListAsync();
            foreach (var kolJoinCampaign in kolJoinCampaigns)
            {
                kolJoinCampaign.Status = KolJoinCampaignStatus.Active;
                _context.KolsJoinCampaigns.Update(kolJoinCampaign);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Campaign> EndCampaign(int campaignId)
        {
            var campaign = await GetCampaignDetailAsync(campaignId);
            if (campaign == null) return null;

            if (campaign.Status != CampainStatus.InProgress)
                throw new InvalidOperationException("Campaign can only be ended if it is in InProgress status.");

            campaign.Status = CampainStatus.Completed;
            campaign.LastUpdate = DateTimeHelper.GetVietnamNow();
            _context.Campaigns.Update(campaign);

            // Update KolsJoinCampaigns status to Completed
            var kolJoinCampaigns = await _context.KolsJoinCampaigns
                .Where(k => k.CampaignId == campaignId && k.Status == KolJoinCampaignStatus.Active)
                .ToListAsync();
            foreach (var kolJoinCampaign in kolJoinCampaigns)
            {
                kolJoinCampaign.Status = KolJoinCampaignStatus.Completed;
                _context.KolsJoinCampaigns.Update(kolJoinCampaign);
            }
            await _context.SaveChangesAsync();
            return campaign;
        }

        public async Task<Campaign> CancelCampaign(int campaignId)
        {
            var campaign = await GetCampaignDetailAsync(campaignId);
            if (campaign == null) return null;

            if (campaign.Status != CampainStatus.Pending)
                throw new InvalidOperationException("Campaign can only be cancel if it is in Pending status.");

            campaign.Status = CampainStatus.Completed;
            campaign.LastUpdate = DateTimeHelper.GetVietnamNow();
            _context.Campaigns.Update(campaign);
            await _context.SaveChangesAsync();
            return campaign;
        }

        public async Task<List<KolsJoinCampaign>> GetKolsJoinCampaigns(int campaignId)
        {
            var kolsJoinCampaigns = await _context.KolsJoinCampaigns
                .Include(k => k.User)
                .ThenInclude(u => u.InfluenceProfile)
                .ThenInclude(ip => ip.InfluencerType)
                .Where(k => k.CampaignId == campaignId)
                .ToListAsync();
            return kolsJoinCampaigns;
        }

        public async Task UpdateKolJoinCampaignStatus(int kolJoinCampaignId, KolJoinCampaignStatus status)
        {
            var kolJoinCampaign = await _context.KolsJoinCampaigns.FindAsync(kolJoinCampaignId);
            if (kolJoinCampaign == null) return;
            kolJoinCampaign.Status = status;
            _context.KolsJoinCampaigns.Update(kolJoinCampaign);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Campaign>> GetAllCampaignsAsync()
        {
            return await _context.Campaigns
                .Include(c => c.CampaignImages)
                .Include(c => c.Brand)
                .Include(c => c.Creator)
                .OrderByDescending(c => c.LastUpdate)
                .ToListAsync();
        }
    }

}
