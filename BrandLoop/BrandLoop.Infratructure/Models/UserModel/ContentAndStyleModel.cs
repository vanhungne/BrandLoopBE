using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.UserModel
{
    public class ContentAndStyleModel
    {
        public int ContentId { get; set; }
        public string UID { get; set; }
        public string ContentName { get; set; }
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
