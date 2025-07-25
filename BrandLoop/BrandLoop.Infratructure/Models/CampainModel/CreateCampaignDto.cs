﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.CampainModel
{
    public class CreateCampaignDto
    {
        public string CampaignName { get; set; }
        public string Description { get; set; }
        public string ContentRequirements { get; set; }
        public string CampaignGoals { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? Deadline { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();

        public List<string> ImageDescriptions { get; set; } = new List<string>();
    }
}
