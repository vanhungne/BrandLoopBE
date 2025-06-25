using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.BannerDTO
{
    public class BannerDto
    {
        public int BannerId { get; set; }
        public int InfluenceId { get; set; }
        public string ImageUrl { get; set; }
        public string TargetUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Nickname { get; set; }
    }
}
