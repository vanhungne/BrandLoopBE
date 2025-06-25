using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.FeartureDTO
{
    public class ExclusiveBannerDto
    {
        public int InfluenceId { get; set; }
        public string BannerUrl { get; set; }
        public string TargetLink { get; set; }
    }

    public class SpotlightDto
    {
        public int InfluenceId { get; set; }
        public string Nickname { get; set; }
        public string ContentCategory { get; set; }
        public decimal EngagementRate { get; set; }
    }

}
