using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.Authen
{
    public class RegisterKOLModel : RegisterBaseModel
    {
        [StringLength(100)]
        public string Nickname { get; set; }

        public string Bio { get; set; }

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

        public int? FollowerCount { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}
