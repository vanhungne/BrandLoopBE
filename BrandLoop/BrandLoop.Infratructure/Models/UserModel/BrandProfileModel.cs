using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.UserModel
{
    public class BrandProfileModel
    {
        public int BrandId { get; set; }
        public string UID { get; set; }
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public string CompanySize { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public int? EstablishedYear { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Tiktok { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Basic user info
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string ProfileImage { get; set; }
        public string Status { get; set; }
        public DateTime? LastLogin { get; set; }

        // Campaign statistics
        public int TotalCampaigns { get; set; }
        public int ActiveCampaigns { get; set; }
        public int CompletedCampaigns { get; set; }

        // Skills and Content
        public List<SkillModel> Skills { get; set; } = new List<SkillModel>();
        public List<ContentAndStyleModel> ContentAndStyles { get; set; } = new List<ContentAndStyleModel>();
    }
}
