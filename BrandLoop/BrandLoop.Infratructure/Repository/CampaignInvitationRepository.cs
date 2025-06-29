using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.CampainModel;
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
    public class CampaignInvitationRepository : ICampaignInvitationRepository
    {
        private readonly BLDBContext _context;
        public CampaignInvitationRepository(BLDBContext context)
        {
            _context = context;
        }
        public async Task AprroveInvitation(int invitationId)
        {
            var invitation = await _context.CampaignInvitations.FirstOrDefaultAsync(i => i.InvitationId == invitationId);
            if (invitation == null)
                throw new Exception("Invitation not found");
            invitation.Status = Domain.Enums.CampaignInvitationStatus.accepted;
            _context.CampaignInvitations.Update(invitation);

            // Create kol join campaign
            var kolJoinCampaign = new KolsJoinCampaign
            {
                CampaignId = invitation.CampaignId,
                UID = invitation.UID,
                Status = KolJoinCampaignStatus.Pending,
                AppliedAt = DateTimeHelper.GetVietnamNow()
            };
            _context.KolsJoinCampaigns.Add(kolJoinCampaign);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckIsBrand(int invitationId, string uid)
        {
            var invitation = await _context.CampaignInvitations
                .Include(i => i.Campaign)
                .FirstOrDefaultAsync(i => i.InvitationId == invitationId);
            if (invitation == null)
                throw new Exception("Invitation not found");

            return invitation.Campaign.CreatedBy == uid ? true : false;
        }

        public async Task<CampaignInvitation> CreateInvitationAsync(JoinCampaign joinCampaign, JoinCampaignType type)
        {
            var invitation = new CampaignInvitation
            {
                CampaignId = joinCampaign.CampaignId,
                UID = joinCampaign.UID,
                Title = joinCampaign.Title,
                Message = joinCampaign.Message,
                ProposedRate = joinCampaign.ProposedRate,
                Status = CampaignInvitationStatus.pending
            };
            invitation.Type = type;
             _context.CampaignInvitations.Add(invitation);
            await _context.SaveChangesAsync();
            return invitation;
        }

        public async Task<List<CampaignInvitation>> GetAllInvitationsOfCampaignAsync(int campaignId, CampaignInvitationStatus status)
        {
            var invitations = await _context.CampaignInvitations
                .Include(i => i.User)
                .Include(i => i.Campaign)
                .OrderByDescending(i => i.CreatedAt)
                .Where(i => i.CampaignId == campaignId && i.Status == status)
                .ToListAsync();
            return invitations;
        }

        public async Task<List<CampaignInvitation>> GetAllInvitationsOfBrandAsync(string brandUid, CampaignInvitationStatus status)
        {
            var invitations = await _context.CampaignInvitations
                .Include(i => i.User)
                .Include(i => i.Campaign)
                .Where(i => i.Campaign.CreatedBy == brandUid && i.Status == status)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
            return invitations;
        }

        public async Task<CampaignInvitation> GetInvitationByIdAsync(int invitationId)
        {
            var invitation = await _context.CampaignInvitations
                .Include(i => i.User)
                .Include(i => i.Campaign)
                .FirstOrDefaultAsync(i => i.InvitationId == invitationId);
            return invitation ?? throw new Exception("Invitation not found");
        }

        public async Task<List<CampaignInvitation>> GetInvitationsByKOLIdAsync(string kolId, CampaignInvitationStatus status)
        {
            var invitations = await _context.CampaignInvitations
                .Where(i => i.UID == kolId && i.Status == status)
                .Include(i => i.User)
                .Include(i => i.Campaign)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
            return invitations;
        }

        public async Task<bool> IsWaitingForApprove(int campaignId, string uid)
        {
            var isWaiting = await _context.CampaignInvitations
                .AnyAsync(i => i.CampaignId == campaignId && i.UID == uid 
                && (i.Status == Domain.Enums.CampaignInvitationStatus.pending || i.Status == CampaignInvitationStatus.negotiating));
            return isWaiting;
        }

        public async Task Negotiate(InvitationResponse response)
        {
            var invitation = await _context.CampaignInvitations.FirstOrDefaultAsync(i => i.InvitationId == response.InvitationId && i.Status == CampaignInvitationStatus.pending);
            if (invitation == null)
                throw new Exception("Invitation not found or already dealed");

            invitation.NegotiateMessage = response.Message;
            invitation.Price = response.Price;
            invitation.Status = CampaignInvitationStatus.negotiating; // Trạng thái đang thương lượng
            _context.CampaignInvitations.Update(invitation);
            await _context.SaveChangesAsync();
        }

        public async Task RejectInvitation(int invitationId)
        {
            var invitation = await _context.CampaignInvitations.FirstOrDefaultAsync(i => i.InvitationId == invitationId);
            if (invitation == null)
                throw new Exception("Invitation not found");

            invitation.Status = Domain.Enums.CampaignInvitationStatus.rejected;
            _context.CampaignInvitations.Update(invitation);
            await _context.SaveChangesAsync();
        }
    }
}
