using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class CampaignImageDto
    {
        public int CampaignImageId { get; set; }

        public int CampaignId { get; set; }
        public string ImageUrl { get; set; }

        public string Description { get; set; }
    }
}
