using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.Influence
{
    public class InfluenceSearchOptions
    {
        // Exact match by UID
        public string? UID { get; set; }

        // Full-text keyword
        public string? Keyword { get; set; }

        // Category, Location, Languages
        public string? ContentCategory { get; set; }
        public string? Location { get; set; }
        public string? Languages { get; set; }

        // Verified filter
        public bool? Verified { get; set; }

        // Gender, Influencer type
        public string? Gender { get; set; }
        public int? InfluencerTypeId { get; set; }

        // Numeric ranges: followers, engagement rate
        public int? MinFollowerCount { get; set; }
        public int? MaxFollowerCount { get; set; }
        public decimal? MinEngagementRate { get; set; }
        public decimal? MaxEngagementRate { get; set; }

        // Date filters
        public DateTime? CreatedAfter { get; set; }
        public DateTime? UpdatedAfter { get; set; }

        // Paging
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
