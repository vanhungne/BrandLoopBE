using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class CreateCampaignDto
    {
        public int BrandId { get; set; }
        public string CampaignName { get; set; }
        public string Description { get; set; }
        public string ContentRequirements { get; set; }
        public string CampaignGoals { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? Deadline { get; set; }
        public string CreatedBy { get; set; }
    }
}
