using Azure;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.CampainModel;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class CampaignInvitationService : ICampaignInvitationService
    {
        private readonly ICampaignInvitationRepository _campaignInvitationRepository;
        private readonly IDealRepository _dealRepository;
        private readonly ICampaignRepository _campaignRepository;
        public CampaignInvitationService(ICampaignInvitationRepository campaignInvitationRepository, IDealRepository dealRepository, ICampaignRepository campaignRepository)
        {
            _campaignInvitationRepository = campaignInvitationRepository;
            _dealRepository = dealRepository;
            _campaignRepository = campaignRepository;
        }
        public async Task ApproveInvitation(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
            var checkIsAllowed = (await _campaignInvitationRepository.CheckIsBrand(invitationId, uid)) || uid == invitation.UID;
            if (!checkIsAllowed)
                throw new AuthenticationException("You are not authorized to approve this invitation.");

            if (invitation.Status == CampaignInvitationStatus.pending)
            {
                await _campaignInvitationRepository.AprroveInvitation(invitationId);
                // Create a deal after approving the invitation
                await _dealRepository.CreateDealAsync(invitation, (decimal)invitation.ProposedRate);
            }
            else if (invitation.Status == CampaignInvitationStatus.negotiating)
            {
                await _campaignInvitationRepository.AprroveInvitation(invitationId);
                // Create a deal after approving the invitation
                await _dealRepository.CreateDealAsync(invitation, (decimal)invitation.Price);
            }
            else
            {
                throw new Exception("Invitation is not in a valid state to be approved.");
            }
        }

        public async Task<CampaignInvitation> CreateInvitationAsync(JoinCampaign joinCampaign, string uid, JoinCampaignType type)
        {
            switch (type)
            {
                case JoinCampaignType.BrandInvited: // Brand invites KOLs to the campaign
                    var campaign = await _campaignRepository.GetCampaignDetailAsync(joinCampaign.CampaignId);
                    if (campaign.CreatedBy == uid) 
                    return await _campaignInvitationRepository.CreateInvitationAsync(joinCampaign, type);

                    else
                        throw new AuthenticationException("You are not authorized to invite KOLs to this campaign.");

                default: // KOLs apply to join the campaign
                    joinCampaign.UID = uid;
                    return await _campaignInvitationRepository.CreateInvitationAsync(joinCampaign, type);
            }
        }

        public async Task<List<CampaignInvitation>> GetAllInvitationsOfCampaignAsync(int campaignId, string uid)
        {
            // Ensure the user is authorized to view the invitations for this campaign
            var campaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (campaign == null)
                throw new KeyNotFoundException("Campaign not found.");
            if(campaign.CreatedBy != uid)
                throw new AuthenticationException("You are not authorized to view invitations for this campaign.");

            return await _campaignInvitationRepository.GetAllInvitationsOfCampaignAsync(campaignId);
        }

        public async Task<CampaignInvitation> GetInvitationByIdAsync(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
            var checkIsAllowed = (await _campaignInvitationRepository.CheckIsBrand(invitationId, uid)) || uid == invitation.UID;
            if (!checkIsAllowed)
                throw new AuthenticationException("You are not authorized to view this invitation.");

            return await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
        }

        public async Task<List<CampaignInvitation>> GetInvitationsByKOLIdAsync(string kolId)
        {
            return await _campaignInvitationRepository.GetInvitationsByKOLIdAsync(kolId);
        }

        public async Task<bool> IsWaitingForApprove(int campaignId, string uid)
        {
            return await _campaignInvitationRepository.IsWaitingForApprove(campaignId, uid);
        }

        public async Task Negotiate(InvitationResponse response, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(response.InvitationId);
            var checkIsAllowed = (await _campaignInvitationRepository.CheckIsBrand(response.InvitationId, uid)) || uid == invitation.UID;
            if (!checkIsAllowed)
                throw new AuthenticationException("You are not authorized to negotiate this invitation.");
            await _campaignInvitationRepository.Negotiate(response);
        }

        public async Task RejectInvitation(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
            var checkIsAllowed = (await _campaignInvitationRepository.CheckIsBrand(invitationId, uid)) || uid == invitation.UID;
            if (!checkIsAllowed)
                throw new AuthenticationException("You are not authorized to negotiate this invitation.");
            await _campaignInvitationRepository.RejectInvitation(invitationId);
        }
    }
}
