using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BrandLoop.Domain.Entities
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        [StringLength(32)]
        public string SenderId { get; set; }

        [Required]
        [StringLength(32)]
        public string ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }

        // Message type and status
        public MessageType MessageType { get; set; } = MessageType.Text;
        public MessageStatus Status { get; set; } = MessageStatus.Sent;

        // Reply functionality
        public int? ReplyToMessageId { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; } // Soft delete

        // Attachment info
        [StringLength(255)]
        public string? AttachmentUrl { get; set; }
        [StringLength(255)]
        public string? AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }

        // Navigation properties
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        [ForeignKey("ReplyToMessageId")]
        public virtual Message ReplyToMessage { get; set; }

        public virtual ICollection<Message> Replies { get; set; }
        public virtual ICollection<MessageReadStatus> ReadStatuses { get; set; }

    }
}