using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.UserModel
{
    public class UpdateInfluenceProfileDto
    {
        [StringLength(100)]
        public string Nickname { get; set; }

        public string Bio { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(100)]
        public string ContentCategory { get; set; }

        [StringLength(255)]
        public string Location { get; set; }

        [StringLength(255)]
        public string Languages { get; set; }


        [StringLength(255)]
        public string PortfolioUrl { get; set; }

        [StringLength(255)]
        public string Facebook { get; set; }

        [StringLength(255)]
        public string Instagram { get; set; }

        [StringLength(255)]
        public string Tiktok { get; set; }

        [StringLength(255)]
        public string Youtube { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        public DateOnly? DayOfBirth { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }
        public string FullName { get; set; }
    }
}
