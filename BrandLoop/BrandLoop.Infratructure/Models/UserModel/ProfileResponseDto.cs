using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.UserModel
{
    public class ProfileResponseDto
    {
        public string UID { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string ProfileImage { get; set; }
        public DateTime? LastLogin { get; set; }
        public BrandProfileResponseDto BrandProfile { get; set; }
        public InfluenceProfileResponseDto InfluenceProfile { get; set; }
    }
}
