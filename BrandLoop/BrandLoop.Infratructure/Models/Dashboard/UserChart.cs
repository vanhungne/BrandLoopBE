using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.Dashboard
{
    public class UserChart
    {
        public int totalNewBrands { get; set; } = 0;
        public int totalNewInfluencers { get; set; } = 0;
        public int totalNewUsers { get; set; } = 0;
        public List<UserPerMonth> userPerMonths { get; set; } = new List<UserPerMonth>();
    }

    public class  UserPerMonth
    {
        public int month { get; set; }
        public int newBrands { get; set; } = 0;
        public int newInfluencers { get; set; } = 0;
    }
}
