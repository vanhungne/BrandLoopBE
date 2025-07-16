using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Enums;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class CampaignDto
    {
        public int CampaignId { get; set; }
        public int BrandId { get; set; }
        public string CampaignName { get; set; }
        public string Description { get; set; }
        public DateTime UploadedDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string ContentRequirements { get; set; }
        public string CampaignGoals { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public CampaignStatus Status { get; set; }
        public string CreatedBy { get; set; }

        public string BrandIndustry { get; set; }
        public int TotalKolsJoined { get; set; }

        public List<CampaignImageDto> Images { get; set; } = new List<CampaignImageDto>();
    }

    public class CampaignDtoVer2 : CampaignDto
    {
        public List<KoljoinStatus> KoljoinStatuses { get; set; } = new List<KoljoinStatus>();
    }
    public class KoljoinStatus
    {
        public string UID { get; set; }
        public string Status { get; set; }
    }

    public class CampaignDtoDetail
    {
        public int CampaignId { get; set; }
        public int BrandId { get; set; }
        public string CampaignName { get; set; }
        public string Description { get; set; }
        public DateTime UploadedDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string ContentRequirements { get; set; }
        public string CampaignGoals { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public CampaignStatus Status { get; set; }
        public string CreatedBy { get; set; }

        public string BrandIndustry { get; set; }
        public int TotalKolsJoined { get; set; }

        public List<CampaignImageDto> Images { get; set; } = new List<CampaignImageDto>();
        public BrandInfoDto BrandInfo { get; set; }
    }

    public class BrandInfoDto
    {
        public string Logo { get; set; }
        public string Avatar { get; set; }
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    public class PaymentCampaign
    {
        public long PaymentId { get; set; }
        public int Amount { get; set; }
        public string paymentType { get; set; }
        public int CampaignId { get; set; }
        public int BrandId { get; set; }
        public string CampaignName { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public string Status { get; set; }
        public string PaymentLink { get; set; }
    }
}
