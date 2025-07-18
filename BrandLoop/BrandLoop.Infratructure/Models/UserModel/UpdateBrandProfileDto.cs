using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.UserModel
{
    public class UpdateBrandProfileDto
    {
        [StringLength(255)]
        public string? CompanyName { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        [StringLength(255)]
        public string? Logo { get; set; }

        [StringLength(50)]
        public string? CompanySize { get; set; }

        public string? Description { get; set; }

        public string? Address { get; set; }

        [StringLength(50)]
        public string? TaxCode { get; set; }

        public int? EstablishedYear { get; set; }

        [StringLength(255)]
        public string? Facebook { get; set; }

        [StringLength(255)]
        public string? Instagram { get; set; }

        [StringLength(255)]
        public string? Tiktok { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }
    }
}
