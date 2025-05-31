using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using BrandLoop.Shared.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class DealRepository : IDealRepository
    {
        private readonly BLDBContext _context;
        public DealRepository(BLDBContext context)
        {
            _context = context;
        }
        public async Task<Deal> CreateDealAsync(CampaignInvitation invitation, decimal price)
        {
            var deal = new Deal
            {
                InvitationId = invitation.InvitationId,
                Description =   $"Thoả thuận hợp tác cho chiến dịch \"{invitation.Campaign.CampaignName}\".\n\n" +
                                $"Nội dung lời mời: {invitation.Message}\n\n" +
                                $"Thù lao đã thống nhất: {price:N0}đ.",
                Price = price,
                CreatedAt = DateTimeHelper.GetVietnamNow(),
                EditedAt = null,
                AdminCommissionRate = 10.0m,
                AdminCommissionAmount = price * (10.0m / 100),
                PaymentStatus = "unpaid", // Default payment status
                PaidAmount = 0
            };
            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();
            return deal;
        }

        public async Task<List<Deal>> GetAllDealsByCampaignId(int campaignId, string uid)
        {
            var deals = await _context.Deals
                .Include(d => d.Invitation)
                .ThenInclude(i => i.Campaign)
                .Where(d => d.Invitation.CampaignId == campaignId && d.Invitation.Campaign.CreatedBy == uid)
                .ToListAsync();
            return deals;
        }

        public Task<List<Deal>> GetAllKolDeals(string kolUid)
        {
            var deals = _context.Deals
                .Include(d => d.Invitation)
                .Where(d => d.Invitation.UID == kolUid)
                .ToListAsync();
            return deals;
        }

        public async Task<Deal> GetDealByIdAsync(int dealId)
        {
            var deal = await _context.Deals
                .Include(d => d.Invitation)
                .ThenInclude(i => i.Campaign)
                .FirstOrDefaultAsync(d => d.DealId == dealId);
            if (deal == null)
                throw new FileNotFoundException("Deal not found");

            return deal;
        }

        public async Task<Deal> UpdateDeal(int id, string description)
        {
            var deal = await _context.Deals.FirstOrDefaultAsync(d => d.DealId == id);
            if (deal == null)
                throw new FileNotFoundException("Deal not found");

            deal.Description = description;
            deal.EditedAt = DateTimeHelper.GetVietnamNow();
            _context.Deals.Update(deal);
            await _context.SaveChangesAsync();
            return deal;
        }
    }
}
