using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.Authen
{
    public class PendingRegistrationDto
    {
        public string UID { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }

        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Website { get; set; }

        public string logoUrl { get; set; }
        public string imageUrl { get; set; }
        public string Nickname { get; set; }
        public string ContentCategory { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
    }

}
