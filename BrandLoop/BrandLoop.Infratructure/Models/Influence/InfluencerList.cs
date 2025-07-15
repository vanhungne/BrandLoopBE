using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.Influence
{
    public class InfluencerList
    {
        public string UID { get; set; }
        public string FullName { get; set; }
        public string ContentCategory { get; set; }
        public string Bio { get; set; }
        public string ProfileImage { get; set; }
        public string InfluencerType { get; set; }
        public int FollowerCount { get; set; }
        public int PlatformFee { get; set; }
        public string Location { get; set; }
    }

    public class InfluencerTypeSelectionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
