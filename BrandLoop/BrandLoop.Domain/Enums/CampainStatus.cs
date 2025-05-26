using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Enums
{
    public enum CampainStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        InProgress = 4,
        Completed = 5,
        Cancelled = 6,
        Deleted = 7
    }
}
