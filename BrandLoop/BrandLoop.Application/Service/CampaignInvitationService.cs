﻿using AutoMapper;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    // check if the influencer already in this campaign
                    var kolJoinCampaign = (await _campaignRepository.GetKolsJoinCampaigns(invitation.CampaignId)).FirstOrDefault(k => k.UID == invitation.UID);
                    if (kolJoinCampaign != null)
                        throw new Exception("This influencer has already joined this campaign.");

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
                    // Check if user is already invited to the campaign
                    var existingInvitation = await _campaignInvitationRepository.GetByCampaignAndCreatedBy(joinCampaign.CampaignId, joinCampaign.UID);
                    if (existingInvitation != null)
                        throw new Exception("You have already invited this influencer to this campaign.");
                    var campaign = await _campaignRepository.GetCampaignDetailAsync(joinCampaign.CampaignId);
                    if (campaign.CreatedBy == uid)
                    {
                        var invitation = await _campaignInvitationRepository.CreateInvitationAsync(joinCampaign, type);
                        return _mapper.Map<InvitationDTO>(invitation);
                    }

                    else
                        throw new AuthenticationException("You are not authorized to invite Influencer to this campaign.");

                default: // KOLs apply to join the campaign
                    joinCampaign.UID = uid;
                    // Check if user has already applied to the campaign
                    var existingKolInvitation = await _campaignInvitationRepository.GetByCampaignAndCreatedBy(joinCampaign.CampaignId, uid);
                    if (existingKolInvitation != null)
                        throw new Exception("You have already applied to this campaign.");
                    var kolInvitation = await _campaignInvitationRepository.CreateInvitationAsync(joinCampaign, type);
                    return _mapper.Map<InvitationDTO>(kolInvitation);
            }
        }

        public async Task<List<InvitationDTO>> GetAllInvitationsOfCampaignAsync(int campaignId, string uid, CampaignInvitationStatus? status)
        {
            // Ensure the user is authorized to view the invitations for this campaign
            var campaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (campaign == null)
                throw new KeyNotFoundException("Campaign not found.");
            if (campaign.CreatedBy != uid)
                throw new AuthenticationException("You are not authorized to view invitations for this campaign.");

            var invitations = await _campaignInvitationRepository.GetAllInvitationsOfCampaignAsync(campaignId, status);
            return _mapper.Map<List<InvitationDTO>>(invitations);
        }

        public async Task<InvitationTotal> GetAllInvitationsOfBrandAsync(string brandUid, CampaignInvitationStatus? status)
        {
            var allInvitation = await _campaignInvitationRepository.GetAllInvitationsOfBrandAsync(brandUid, null);
            var result = new InvitationTotal();
            result.totalInvitation = allInvitation.Count;
            var invitationByStatus = await _campaignInvitationRepository.GetAllInvitationsOfBrandAsync(brandUid, status);
            result.Invitations = _mapper.Map<List<InvitationDTO>>(invitationByStatus);
            foreach (var invitation in allInvitation)
            {
                if (invitation.Status == CampaignInvitationStatus.pending)
                    result.totalWaitingInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.negotiating)
                    result.totalWaitingInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.accepted)
                    result.totalAcceptedInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.rejected)
                    result.totalRejectedInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.expired)
                    result.totalExpiredInvitation++;
            }
            return result;
        }
        public async Task<InvitationTotal> GetAllRequestOfBrandAsync(string brandUid, CampaignInvitationStatus? status)
        {
            var allInvitation = await _campaignInvitationRepository.GetAllRequestedsOfBrandAsync(brandUid, null);
            var result = new InvitationTotal();
            result.totalInvitation = allInvitation.Count;
            var invitationByStatus = await _campaignInvitationRepository.GetAllRequestedsOfBrandAsync(brandUid, status);
            result.Invitations = _mapper.Map<List<InvitationDTO>>(invitationByStatus);
            foreach (var invitation in allInvitation)
            {
                if (invitation.Status == CampaignInvitationStatus.pending)
                    result.totalWaitingInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.negotiating)
                    result.totalWaitingInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.accepted)
                    result.totalAcceptedInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.rejected)
                    result.totalRejectedInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.expired)
                    result.totalExpiredInvitation++;
            }
            return result;
        }

        public async Task<InvitationDTO> GetInvitationByIdAsync(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
            var checkIsAllowed = (await _campaignInvitationRepository.CheckIsBrand(invitationId, uid)) || uid == invitation.UID;
            if (!checkIsAllowed)
                throw new AuthenticationException("You are not authorized to view this invitation.");

            return _mapper.Map<InvitationDTO>(invitation);
        }

        public async Task<InvitationTotal> GetInvitationsByKOLIdAsync(string kolId, CampaignInvitationStatus? status)
        {
            var allKolInvitation = await _campaignInvitationRepository.GetInvitationsByKOLIdAsync(kolId, null);
            var result = new InvitationTotal();
            result.totalInvitation = allKolInvitation.Count;
            var invitationByStatus = await _campaignInvitationRepository.GetInvitationsByKOLIdAsync(kolId, status);
            result.Invitations = _mapper.Map<List<InvitationDTO>>(invitationByStatus);
            foreach (var invitation in allKolInvitation)
            {
                if (invitation.Status == CampaignInvitationStatus.pending)
                    result.totalWaitingInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.negotiating)
                    result.totalWaitingInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.accepted)
                    result.totalAcceptedInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.rejected)
                    result.totalRejectedInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.expired)
                    result.totalExpiredInvitation++;
            }
            return result;
        }
        public async Task<InvitationTotal> GetRequestedByKOLIdAsync(string kolId, CampaignInvitationStatus? status)
        {
            var allKolInvitation = await _campaignInvitationRepository.GetRequestByKOLIdAsync(kolId, null);
            var result = new InvitationTotal();
            result.totalInvitation = allKolInvitation.Count;
            var invitationByStatus = await _campaignInvitationRepository.GetRequestByKOLIdAsync(kolId, status);
            result.Invitations = _mapper.Map<List<InvitationDTO>>(invitationByStatus);
            foreach (var invitation in allKolInvitation)
            {
                if (invitation.Status == CampaignInvitationStatus.pending)
                    result.totalWaitingInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.negotiating)
                    result.totalWaitingInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.accepted)
                    result.totalAcceptedInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.rejected)
                    result.totalRejectedInvitation++;
                else if (invitation.Status == CampaignInvitationStatus.expired)
                    result.totalExpiredInvitation++;
            }
            return result;
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
