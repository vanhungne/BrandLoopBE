using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class InvitationDTO
    {
        public int InvitationId { get; set; }
        public int CampaignId { get; set; }
        public string UID { get; set; } // KOL được mời
        public string Title { get; set; }
        public string Message { get; set; }
        public string? NegotiateMessage { get; set; }
        public decimal? ProposedRate { get; set; }
        public string Status { get; set; } // CampaignInvitationStatus
        public DateTime CreatedAt { get; set; }
        public decimal? Price { get; set; }
        public string Type { get; set; } // JoinCampaignType
        public string Email { get; set; } // Email của KOL
        public string FullName { get; set; } // Tên đầy đủ của KOL
        public string ProfileImage { get; set; } // Hình ảnh đại diện của KOL
    }

    public class InvitationOfBrand : InvitationDTO
    {
        public string BrandName { get; set; } // Tên thương hiệu
    }

    public class InvitationTotal
    {
        public int totalInvitation { get; set; } = 0;
        public int totalWaitingInvitation { get; set; } = 0;
        public int totalAcceptedInvitation { get; set; } = 0;
        public int totalRejectedInvitation { get; set; } = 0;
        public int totalExpiredInvitation { get; set; } = 0;

        public List<InvitationOfBrand> Invitations { get; set; } = new List<InvitationOfBrand>();
    }
}
