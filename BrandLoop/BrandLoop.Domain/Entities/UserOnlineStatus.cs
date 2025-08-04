using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class UserOnlineStatus
    {
        [Key]
        [StringLength(32)]
        public string UserId { get; set; }

        public bool IsOnline { get; set; } = true;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string ConnectionId { get; set; } // SignalR Connection ID

        // For multiple device support
        [StringLength(50)]
        public string DeviceType { get; set; } // Mobile, Web, Desktop

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
