using AutoMapper;
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
        private readonly IMapper _mapper;
        public CampaignInvitationService(ICampaignInvitationRepository campaignInvitationRepository, IDealRepository dealRepository, ICampaignRepository campaignRepository, IMapper mapper)
        {
            _campaignInvitationRepository = campaignInvitationRepository;
            _dealRepository = dealRepository;
            _campaignRepository = campaignRepository;
            _mapper = mapper;
        }
        public async Task ApproveInvitation(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
            var checkIsAllowed = (await _campaignInvitationRepository.CheckIsBrand(invitationId, uid)) || uid == invitation.UID;
            if (!checkIsAllowed)
                throw new AuthenticationException("You are not authorized to approve this invitation.");

            if (invitation.Status == CampaignInvitationStatus.pending)
            {
                if (await CheckCanApprove(invitationId, uid))
                {
                    await _campaignInvitationRepository.AprroveInvitation(invitationId);
                    // Create a deal after approving the invitation
                    await _dealRepository.CreateDealAsync(invitation, (decimal)invitation.ProposedRate);
                }
                else
                    throw new Exception("You are not allowed to approve this invitation in its current state.");
            }
            else if (invitation.Status == CampaignInvitationStatus.negotiating)
            {
                if (await CheckCanApprove(invitationId, uid))
                {
                    await _campaignInvitationRepository.AprroveInvitation(invitationId);
                    // Create a deal after approving the invitation
                    await _dealRepository.CreateDealAsync(invitation, (decimal)invitation.Price);
                }
                else
                    throw new Exception("You are not allowed to approve this invitation in its current state.");
            }
            else
                throw new Exception("Invitation is not in a valid state to be approved.");
        }

        public async Task<InvitationDTO> CreateInvitationAsync(JoinCampaign joinCampaign, string uid, JoinCampaignType type)
        {
            switch (type)
            {
                case JoinCampaignType.BrandInvited: // Brand invites KOLs to the campaign
                    var campaign = await _campaignRepository.GetCampaignDetailAsync(joinCampaign.CampaignId);
                    if (campaign.CreatedBy == uid)
                    {
                        var invitation = await _campaignInvitationRepository.CreateInvitationAsync(joinCampaign, type);
                        return _mapper.Map<InvitationDTO>(invitation);
                    }

                    else
                        throw new AuthenticationException("You are not authorized to invite KOLs to this campaign.");

                default: // KOLs apply to join the campaign
                    joinCampaign.UID = uid;
                    var kolInvitation = await _campaignInvitationRepository.CreateInvitationAsync(joinCampaign, type);
                    return _mapper.Map<InvitationDTO>(kolInvitation);
            }
        }

        public async Task<List<InvitationDTO>> GetAllInvitationsOfCampaignAsync(int campaignId, string uid)
        {
            // Ensure the user is authorized to view the invitations for this campaign
            var campaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (campaign == null)
                throw new KeyNotFoundException("Campaign not found.");
            if (campaign.CreatedBy != uid)
                throw new AuthenticationException("You are not authorized to view invitations for this campaign.");

            var invitations = await _campaignInvitationRepository.GetAllInvitationsOfCampaignAsync(campaignId);
            return _mapper.Map<List<InvitationDTO>>(invitations);
        }

        public async Task<InvitationDTO> GetInvitationByIdAsync(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
            var checkIsAllowed = (await _campaignInvitationRepository.CheckIsBrand(invitationId, uid)) || uid == invitation.UID;
            if (!checkIsAllowed)
                throw new AuthenticationException("You are not authorized to view this invitation.");

            return _mapper.Map<InvitationDTO>(invitation);
        }

        public async Task<List<InvitationDTO>> GetInvitationsByKOLIdAsync(string kolId)
        {
            var kolInvitation = await _campaignInvitationRepository.GetInvitationsByKOLIdAsync(kolId);
            return _mapper.Map<List<InvitationDTO>>(kolInvitation);
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
            if (!await CheckCanNegotiate(invitation.InvitationId, uid))
                throw new Exception("You are not allowed to negotiate this invitation.");

            await _campaignInvitationRepository.Negotiate(response);
        }

        public async Task RejectInvitation(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
            var checkIsAllowed = (await _campaignInvitationRepository.CheckIsBrand(invitationId, uid)) || uid == invitation.UID;
            if (!checkIsAllowed)
                throw new AuthenticationException("You are not authorized to negotiate this invitation.");

            if (!await CheckCanReject(invitationId, uid))
                throw new Exception("You are not allowed to reject this invitation.");
            await _campaignInvitationRepository.RejectInvitation(invitationId);
        }

        public async Task<bool> CheckCanApprove(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);

            if (invitation == null || invitation.Campaign == null)
                return false;

            var isKOL = invitation.UID == uid;
            var isBrand = invitation.Campaign.CreatedBy == uid;

            return
                // Case 1: BrandInvited, Status = pending, KOL approves
                (invitation.Type == JoinCampaignType.BrandInvited && invitation.Status == CampaignInvitationStatus.pending && isKOL)
                ||
                // Case 2: BrandInvited, Status = negotiating, Brand approves
                (invitation.Type == JoinCampaignType.BrandInvited && invitation.Status == CampaignInvitationStatus.negotiating && isBrand)
                ||
                // Case 3: BrandApplied, Status = pending, Brand approves
                (invitation.Type == JoinCampaignType.KolApplied && invitation.Status == CampaignInvitationStatus.pending && isBrand)
                ||
                // Case 4: BrandApplied, Status = negotiating, KOL approves
                (invitation.Type == JoinCampaignType.KolApplied && invitation.Status == CampaignInvitationStatus.negotiating && isKOL);
        }


        public async Task<bool> CheckCanNegotiate(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
            if (invitation == null || invitation.Campaign == null || invitation.Status != CampaignInvitationStatus.pending)
                return false;

            var isKOL = invitation.UID == uid;
            var isBrand = invitation.Campaign.CreatedBy == uid;
            return
                // Case 1: BrandInvited, KOL negotiates
                (invitation.Type == JoinCampaignType.BrandInvited && isKOL)
                ||
                // Case 2: KolApplied, Brand negotiates
                (invitation.Type == JoinCampaignType.KolApplied && isBrand);
        }

        public async Task<bool> CheckCanReject(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
            if (invitation == null || invitation.Campaign == null)
                return false;
            var isKOL = invitation.UID == uid;
            var isBrand = invitation.Campaign.CreatedBy == uid;
            return
                // Case 1: BrandInvited, KOL rejects
                (invitation.Type == JoinCampaignType.BrandInvited && isKOL && invitation.Status == CampaignInvitationStatus.pending)
                ||
                // Case 2: BrandInvited, KOL negotiates, Brand rejects
                (invitation.Type == JoinCampaignType.BrandInvited && isBrand && invitation.Status == CampaignInvitationStatus.negotiating)
                ||
                // Case 3: KolApplied, Brand rejects
                (invitation.Type == JoinCampaignType.KolApplied && isBrand && invitation.Status == CampaignInvitationStatus.pending)
                ||
                // Case 4: KolApplied, Brand negotiates, KOL rejects
                (invitation.Type == JoinCampaignType.KolApplied && isKOL && invitation.Status == CampaignInvitationStatus.negotiating);
        }
    }
}
