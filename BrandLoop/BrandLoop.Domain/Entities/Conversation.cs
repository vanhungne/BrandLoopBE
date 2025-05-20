using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Conversation
    {
        [Key]
        public int ConversationId { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public int Status { get; set; } = 1; // 1:active, 0:closed

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<ConversationParticipant> Participants { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
