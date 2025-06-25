using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Enums
{
    public enum MessageStatus
    {
        Sent = 1,      // Message sent by sender
        Delivered = 2, // Message delivered to receiver
        Read = 3,      // Message read by receiver
        Failed = 4     // Message failed to send
    }
}
