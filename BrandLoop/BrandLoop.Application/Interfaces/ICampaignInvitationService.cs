﻿using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.CampainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface ICampaignInvitationService
    {
        // Define methods for the CampaignInvitation service here
        Task<List<InvitationDTO>> GetAllInvitationsOfCampaignAsync(int campaignId, string uid, CampaignInvitationStatus? status);
        Task<InvitationTotal> GetAllInvitationsOfBrandAsync(string brandUid, CampaignInvitationStatus? status);
        Task<InvitationTotal> GetAllRequestOfBrandAsync(string brandUid, CampaignInvitationStatus? status);
        Task<InvitationTotal> GetInvitationsByKOLIdAsync(string kolId, CampaignInvitationStatus? status);
        Task<InvitationTotal> GetRequestedByKOLIdAsync(string kolId, CampaignInvitationStatus? status);
        Task<InvitationDTO> GetInvitationByIdAsync(int invitationId, string uid);
        Task<InvitationDTO> CreateInvitationAsync(JoinCampaign joinCampaign, string uid,JoinCampaignType type);
        Task ApproveInvitation(int invitationId, string uid);
        Task Negotiate(InvitationResponse response, string uid);
        Task RejectInvitation(int invitationId, string uid);
        Task<bool> IsWaitingForApprove(int campaignId, string uid);
        Task<bool> CheckCanApprove(int invitationId, string uid);
        Task<bool> CheckCanNegotiate(int invitationId, string uid);
        Task<bool> CheckCanReject(int invitationId, string uid);
    }
}
