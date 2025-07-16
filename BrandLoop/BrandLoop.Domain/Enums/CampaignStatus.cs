using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Enums
{
    public enum CampaignStatus
    {
        Approved = 1,
        Rejected = 2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5,
        Deleted = 6,
        Overdue = 7,
    }
}
