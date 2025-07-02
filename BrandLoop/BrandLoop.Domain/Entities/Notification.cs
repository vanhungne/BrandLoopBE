using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Enums;

namespace BrandLoop.Domain.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        [StringLength(32)]
        public string UID { get; set; }

        [Required]
        [StringLength(50)]
        public string NotificationType { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(20)]
        public NotificationStatus Status { get; set; } = NotificationStatus.Unread;

        public NotificationTarget Target { get; set; } = NotificationTarget.All;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? RelatedId { get; set; }

        // Navigation properties
        [ForeignKey("UID")]
        public virtual User User { get; set; }
    }

}
