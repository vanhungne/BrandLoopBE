using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class ConversationParticipant
    {
        [Key]
        public int Id { get; set; }

        public int ConversationId { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.Now;

        public DateTime? LeftAt { get; set; }

        public int Status { get; set; } = 1;

        // Navigation properties
        [ForeignKey("ConversationId")]
        public virtual Conversation Conversation { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }
    }
}
