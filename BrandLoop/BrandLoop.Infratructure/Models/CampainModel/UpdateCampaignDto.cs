using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Enums;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class UpdateCampaignDto
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string Description { get; set; }
        public string ContentRequirements { get; set; }
        public string CampaignGoals { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? Deadline { get; set; }
        public CampainStatus Status { get; set; }
    }

}
