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
                throw new AuthenticationException("Bạn không có quyền để chấp nhận lời mời này.");

            if (invitation.Status == CampaignInvitationStatus.pending)
            {
                if (await CheckCanApprove(invitationId, uid))
                {
                    // check if the influencer already in this campaign
                    var kolJoinCampaign = (await _campaignRepository.GetKolsJoinCampaigns(invitation.CampaignId)).FirstOrDefault(k => k.UID == invitation.UID);
                    if (kolJoinCampaign != null)
                        throw new Exception("Influencer này đã tham gia chiến dịch.");

                    await _campaignInvitationRepository.AprroveInvitation(invitationId);
                    // Create a deal after approving the invitation
                    await _dealRepository.CreateDealAsync(invitation, (decimal)invitation.ProposedRate);
                }
                else
                    throw new Exception("Bạn không thể chấp nhận lời mời này.");
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
                    throw new Exception("Bạn không thể chấp nhận lời mời này.");
            }
            else
                throw new Exception("Lời mời này có thể đã được chấp nhận hoặc từ chối.");
        }

        public async Task<InvitationDTO> CreateInvitationAsync(JoinCampaign joinCampaign, string uid, JoinCampaignType type)
        {
            switch (type)
            {
                case JoinCampaignType.BrandInvited: // Brand invites KOLs to the campaign
                    // Check if user is already invited to the campaign
                    var existingInvitation = await _campaignInvitationRepository.GetByCampaignAndCreatedBy(joinCampaign.CampaignId, joinCampaign.UID);
                    if (existingInvitation != null)
                        throw new Exception("Bạn đã được mời vào chiến dịch này.");
                    var campaign = await _campaignRepository.GetCampaignDetailAsync(joinCampaign.CampaignId);
                    if (campaign.CreatedBy == uid)
                    {
                        var invitation = await _campaignInvitationRepository.CreateInvitationAsync(joinCampaign, type);
                        return _mapper.Map<InvitationDTO>(invitation);
                    }

                    else
                        throw new AuthenticationException("Bạn không có quyền mời Influencer vào chiến dịch này.");

                default: // KOLs apply to join the campaign
                    joinCampaign.UID = uid;
                    // Check if user has already applied to the campaign
                    var existingKolInvitation = await _campaignInvitationRepository.GetByCampaignAndCreatedBy(joinCampaign.CampaignId, uid);
                    if (existingKolInvitation != null)
                        throw new Exception("Bạn đã được mời vào chiến dịch này.");
                    var kolInvitation = await _campaignInvitationRepository.CreateInvitationAsync(joinCampaign, type);
                    return _mapper.Map<InvitationDTO>(kolInvitation);
            }
        }

        public async Task<List<InvitationDTO>> GetAllInvitationsOfCampaignAsync(int campaignId, string uid, CampaignInvitationStatus? status)
        {
            // Ensure the user is authorized to view the invitations for this campaign
            var campaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (campaign == null)
                throw new KeyNotFoundException("Không thể tìm thấy campaign.");
            if (campaign.CreatedBy != uid)
                throw new AuthenticationException("Bạn không có quyền để xem các lời mời của chiến dịch này.");

            var invitations = await _campaignInvitationRepository.GetAllInvitationsOfCampaignAsync(campaignId, status);
            return _mapper.Map<List<InvitationDTO>>(invitations);
        }

        public async Task<InvitationTotal> GetAllInvitationsOfBrandAsync(string brandUid, CampaignInvitationStatus? status)
        {
            var allInvitation = await _campaignInvitationRepository.GetAllInvitationsOfBrandAsync(brandUid, null);
            var result = new InvitationTotal();
            result.totalInvitation = allInvitation.Count;
            var invitationByStatus = await _campaignInvitationRepository.GetAllInvitationsOfBrandAsync(brandUid, status);
            result.Invitations = _mapper.Map<List<InvitationOfBrand>>(invitationByStatus);
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
            result.Invitations = _mapper.Map<List<InvitationOfBrand>>(invitationByStatus);
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
                throw new AuthenticationException("Bạn không có quyền để xem lời mời này.");

            return _mapper.Map<InvitationDTO>(invitation);
        }

        public async Task<InvitationTotal> GetInvitationsByKOLIdAsync(string kolId, CampaignInvitationStatus? status)
        {
            var allKolInvitation = await _campaignInvitationRepository.GetInvitationsByKOLIdAsync(kolId, null);
            var result = new InvitationTotal();
            result.totalInvitation = allKolInvitation.Count;
            var invitationByStatus = await _campaignInvitationRepository.GetInvitationsByKOLIdAsync(kolId, status);
            result.Invitations = _mapper.Map<List<InvitationOfBrand>>(invitationByStatus);
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
            result.Invitations = _mapper.Map<List<InvitationOfBrand>>(invitationByStatus);
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
            if (invitation == null || invitation.Campaign == null)
                throw new KeyNotFoundException("Không thể tìm thấy lời mời này hoặc chiến dịch liên quan.");

            var checkIsAllowed = (await _campaignInvitationRepository.CheckIsBrand(response.InvitationId, uid)) || uid == invitation.UID;

            if (!checkIsAllowed)
                throw new AuthenticationException("Bạn không có quyền để đàm phán cho giao dịch này.");
            if (!await CheckCanNegotiate(invitation.InvitationId, uid))
                throw new Exception("Lời mời này hiện không thể đàm phán. Vui lòng kiểm tra trạng thái lời mời hoặc liên hệ quản trị viên.");

            await _campaignInvitationRepository.Negotiate(response);
        }

        public async Task RejectInvitation(int invitationId, string uid)
        {
            var invitation = await _campaignInvitationRepository.GetInvitationByIdAsync(invitationId);
            if (invitation == null || invitation.Campaign == null)
                throw new KeyNotFoundException("Không thể tìm thấy lời mời này hoặc chiến dịch liên quan.");
            var checkIsAllowed = (await _campaignInvitationRepository.CheckIsBrand(invitationId, uid)) || uid == invitation.UID;
            if (!checkIsAllowed)
                throw new AuthenticationException("Bạn không có quyền để từ chối lời mời này.");

            if (!await CheckCanReject(invitationId, uid))
                throw new Exception("Lời mời này hiện không thể từ chối. Vui lòng kiểm tra trạng thái lời mời hoặc liên hệ quản trị viên.");
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
