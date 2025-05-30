using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.UserModel
{
    public class BasicAccountProfileModel
    {
        public string UID { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string ProfileImage { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }

        // Skills and Content
        public List<SkillModel> Skills { get; set; } = new List<SkillModel>();
        public List<ContentAndStyleModel> ContentAndStyles { get; set; } = new List<ContentAndStyleModel>();
    }
}
