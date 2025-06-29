using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.CampainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface ICampaignInvitationRepository
    {
        // Define methods for the CampaignInvitation repository here
        Task<List<CampaignInvitation>> GetAllInvitationsOfCampaignAsync(int campaignId, CampaignInvitationStatus status);
        Task<List<CampaignInvitation>> GetAllInvitationsOfBrandAsync(string brandUid, CampaignInvitationStatus status);
        Task<List<CampaignInvitation>> GetInvitationsByKOLIdAsync(string kolId, CampaignInvitationStatus status);
        Task<CampaignInvitation> GetInvitationByIdAsync(int invitationId);
        Task<CampaignInvitation> CreateInvitationAsync(JoinCampaign joinCampaign, JoinCampaignType type);
        Task AprroveInvitation(int invitationId);
        Task Negotiate(InvitationResponse response);
        Task RejectInvitation(int invitationId);
        Task<bool> IsWaitingForApprove(int campaignId, string uid);
        Task<bool> CheckIsBrand(int campaignId, string uid);
    }
}
