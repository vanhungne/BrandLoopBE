using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.UserModel
{
    public class InfluenceProfileResponseDto
    {
        public int InfluenceId { get; set; }
        public string Nickname { get; set; }
        public string Bio { get; set; }
        public string ContentCategory { get; set; }
        public string Location { get; set; }
        public string Languages { get; set; }
        public string PortfolioUrl { get; set; }
        public decimal? AverageRate { get; set; }
        public bool Verified { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Tiktok { get; set; }
        public string Youtube { get; set; }
        public int? FollowerCount { get; set; }
        public decimal? EngagementRate { get; set; }
        public string Gender { get; set; }
        public DateOnly? DayOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
