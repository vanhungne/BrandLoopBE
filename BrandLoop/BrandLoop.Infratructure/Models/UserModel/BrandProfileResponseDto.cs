using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.UserModel
{
    public class BrandProfileResponseDto
    {
        public int BrandId { get; set; }
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public string CompanySize { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public int? EstablishedYear { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Tiktok { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Traffic { get; set; }
    }

}
