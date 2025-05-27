using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        public int ConversationId { get; set; }

        [Required]
        [StringLength(32)]
        public string Sender { get; set; }

        [Required]
        public string Content { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [StringLength(255)]
        public string AttachmentUrl { get; set; }

        // Navigation properties
        [ForeignKey("ConversationId")]
        public virtual Conversation Conversation { get; set; }

        [ForeignKey("Sender")]
        public virtual User SenderUser { get; set; }

        public virtual ICollection<MessageReadStatus> ReadStatuses { get; set; }
    }
}
